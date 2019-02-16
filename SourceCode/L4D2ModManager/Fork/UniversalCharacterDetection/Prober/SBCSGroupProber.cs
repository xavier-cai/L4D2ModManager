using System;
using System.Collections.Generic;
using System.Text;
using Mozilla.UniversalCharacterDetection.Prober.Sequence;

namespace Mozilla.UniversalCharacterDetection.Prober
{
	/// <summary>
	/// Description of SBCSGroupProber.
	/// </summary>
	public class SBCSGroupProber : CharsetProber
	{
		private ProbingState        state;
		private CharsetProber[]     probers;
		private bool[]           isActive;
		private int                 bestGuess;
		private int                 activeNum;

		private static SequenceModel win1251Model = new Win1251Model();
		private static SequenceModel koi8rModel = new Koi8rModel();
		private static SequenceModel latin5Model = new Latin5Model();
		private static SequenceModel macCyrillicModel = new MacCyrillicModel();
		private static SequenceModel ibm866Model = new Ibm866Model();
		private static SequenceModel ibm855Model = new Ibm855Model();
		private static SequenceModel latin7Model = new Latin7Model();
		private static SequenceModel win1253Model = new Win1253Model();
		private static SequenceModel latin5BulgarianModel = new Latin5BulgarianModel();
		private static SequenceModel win1251BulgarianModel = new Win1251BulgarianModel();
		private static SequenceModel hebrewModel = new HebrewModel();
		
		public SBCSGroupProber():base()
		{
			this.probers = new CharsetProber[13];
			this.isActive = new bool[13];
			
			this.probers[0] = new SingleByteCharsetProber(win1251Model);
			this.probers[1] = new SingleByteCharsetProber(koi8rModel);
			this.probers[2] = new SingleByteCharsetProber(latin5Model);
			this.probers[3] = new SingleByteCharsetProber(macCyrillicModel);
			this.probers[4] = new SingleByteCharsetProber(ibm866Model);
			this.probers[5] = new SingleByteCharsetProber(ibm855Model);
			this.probers[6] = new SingleByteCharsetProber(latin7Model);
			this.probers[7] = new SingleByteCharsetProber(win1253Model);
			this.probers[8] = new SingleByteCharsetProber(latin5BulgarianModel);
			this.probers[9] = new SingleByteCharsetProber(win1251BulgarianModel);
			
			HebrewProber hebprober = new HebrewProber();
			this.probers[10] = hebprober;
			this.probers[11] = new SingleByteCharsetProber(hebrewModel, false, hebprober);
			this.probers[12] = new SingleByteCharsetProber(hebrewModel, true, hebprober);
			hebprober.setModalProbers(this.probers[11], this.probers[12]);
			
			reset();
		}
		
		public override string getCharSetName()
		{
			if (this.bestGuess == -1)
            {
				getConfidence();
				if (this.bestGuess == -1)
                {
					this.bestGuess = 0;
				}
			}
			
			return this.probers[this.bestGuess].getCharSetName();
		}

		public override float getConfidence()
		{
			float bestConf = 0.0f;
			float cf;

			if (this.state == ProbingState.FOUND_IT)
            {
				return 0.99f;
			}
            else if (this.state == ProbingState.NOT_ME)
            {
				return 0.01f;
			}
            else
            {
				for (int i=0; i<probers.Length; ++i)
                {
					if (!this.isActive[i])
                    {
						continue;
					}
					
					cf = this.probers[i].getConfidence();
					if (bestConf < cf)
                    {
						bestConf = cf;
						this.bestGuess = i;
					}
				}
			}

			return bestConf;
		}
		
		public override ProbingState getState()
		{
			return this.state;
		}
		
		public override ProbingState handleData(byte[] buf, int offset, int length)
		{
			ProbingState st;
			
			do
            {
				ByteBuffer newbuf = filterWithoutEnglishLetters(buf, offset, length);
				if (newbuf.Position == 0)
                {
					break;
				}
				
				for (int i=0; i<this.probers.Length; ++i)
                {
					if (!this.isActive[i])
                    {
						continue;
					}
					st = this.probers[i].handleData(newbuf.ToByteArray(), 0, newbuf.Position);
					if (st == ProbingState.FOUND_IT)
                    {
						this.bestGuess = i;
						this.state = ProbingState.FOUND_IT;
						break;
					}
                    else if (st == ProbingState.NOT_ME)
                    {
						this.isActive[i] = false;
						--this.activeNum;
						if (this.activeNum <= 0)
                        {
							this.state = ProbingState.NOT_ME;
							break;
						}
					}
				}
			} while (false);
			
			return this.state;
		}

		
		public override void reset()
		{
			this.activeNum = 0;
			for (int i=0; i<this.probers.Length; ++i)
            {
				this.probers[i].reset();
				this.isActive[i] = true;
				++this.activeNum;
			}
			
			this.bestGuess = -1;
			this.state = ProbingState.DETECTING;
		}
		
		public override void setOption()
		{ }
	}
}
