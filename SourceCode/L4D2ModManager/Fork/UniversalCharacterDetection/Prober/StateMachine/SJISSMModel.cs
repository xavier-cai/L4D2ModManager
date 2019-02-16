using System;
using System.Collections.Generic;
using System.Text;

namespace Mozilla.UniversalCharacterDetection.Prober.StateMachine
{
    public class SJISSMModel : SMModel
    {
        public static int SJIS_CLASS_FACTOR = 6;

        public SJISSMModel() : base(new PkgInt(PkgInt.INDEX_SHIFT_4BITS, PkgInt.SHIFT_MASK_4BITS, PkgInt.BIT_SHIFT_4BITS, PkgInt.UNIT_MASK_4BITS, sjisClassTable), SJIS_CLASS_FACTOR, new PkgInt(PkgInt.INDEX_SHIFT_4BITS, PkgInt.SHIFT_MASK_4BITS, PkgInt.BIT_SHIFT_4BITS, PkgInt.UNIT_MASK_4BITS, sjisStateTable), sjisCharLenTable, Constants.CHARSET_SHIFT_JIS)
        {

        }

        private static int[] sjisClassTable = new int[]
        {
            // PkgInt.pack4bits(0,1,1,1,1,1,1,1),  // 00 - 07 
            PkgInt.pack4bits(1,1,1,1,1,1,1,1),  // 00 - 07 
            PkgInt.pack4bits(1,1,1,1,1,1,0,0),  // 08 - 0f 
            PkgInt.pack4bits(1,1,1,1,1,1,1,1),  // 10 - 17 
            PkgInt.pack4bits(1,1,1,0,1,1,1,1),  // 18 - 1f 
            PkgInt.pack4bits(1,1,1,1,1,1,1,1),  // 20 - 27 
            PkgInt.pack4bits(1,1,1,1,1,1,1,1),  // 28 - 2f 
            PkgInt.pack4bits(1,1,1,1,1,1,1,1),  // 30 - 37 
            PkgInt.pack4bits(1,1,1,1,1,1,1,1),  // 38 - 3f 
            PkgInt.pack4bits(2,2,2,2,2,2,2,2),  // 40 - 47 
            PkgInt.pack4bits(2,2,2,2,2,2,2,2),  // 48 - 4f 
            PkgInt.pack4bits(2,2,2,2,2,2,2,2),  // 50 - 57 
            PkgInt.pack4bits(2,2,2,2,2,2,2,2),  // 58 - 5f 
            PkgInt.pack4bits(2,2,2,2,2,2,2,2),  // 60 - 67 
            PkgInt.pack4bits(2,2,2,2,2,2,2,2),  // 68 - 6f 
            PkgInt.pack4bits(2,2,2,2,2,2,2,2),  // 70 - 77 
            PkgInt.pack4bits(2,2,2,2,2,2,2,1),  // 78 - 7f 
            PkgInt.pack4bits(3,3,3,3,3,3,3,3),  // 80 - 87 
            PkgInt.pack4bits(3,3,3,3,3,3,3,3),  // 88 - 8f 
            PkgInt.pack4bits(3,3,3,3,3,3,3,3),  // 90 - 97 
            PkgInt.pack4bits(3,3,3,3,3,3,3,3),  // 98 - 9f 
            // 0xa0 is illegal in sjis encoding, but some pages does 
            // contain such byte. We need to be more error forgiven.
            PkgInt.pack4bits(2,2,2,2,2,2,2,2),  // a0 - a7     
            PkgInt.pack4bits(2,2,2,2,2,2,2,2),  // a8 - af 
            PkgInt.pack4bits(2,2,2,2,2,2,2,2),  // b0 - b7 
            PkgInt.pack4bits(2,2,2,2,2,2,2,2),  // b8 - bf 
            PkgInt.pack4bits(2,2,2,2,2,2,2,2),  // c0 - c7 
            PkgInt.pack4bits(2,2,2,2,2,2,2,2),  // c8 - cf 
            PkgInt.pack4bits(2,2,2,2,2,2,2,2),  // d0 - d7 
            PkgInt.pack4bits(2,2,2,2,2,2,2,2),  // d8 - df 
            PkgInt.pack4bits(3,3,3,3,3,3,3,3),  // e0 - e7 
            PkgInt.pack4bits(3,3,3,3,3,4,4,4),  // e8 - ef 
            PkgInt.pack4bits(4,4,4,4,4,4,4,4),  // f0 - f7 
            PkgInt.pack4bits(4,4,4,4,4,0,0,0)   // f8 - ff 
        };

        private static int[] sjisStateTable = new int[]
        {
            PkgInt.pack4bits(ERROR,START,START, 3,ERROR,ERROR,ERROR,ERROR),//00-07 
            PkgInt.pack4bits(ERROR,ERROR,ERROR,ERROR,ITSME,ITSME,ITSME,ITSME),//08-0f 
            PkgInt.pack4bits(ITSME,ITSME,ERROR,ERROR,START,START,START,START) //10-17 
        };

        private static int[] sjisCharLenTable = new int[]
        {
            0, 1, 1, 2, 0, 0
        };
    }
}
