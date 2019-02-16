using System;
using System.Collections.Generic;
using System.Text;
using Mozilla.UniversalCharacterDetection.Prober.Sequence;

namespace Mozilla.UniversalCharacterDetection.Prober
{
	/// <summary>
	/// Description of SingleByteCharsetProber.
	/// </summary>
	public class SingleByteCharsetProber : CharsetProber
    {
        public static int SAMPLE_SIZE = 64;
        public static int SB_ENOUGH_REL_THRESHOLD = 1024;
        public static float POSITIVE_SHORTCUT_THRESHOLD = 0.95f;
        public static float NEGATIVE_SHORTCUT_THRESHOLD = 0.05f;
        public static int SYMBOL_CAT_ORDER = 250;
        public static int NUMBER_OF_SEQ_CAT = 4;
        public static int POSITIVE_CAT = NUMBER_OF_SEQ_CAT-1;
        public static int NEGATIVE_CAT = 0;
    
        private ProbingState state;
        private SequenceModel model;
        private bool reversed;
    
        private short lastOrder;

        private int totalSeqs;
        private int[] seqCounters;
    
        private int totalChar;
        private int freqChar;
    
        private CharsetProber   nameProber;
    
        public SingleByteCharsetProber(SequenceModel model) : base()
        {
            this.model = model;
            this.reversed = false;
            this.nameProber = null;
            this.seqCounters = new int[NUMBER_OF_SEQ_CAT];
            reset();
        }
    
        public SingleByteCharsetProber(SequenceModel model, bool reversed, CharsetProber nameProber):base()
        {
            this.model = model;
            this.reversed = reversed;
            this.nameProber = nameProber;
            this.seqCounters = new int[NUMBER_OF_SEQ_CAT];
            reset();
        }
    
        bool keepEnglishLetters()
        {
            return this.model.getKeepEnglishLetter();
        }
    
        public override string getCharSetName()
        {
            if (this.nameProber == null)
            {
                return this.model.getCharsetName();
            }
            else
            {
                return this.nameProber.getCharSetName();
            }
        }
    
        public override float getConfidence()
        {
            if (this.totalSeqs > 0)
            {
                float r = 1.0f * this.seqCounters[POSITIVE_CAT] / this.totalSeqs / this.model.getTypicalPositiveRatio();
                r = r * this.freqChar / this.totalChar;
                if (r >= 1.0f)
                {
                    r = 0.99f;
                }
                return r;
            }

            return 0.01f;
        }

        public override ProbingState getState()
        {
            return this.state;
        }

        public override ProbingState handleData(byte[] buf, int offset, int length)
        {
            short order;
        
            int maxPos = offset + length;
            for (int i=offset; i<maxPos; ++i)
            {
                order = this.model.getOrder(buf[i]);
            
                if (order < SYMBOL_CAT_ORDER)
                {
                    ++this.totalChar;
                }
                if (order < SAMPLE_SIZE)
                {
                    ++this.freqChar;
                    if (this.lastOrder < SAMPLE_SIZE)
                    {
                        ++this.totalSeqs;
                        if (!this.reversed)
                        {
                            ++(this.seqCounters[this.model.getPrecedence(this.lastOrder*SAMPLE_SIZE+order)]);
                        }
                        else
                        {
                            ++(this.seqCounters[this.model.getPrecedence(order*SAMPLE_SIZE+this.lastOrder)]);
                        }
                    }
                }
                this.lastOrder = order;
            }
        
            if (this.state == ProbingState.DETECTING)
            {
                if (this.totalSeqs > SB_ENOUGH_REL_THRESHOLD)
                {
                    float cf = getConfidence();
                    if (cf > POSITIVE_SHORTCUT_THRESHOLD)
                    {
                        this.state = ProbingState.FOUND_IT;
                    }
                    else if (cf < NEGATIVE_SHORTCUT_THRESHOLD)
                    {
                        this.state = ProbingState.NOT_ME;
                    }
                }
            }
        
            return this.state;
        }

    
        public override void reset()
        {
            this.state = ProbingState.DETECTING;
            this.lastOrder = 255;
            for (int i=0; i<NUMBER_OF_SEQ_CAT; ++i)
            {
                this.seqCounters[i] = 0;
            }
            this.totalSeqs = 0;
            this.totalChar = 0;
            this.freqChar = 0;
        }

    
        public override void setOption()
        { }
    }
}
