using System;
using System.Collections.Generic;
using System.Text;
using Mozilla.UniversalCharacterDetection;
using Mozilla.UniversalCharacterDetection.Prober.ContextAnalysis;
using Mozilla.UniversalCharacterDetection.Prober.DistributionAnalysis;
using Mozilla.UniversalCharacterDetection.Prober.StateMachine;

namespace Mozilla.UniversalCharacterDetection.Prober
{
	/// <summary>
	/// Description of EUCJPProber.
	/// </summary>
	public class EUCJPProber : CharsetProber
    {
        private CodingStateMachine          codingSM;
        private ProbingState                state;
    
        private EUCJPContextAnalysis        contextAnalyzer;
        private EUCJPDistributionAnalysis   distributionAnalyzer;
    
        private byte[]                      lastChar;

        private static SMModel smModel = new EUCJPSMModel();

        public EUCJPProber():base()
        {
            this.codingSM = new CodingStateMachine(smModel);
            this.contextAnalyzer = new EUCJPContextAnalysis();
            this.distributionAnalyzer = new EUCJPDistributionAnalysis();
            this.lastChar = new byte[2];
            reset();
        }

        public override string getCharSetName()
        {
            return Constants.CHARSET_EUC_JP;
        }

        public override float getConfidence()
        {
            float contextCf = this.contextAnalyzer.getConfidence();
            float distribCf = this.distributionAnalyzer.getConfidence();
        
            return Math.Max(contextCf, distribCf);
        }

        public override ProbingState getState()
        {
            return this.state;
        }

        public override ProbingState handleData(byte[] buf, int offset, int length)
        {
            int codingState;
        
            int maxPos = offset + length;
            for (int i=offset; i<maxPos; ++i)
            {
                codingState = this.codingSM.nextState(buf[i]);
                if (codingState == SMModel.ERROR)
                {
                    this.state = ProbingState.NOT_ME;
                    break;
                }
                if (codingState == SMModel.ITSME)
                {
                    this.state = ProbingState.FOUND_IT;
                    break;
                }
                if (codingState == SMModel.START)
                {
                    int charLen = this.codingSM.getCurrentCharLen();
                
                    if (i == offset)
                    {
                        this.lastChar[1] = buf[offset];
                        this.contextAnalyzer.handleOneChar(this.lastChar, 0, charLen);
                        this.distributionAnalyzer.handleOneChar(this.lastChar, 0, charLen);
                    }
                    else
                    {
                        this.contextAnalyzer.handleOneChar(buf, i-1, charLen);
                        this.distributionAnalyzer.handleOneChar(buf, i-1, charLen);
                    }
                }
            }
        
            this.lastChar[0] = buf[maxPos-1];
        
            if (this.state == ProbingState.DETECTING)
            {
                if (this.contextAnalyzer.gotEnoughData() && getConfidence() > SHORTCUT_THRESHOLD)
                {
                    this.state = ProbingState.FOUND_IT;
                }
            }
        
            return this.state;
        }

        public override void reset()
        {
            this.codingSM.reset();
            this.state = ProbingState.DETECTING;
            this.contextAnalyzer.reset();
            this.distributionAnalyzer.reset();
            Array.Clear(this.lastChar,0,this.lastChar.Length);
        }

        public override void setOption()
        { }
    }
}
