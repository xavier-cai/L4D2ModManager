using System;
using System.Collections.Generic;
using System.Text;

namespace Mozilla.UniversalCharacterDetection.Prober.StateMachine
{
    public class EUCJPSMModel : SMModel
    {
        public static int EUCJP_CLASS_FACTOR = 6;

        public EUCJPSMModel() : base(new PkgInt(PkgInt.INDEX_SHIFT_4BITS, PkgInt.SHIFT_MASK_4BITS, PkgInt.BIT_SHIFT_4BITS, PkgInt.UNIT_MASK_4BITS, eucjpClassTable), EUCJP_CLASS_FACTOR, new PkgInt(PkgInt.INDEX_SHIFT_4BITS, PkgInt.SHIFT_MASK_4BITS, PkgInt.BIT_SHIFT_4BITS, PkgInt.UNIT_MASK_4BITS, eucjpStateTable), eucjpCharLenTable, Constants.CHARSET_EUC_JP)
        {

        }

        private static int[] eucjpClassTable = new int[]
        {
            // PkgInt.pack4bits(5,4,4,4,4,4,4,4),  // 00 - 07 
            PkgInt.pack4bits(4,4,4,4,4,4,4,4),  // 00 - 07 
            PkgInt.pack4bits(4,4,4,4,4,4,5,5),  // 08 - 0f 
            PkgInt.pack4bits(4,4,4,4,4,4,4,4),  // 10 - 17 
            PkgInt.pack4bits(4,4,4,5,4,4,4,4),  // 18 - 1f 
            PkgInt.pack4bits(4,4,4,4,4,4,4,4),  // 20 - 27 
            PkgInt.pack4bits(4,4,4,4,4,4,4,4),  // 28 - 2f 
            PkgInt.pack4bits(4,4,4,4,4,4,4,4),  // 30 - 37 
            PkgInt.pack4bits(4,4,4,4,4,4,4,4),  // 38 - 3f 
            PkgInt.pack4bits(4,4,4,4,4,4,4,4),  // 40 - 47 
            PkgInt.pack4bits(4,4,4,4,4,4,4,4),  // 48 - 4f 
            PkgInt.pack4bits(4,4,4,4,4,4,4,4),  // 50 - 57 
            PkgInt.pack4bits(4,4,4,4,4,4,4,4),  // 58 - 5f 
            PkgInt.pack4bits(4,4,4,4,4,4,4,4),  // 60 - 67 
            PkgInt.pack4bits(4,4,4,4,4,4,4,4),  // 68 - 6f 
            PkgInt.pack4bits(4,4,4,4,4,4,4,4),  // 70 - 77 
            PkgInt.pack4bits(4,4,4,4,4,4,4,4),  // 78 - 7f 
            PkgInt.pack4bits(5,5,5,5,5,5,5,5),  // 80 - 87 
            PkgInt.pack4bits(5,5,5,5,5,5,1,3),  // 88 - 8f 
            PkgInt.pack4bits(5,5,5,5,5,5,5,5),  // 90 - 97 
            PkgInt.pack4bits(5,5,5,5,5,5,5,5),  // 98 - 9f 
            PkgInt.pack4bits(5,2,2,2,2,2,2,2),  // a0 - a7 
            PkgInt.pack4bits(2,2,2,2,2,2,2,2),  // a8 - af 
            PkgInt.pack4bits(2,2,2,2,2,2,2,2),  // b0 - b7 
            PkgInt.pack4bits(2,2,2,2,2,2,2,2),  // b8 - bf 
            PkgInt.pack4bits(2,2,2,2,2,2,2,2),  // c0 - c7 
            PkgInt.pack4bits(2,2,2,2,2,2,2,2),  // c8 - cf 
            PkgInt.pack4bits(2,2,2,2,2,2,2,2),  // d0 - d7 
            PkgInt.pack4bits(2,2,2,2,2,2,2,2),  // d8 - df 
            PkgInt.pack4bits(0,0,0,0,0,0,0,0),  // e0 - e7 
            PkgInt.pack4bits(0,0,0,0,0,0,0,0),  // e8 - ef 
            PkgInt.pack4bits(0,0,0,0,0,0,0,0),  // f0 - f7 
            PkgInt.pack4bits(0,0,0,0,0,0,0,5)   // f8 - ff 
        };

        private static int[] eucjpStateTable = new int[]
        {
            PkgInt.pack4bits(3, 4, 3, 5,START,ERROR,ERROR,ERROR),//00-07 
            PkgInt.pack4bits(ERROR,ERROR,ERROR,ERROR,ITSME,ITSME,ITSME,ITSME),//08-0f 
            PkgInt.pack4bits(ITSME,ITSME,START,ERROR,START,ERROR,ERROR,ERROR),//10-17 
            PkgInt.pack4bits(ERROR,ERROR,START,ERROR,ERROR,ERROR,    3,ERROR),//18-1f 
            PkgInt.pack4bits(3,ERROR,ERROR,ERROR,START,START,START,START) //20-27 
        };

        private static int[] eucjpCharLenTable = new int[] { 2, 2, 2, 3, 1, 0 };
    }
}
