using System;
using System.Collections.Generic;
using System.Text;

namespace Mozilla.UniversalCharacterDetection.Prober.StateMachine
{
    public class EUCTWSMModel : SMModel
    {
        public static int EUCTW_CLASS_FACTOR = 7;

        public EUCTWSMModel() : base(new PkgInt(PkgInt.INDEX_SHIFT_4BITS, PkgInt.SHIFT_MASK_4BITS, PkgInt.BIT_SHIFT_4BITS, PkgInt.UNIT_MASK_4BITS, euctwClassTable), EUCTW_CLASS_FACTOR, new PkgInt(PkgInt.INDEX_SHIFT_4BITS, PkgInt.SHIFT_MASK_4BITS, PkgInt.BIT_SHIFT_4BITS, PkgInt.UNIT_MASK_4BITS, euctwStateTable), euctwCharLenTable, Constants.CHARSET_EUC_TW)
        {
        }

        private static int[] euctwClassTable = new int[]
        {
            // PkgInt.pack4bits(0,2,2,2,2,2,2,2),  // 00 - 07 
            PkgInt.pack4bits(2,2,2,2,2,2,2,2),  // 00 - 07 
            PkgInt.pack4bits(2,2,2,2,2,2,0,0),  // 08 - 0f 
            PkgInt.pack4bits(2,2,2,2,2,2,2,2),  // 10 - 17 
            PkgInt.pack4bits(2,2,2,0,2,2,2,2),  // 18 - 1f 
            PkgInt.pack4bits(2,2,2,2,2,2,2,2),  // 20 - 27 
            PkgInt.pack4bits(2,2,2,2,2,2,2,2),  // 28 - 2f 
            PkgInt.pack4bits(2,2,2,2,2,2,2,2),  // 30 - 37 
            PkgInt.pack4bits(2,2,2,2,2,2,2,2),  // 38 - 3f 
            PkgInt.pack4bits(2,2,2,2,2,2,2,2),  // 40 - 47 
            PkgInt.pack4bits(2,2,2,2,2,2,2,2),  // 48 - 4f 
            PkgInt.pack4bits(2,2,2,2,2,2,2,2),  // 50 - 57 
            PkgInt.pack4bits(2,2,2,2,2,2,2,2),  // 58 - 5f 
            PkgInt.pack4bits(2,2,2,2,2,2,2,2),  // 60 - 67 
            PkgInt.pack4bits(2,2,2,2,2,2,2,2),  // 68 - 6f 
            PkgInt.pack4bits(2,2,2,2,2,2,2,2),  // 70 - 77 
            PkgInt.pack4bits(2,2,2,2,2,2,2,2),  // 78 - 7f 
            PkgInt.pack4bits(0,0,0,0,0,0,0,0),  // 80 - 87 
            PkgInt.pack4bits(0,0,0,0,0,0,6,0),  // 88 - 8f 
            PkgInt.pack4bits(0,0,0,0,0,0,0,0),  // 90 - 97 
            PkgInt.pack4bits(0,0,0,0,0,0,0,0),  // 98 - 9f 
            PkgInt.pack4bits(0,3,4,4,4,4,4,4),  // a0 - a7 
            PkgInt.pack4bits(5,5,1,1,1,1,1,1),  // a8 - af 
            PkgInt.pack4bits(1,1,1,1,1,1,1,1),  // b0 - b7 
            PkgInt.pack4bits(1,1,1,1,1,1,1,1),  // b8 - bf 
            PkgInt.pack4bits(1,1,3,1,3,3,3,3),  // c0 - c7 
            PkgInt.pack4bits(3,3,3,3,3,3,3,3),  // c8 - cf 
            PkgInt.pack4bits(3,3,3,3,3,3,3,3),  // d0 - d7 
            PkgInt.pack4bits(3,3,3,3,3,3,3,3),  // d8 - df 
            PkgInt.pack4bits(3,3,3,3,3,3,3,3),  // e0 - e7 
            PkgInt.pack4bits(3,3,3,3,3,3,3,3),  // e8 - ef 
            PkgInt.pack4bits(3,3,3,3,3,3,3,3),  // f0 - f7 
            PkgInt.pack4bits(3,3,3,3,3,3,3,0)   // f8 - ff 
        };

        private static int[] euctwStateTable = new int[]
        {
            PkgInt.pack4bits(ERROR,ERROR,START, 3, 3, 3, 4,ERROR),//00-07 
            PkgInt.pack4bits(ERROR,ERROR,ERROR,ERROR,ERROR,ERROR,ITSME,ITSME),//08-0f 
            PkgInt.pack4bits(ITSME,ITSME,ITSME,ITSME,ITSME,ERROR,START,ERROR),//10-17 
            PkgInt.pack4bits(START,START,START,ERROR,ERROR,ERROR,ERROR,ERROR),//18-1f 
            PkgInt.pack4bits(5,ERROR,ERROR,ERROR,START,ERROR,START,START),//20-27 
            PkgInt.pack4bits(START,ERROR,START,START,START,START,START,START) //28-2f 
        };

        private static int[] euctwCharLenTable = new int[]
        {
            0, 0, 1, 2, 2, 2, 3
        };
    }
}
