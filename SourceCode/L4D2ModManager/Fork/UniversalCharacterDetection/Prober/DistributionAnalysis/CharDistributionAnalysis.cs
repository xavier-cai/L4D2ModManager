using System;
using System.Collections.Generic;
using System.Text;

namespace Mozilla.UniversalCharacterDetection.Prober.DistributionAnalysis
{
    public abstract class CharDistributionAnalysis
    {
        public static float SURE_NO = 0.01f;
        public static float SURE_YES = 0.99f;
        public static int ENOUGH_DATA_THRESHOLD = 1024;
        public static int MINIMUM_DATA_THRESHOLD = 4;

        private int freqChars;
        private int totalChars;
        protected int[] charToFreqOrder; // set by subclasses
        protected float typicalDistributionRatio; // set by subclasses
        protected bool done; // set by subclasses and reset()

        public CharDistributionAnalysis()
        {
            reset();
        }

        public void handleData(byte[] buf, int offset, int length)
        { }

        public void handleOneChar(byte[] buf, int offset, int charLength)
        {
            int order = -1;

            if (charLength == 2)
            {
                order = getOrder(buf, offset);
            }

            if (order >= 0)
            {
                ++this.totalChars;
                if (order < this.charToFreqOrder.Length)
                {
                    if (512 > this.charToFreqOrder[order])
                    {
                        ++this.freqChars;
                    }
                }
            }
        }

        public float getConfidence()
        {
            if (this.totalChars <= 0 || this.freqChars <= MINIMUM_DATA_THRESHOLD)
            {
                return SURE_NO;
            }

            if (this.totalChars != this.freqChars)
            {
                float r = this.freqChars / (this.totalChars - this.freqChars) * this.typicalDistributionRatio;

                if (r < SURE_YES)
                {
                    return r;
                }
            }

            return SURE_YES;
        }

        public void reset()
        {
            this.done = false;
            this.totalChars = 0;
            this.freqChars = 0;
        }

        public void setOption()
        { }

        public bool gotEnoughData()
        {
            return (this.totalChars > ENOUGH_DATA_THRESHOLD);
        }

        protected abstract int getOrder(byte[] buf, int offset);
    }
}
