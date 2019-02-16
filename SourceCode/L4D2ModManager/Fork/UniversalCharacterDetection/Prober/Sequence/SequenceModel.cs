using System;
using System.Collections.Generic;
using System.Text;

namespace Mozilla.UniversalCharacterDetection.Prober.Sequence
{
    public abstract class SequenceModel
    {
        protected short[] charToOrderMap;
        protected byte[] precedenceMatrix;
        protected float typicalPositiveRatio;
        protected bool keepEnglishLetter;
        protected string charsetName;

        public SequenceModel(short[] charToOrderMap, byte[] precedenceMatrix, float typicalPositiveRatio, bool keepEnglishLetter, string charsetName)
        {
            this.charToOrderMap = charToOrderMap;
            this.precedenceMatrix = precedenceMatrix;
            this.typicalPositiveRatio = typicalPositiveRatio;
            this.keepEnglishLetter = keepEnglishLetter;
            this.charsetName = charsetName;
        }

        public short getOrder(byte b)
        {
            int c = b & 0xFF;
            return this.charToOrderMap[c];
        }

        public byte getPrecedence(int pos)
        {
            return this.precedenceMatrix[pos];
        }

        public float getTypicalPositiveRatio()
        {
            return this.typicalPositiveRatio;
        }

        public bool getKeepEnglishLetter()
        {
            return this.keepEnglishLetter;
        }

        public string getCharsetName()
        {
            return this.charsetName;
        }
    }
}
