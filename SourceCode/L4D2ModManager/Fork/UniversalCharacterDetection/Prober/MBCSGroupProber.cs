using System;
using System.Collections.Generic;
using System.Text;

namespace Mozilla.UniversalCharacterDetection.Prober
{
	/// <summary>
	/// Description of MBCSGroupProber.
	/// </summary>
	public class MBCSGroupProber : CharsetProber
	{
		private ProbingState        state;
		private CharsetProber[]     probers;
		private bool[]           isActive;
		private int                 bestGuess;
		private int                 activeNum;

		public MBCSGroupProber():base()
		{
			this.probers = new CharsetProber[7];
			this.isActive = new bool[7];
			
			this.probers[0] = new UTF8Prober();
			this.probers[1] = new SJISProber();
			this.probers[2] = new EUCJPProber();
			this.probers[3] = new GB18030Prober();
			this.probers[4] = new EUCKRProber();
			this.probers[5] = new Big5Prober();
			this.probers[6] = new EUCTWProber();
			
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
			
			bool keepNext = true;
			byte[] highbyteBuf = new byte[length];
			int highpos = 0;

			int maxPos = offset + length;
			for (int i=offset; i<maxPos; ++i)
            {
				if ((buf[i] & 0x80) != 0)
                {
					highbyteBuf[highpos++] = buf[i];
					keepNext = true;
				}
                else
                {
					// If previous is highbyte, keep this even it is a ASCII
					if (keepNext)
                    {
						highbyteBuf[highpos++] = buf[i];
						keepNext = false;
					}
				}
			}
			
			for (int i=0; i<this.probers.Length; ++i)
            {
				if (!this.isActive[i])
                {
					continue;
				}
				st = this.probers[i].handleData(highbyteBuf, 0, highpos);
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
