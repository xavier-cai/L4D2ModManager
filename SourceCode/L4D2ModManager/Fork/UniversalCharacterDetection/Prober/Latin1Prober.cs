using System;
using System.Collections.Generic;
using System.Text;
using Mozilla.UniversalCharacterDetection;

namespace Mozilla.UniversalCharacterDetection.Prober
{
	/// <summary>
	/// Description of Latin1Prober.
	/// </summary>
	public class Latin1Prober : CharsetProber
	{
		public static byte UDF = 0;
		public static byte OTH = 1;
		public static byte ASC = 2;
		public static byte ASS = 3;
		public static byte ACV = 4;
		public static byte ACO = 5;
		public static byte ASV = 6;
		public static byte ASO = 7;
		public static int CLASS_NUM = 8;
		public static int FREQ_CAT_NUM = 4;
		
		private ProbingState state;
		private byte lastCharClass;
		private int[] freqCounter;
		
		public Latin1Prober():base()
		{
			this.freqCounter = new int[FREQ_CAT_NUM];
			reset();
		}

		public override string getCharSetName()
		{
			return Constants.CHARSET_WINDOWS_1252;
		}

		public override  float getConfidence()
		{
			if (this.state == ProbingState.NOT_ME) {
				return 0.01f;
			}
			
			float confidence;
			int total = 0;
			for (int i=0; i<this.freqCounter.Length; ++i) {
				total += this.freqCounter[i];
			}
			
			if (total <= 0) {
				confidence = 0.0f;
			} else {
				confidence = this.freqCounter[3] * 1.0f / total;
				confidence -= this.freqCounter[1] * 20.0f / total;
			}
			
			if (confidence < 0.0f) {
				confidence = 0.0f;
			}
			
			// Lower the confidence of latin1 so that other more accurate detector can take priority.
			confidence *= 0.50f;
			
			return confidence;
		}
		
		public override ProbingState getState()
		{
			return this.state;
		}

		public override ProbingState handleData(byte[] buf, int offset, int length)
		{
			ByteBuffer newBufTmp = filterWithEnglishLetters(buf, offset, length);

			byte charClass;
			byte freq;
			
			byte[] newBuf = newBufTmp.ToByteArray();
			int newBufLen = newBufTmp.Position;

			for (int i=0; i<newBufLen; ++i)
            {
				int c = newBuf[i] & 0xFF;
				charClass = latin1CharToClass[c];
				freq = latin1ClassModel[this.lastCharClass * CLASS_NUM + charClass];
				if (freq == 0)
                {
					this.state = ProbingState.NOT_ME;
					break;
				}
				++this.freqCounter[freq];
				this.lastCharClass = charClass;
			}

			return this.state;
		}

		public override void reset()
		{
			this.state = ProbingState.DETECTING;
			this.lastCharClass = OTH;
			for (int i=0; i<this.freqCounter.Length; ++i)
            {
				this.freqCounter[i] = 0;
			}
		}

		public override void setOption()
		{ }
		
		private static byte[] latin1CharToClass = new byte[]
        {
			OTH, OTH, OTH, OTH, OTH, OTH, OTH, OTH,   // 00 - 07
			OTH, OTH, OTH, OTH, OTH, OTH, OTH, OTH,   // 08 - 0F
			OTH, OTH, OTH, OTH, OTH, OTH, OTH, OTH,   // 10 - 17
			OTH, OTH, OTH, OTH, OTH, OTH, OTH, OTH,   // 18 - 1F
			OTH, OTH, OTH, OTH, OTH, OTH, OTH, OTH,   // 20 - 27
			OTH, OTH, OTH, OTH, OTH, OTH, OTH, OTH,   // 28 - 2F
			OTH, OTH, OTH, OTH, OTH, OTH, OTH, OTH,   // 30 - 37
			OTH, OTH, OTH, OTH, OTH, OTH, OTH, OTH,   // 38 - 3F
			OTH, ASC, ASC, ASC, ASC, ASC, ASC, ASC,   // 40 - 47
			ASC, ASC, ASC, ASC, ASC, ASC, ASC, ASC,   // 48 - 4F
			ASC, ASC, ASC, ASC, ASC, ASC, ASC, ASC,   // 50 - 57
			ASC, ASC, ASC, OTH, OTH, OTH, OTH, OTH,   // 58 - 5F
			OTH, ASS, ASS, ASS, ASS, ASS, ASS, ASS,   // 60 - 67
			ASS, ASS, ASS, ASS, ASS, ASS, ASS, ASS,   // 68 - 6F
			ASS, ASS, ASS, ASS, ASS, ASS, ASS, ASS,   // 70 - 77
			ASS, ASS, ASS, OTH, OTH, OTH, OTH, OTH,   // 78 - 7F
			OTH, UDF, OTH, ASO, OTH, OTH, OTH, OTH,   // 80 - 87
			OTH, OTH, ACO, OTH, ACO, UDF, ACO, UDF,   // 88 - 8F
			UDF, OTH, OTH, OTH, OTH, OTH, OTH, OTH,   // 90 - 97
			OTH, OTH, ASO, OTH, ASO, UDF, ASO, ACO,   // 98 - 9F
			OTH, OTH, OTH, OTH, OTH, OTH, OTH, OTH,   // A0 - A7
			OTH, OTH, OTH, OTH, OTH, OTH, OTH, OTH,   // A8 - AF
			OTH, OTH, OTH, OTH, OTH, OTH, OTH, OTH,   // B0 - B7
			OTH, OTH, OTH, OTH, OTH, OTH, OTH, OTH,   // B8 - BF
			ACV, ACV, ACV, ACV, ACV, ACV, ACO, ACO,   // C0 - C7
			ACV, ACV, ACV, ACV, ACV, ACV, ACV, ACV,   // C8 - CF
			ACO, ACO, ACV, ACV, ACV, ACV, ACV, OTH,   // D0 - D7
			ACV, ACV, ACV, ACV, ACV, ACO, ACO, ACO,   // D8 - DF
			ASV, ASV, ASV, ASV, ASV, ASV, ASO, ASO,   // E0 - E7
			ASV, ASV, ASV, ASV, ASV, ASV, ASV, ASV,   // E8 - EF
			ASO, ASO, ASV, ASV, ASV, ASV, ASV, OTH,   // F0 - F7
			ASV, ASV, ASV, ASV, ASV, ASO, ASO, ASO,   // F8 - FF
		};
		
		private static byte[] latin1ClassModel = new byte[]
        {
			/*      UDF OTH ASC ASS ACV ACO ASV ASO  */
			/*UDF*/  0,  0,  0,  0,  0,  0,  0,  0,
			/*OTH*/  0,  3,  3,  3,  3,  3,  3,  3,
			/*ASC*/  0,  3,  3,  3,  3,  3,  3,  3,
			/*ASS*/  0,  3,  3,  3,  1,  1,  3,  3,
			/*ACV*/  0,  3,  3,  3,  1,  2,  1,  2,
			/*ACO*/  0,  3,  3,  3,  3,  3,  3,  3,
			/*ASV*/  0,  3,  1,  3,  1,  1,  1,  3,
			/*ASO*/  0,  3,  1,  3,  1,  1,  3,  3,
		};
	}
}
