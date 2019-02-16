using System;
using System.Collections.Generic;
using System.Text;
using Mozilla.UniversalCharacterDetection.Prober.StateMachine;

namespace Mozilla.UniversalCharacterDetection.Prober
{
	/// <summary>
	/// Description of EscCharsetProber.
	/// </summary>
	public class EscCharsetProber : CharsetProber
    {
        private CodingStateMachine[]    codingSM;
        private int                     activeSM;
        private ProbingState            state;
        private string                  detectedCharset;
    
        private static HZSMModel hzsModel = new HZSMModel();
        private static ISO2022CNSMModel iso2022cnModel = new ISO2022CNSMModel();
        private static ISO2022JPSMModel iso2022jpModel = new ISO2022JPSMModel();
        private static ISO2022KRSMModel iso2022krModel = new ISO2022KRSMModel();

        public EscCharsetProber():base()
        {

            this.codingSM = new CodingStateMachine[4];
            this.codingSM[0] = new CodingStateMachine(hzsModel);
            this.codingSM[1] = new CodingStateMachine(iso2022cnModel);
            this.codingSM[2] = new CodingStateMachine(iso2022jpModel);
            this.codingSM[3] = new CodingStateMachine(iso2022krModel);

            reset();
        }
    
        public override string getCharSetName()
        {
            return this.detectedCharset;
        }

        public override float getConfidence()
        {
            return 0.99f;
        }

        public override ProbingState getState()
        {
            return this.state;
        }

        public override ProbingState handleData(byte[] buf, int offset, int length)
        {
            int codingState;
        
            int maxPos = offset + length;
            for (int i=offset; i<maxPos && this.state==ProbingState.DETECTING; ++i)
            {
                for (int j=this.activeSM-1; j>=0; --j)
                {
                    codingState = this.codingSM[j].nextState(buf[i]);
                    if (codingState == SMModel.ERROR)
                    {
                        --this.activeSM;
                        if (this.activeSM <= 0)
                        {
                            this.state = ProbingState.NOT_ME;
                            return this.state;
                        }
                        else if (j != this.activeSM)
                        {
                            CodingStateMachine t;
                            t = this.codingSM[this.activeSM];
                            this.codingSM[this.activeSM] = this.codingSM[j];
                            this.codingSM[j] = t;
                        }
                    }
                    else if (codingState == SMModel.ITSME)
                    {
                        this.state = ProbingState.FOUND_IT;
                        this.detectedCharset = this.codingSM[j].getCodingStateMachine();
                        return this.state;
                    }
                }
            }
        
            return this.state;
        }

        public override void reset()
        {
            this.state = ProbingState.DETECTING;
            for (int i=0; i<this.codingSM.Length; ++i)
            {
                this.codingSM[i].reset();
            }
            this.activeSM = this.codingSM.Length;
            this.detectedCharset = null;
        }

        public override void setOption()
        { }
    }
}
