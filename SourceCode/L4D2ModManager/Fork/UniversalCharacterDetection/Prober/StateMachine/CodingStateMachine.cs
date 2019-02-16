using System;
using System.Collections.Generic;
using System.Text;

namespace Mozilla.UniversalCharacterDetection.Prober.StateMachine
{
    public class CodingStateMachine
    {
        protected SMModel model;
        protected int currentState;
        protected int currentCharLen;
        protected int currentBytePos;

        public CodingStateMachine(SMModel model)
        {
            this.model = model;
            this.currentState = SMModel.START;
        }

        public int nextState(byte c)
        {
            int byteCls = this.model.getClass(c);
            if (this.currentState == SMModel.START)
            {
                this.currentBytePos = 0;
                this.currentCharLen = this.model.getCharLen(byteCls);
            }

            this.currentState = this.model.getNextState(byteCls, this.currentState);
            ++this.currentBytePos;

            return this.currentState;
        }

        public int getCurrentCharLen()
        {
            return this.currentCharLen;
        }

        public void reset()
        {
            this.currentState = SMModel.START;
        }

        public string getCodingStateMachine()
        {
            return this.model.getName();
        }
    }
}
