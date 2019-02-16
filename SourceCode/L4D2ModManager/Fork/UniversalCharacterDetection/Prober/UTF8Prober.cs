using System;
using System.Collections.Generic;
using System.Text;
using Mozilla.UniversalCharacterDetection;
using Mozilla.UniversalCharacterDetection.Prober.StateMachine;

namespace Mozilla.UniversalCharacterDetection.Prober
{
	/// <summary>
	/// Description of UTF8Prober.
	/// </summary>
	public class UTF8Prober : CharsetProber
{
    public static float ONE_CHAR_PROB = 0.50f;
    

    private CodingStateMachine codingSM;
    private ProbingState state;
    private int numOfMBChar;
    
    private static SMModel smModel = new UTF8SMModel();
    

    ////////////////////////////////////////////////////////////////
    // methods
    ////////////////////////////////////////////////////////////////
    public UTF8Prober():base()
    {
        this.numOfMBChar = 0;
        this.codingSM = new CodingStateMachine(smModel);

        reset();
    }

    public override string getCharSetName()
    {
        return Constants.CHARSET_UTF_8;
    }

    public override ProbingState handleData(byte[] buf, int offset, int length)
    {
        int codingState;

        int maxPos = offset + length;
        for (int i=offset; i<maxPos; ++i) {
            codingState = this.codingSM.nextState(buf[i]);
            if (codingState == SMModel.ERROR) {
                this.state = ProbingState.NOT_ME;
                break;
            }
            if (codingState == SMModel.ITSME) {
                this.state = ProbingState.FOUND_IT;
                break;
            }
            if (codingState == SMModel.START) {
                if (this.codingSM.getCurrentCharLen() >= 2) {
                    ++this.numOfMBChar;
                }
            }
        }
        
        if (this.state == ProbingState.DETECTING) {
            if (getConfidence() > SHORTCUT_THRESHOLD) {
                this.state = ProbingState.FOUND_IT;
            }
        }
        
        return this.state;
    }

    public override ProbingState getState()
    {
        return this.state;
    }

    public override void reset()
    {
        this.codingSM.reset();
        this.numOfMBChar = 0;
        this.state = ProbingState.DETECTING;
    }

    public override float getConfidence()
    {
        float unlike = 0.99f;
        
        if (this.numOfMBChar < 6) {
            for (int i=0; i<this.numOfMBChar; ++i) {
                unlike *= ONE_CHAR_PROB;
            }
            return (1.0f - unlike);
        } else {
            return 0.99f;
        }
    }

    public override void setOption()
    {}
}

}
