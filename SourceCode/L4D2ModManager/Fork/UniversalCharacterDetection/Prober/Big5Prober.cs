using System;
using System.Collections.Generic;
using System.Text;
using Mozilla.UniversalCharacterDetection;
using Mozilla.UniversalCharacterDetection.Prober.DistributionAnalysis;
using Mozilla.UniversalCharacterDetection.Prober.StateMachine;

namespace Mozilla.UniversalCharacterDetection.Prober
{
    /// <summary>
    /// Description of Big5Prober.
    /// </summary>
    public class Big5Prober : CharsetProber
    {
        private CodingStateMachine codingSM;
        private ProbingState state;

        private Big5DistributionAnalysis distributionAnalyzer;

        private byte[] lastChar;

        private static SMModel smModel = new Big5SMModel();

        public Big5Prober() : base()
        {
            this.codingSM = new CodingStateMachine(smModel);
            this.distributionAnalyzer = new Big5DistributionAnalysis();
            this.lastChar = new byte[2];
            reset();
        }

        public override string getCharSetName()
        {
            return Constants.CHARSET_BIG5;
        }

        public override float getConfidence()
        {
            float distribCf = this.distributionAnalyzer.getConfidence();

            return distribCf;
        }

        public override ProbingState getState()
        {
            return this.state;
        }

        public override ProbingState handleData(byte[] buf, int offset, int length)
        {
            int codingState;

            int maxPos = offset + length;
            for (int i = offset; i < maxPos; ++i)
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
                        this.distributionAnalyzer.handleOneChar(this.lastChar, 0, charLen);
                    }
                    else
                    {
                        this.distributionAnalyzer.handleOneChar(buf, i - 1, charLen);
                    }
                }
            }

            this.lastChar[0] = buf[maxPos - 1];

            if (this.state == ProbingState.DETECTING)
            {
                if (this.distributionAnalyzer.gotEnoughData() && getConfidence() > SHORTCUT_THRESHOLD)
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
            this.distributionAnalyzer.reset();
            Array.Clear(this.lastChar, 0, this.lastChar.Length);
        }

        public override void setOption()
        { }
    }
}
