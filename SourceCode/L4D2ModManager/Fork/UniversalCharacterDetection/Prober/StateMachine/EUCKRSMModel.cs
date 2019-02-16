using System;
using System.Collections.Generic;
using System.Text;

namespace Mozilla.UniversalCharacterDetection.Prober.StateMachine
{
    public class EUCKRSMModel : SMModel
    {
        public static int EUCKR_CLASS_FACTOR = 4;

        public EUCKRSMModel() : base(new PkgInt(PkgInt.INDEX_SHIFT_4BITS, PkgInt.SHIFT_MASK_4BITS, PkgInt.BIT_SHIFT_4BITS, PkgInt.UNIT_MASK_4BITS, euckrClassTable), EUCKR_CLASS_FACTOR, new PkgInt(PkgInt.INDEX_SHIFT_4BITS, PkgInt.SHIFT_MASK_4BITS, PkgInt.BIT_SHIFT_4BITS, PkgInt.UNIT_MASK_4BITS, euckrStateTable), euckrCharLenTable, Constants.CHARSET_EUC_KR)
        {
        }

        private static int[] euckrClassTable = new int[]
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
            PkgInt.pack4bits(1,1,1,1,1,1,1,1),  // 40 - 47 
            PkgInt.pack4bits(1,1,1,1,1,1,1,1),  // 48 - 4f 
            PkgInt.pack4bits(1,1,1,1,1,1,1,1),  // 50 - 57 
            PkgInt.pack4bits(1,1,1,1,1,1,1,1),  // 58 - 5f 
            PkgInt.pack4bits(1,1,1,1,1,1,1,1),  // 60 - 67 
            PkgInt.pack4bits(1,1,1,1,1,1,1,1),  // 68 - 6f 
            PkgInt.pack4bits(1,1,1,1,1,1,1,1),  // 70 - 77 
            PkgInt.pack4bits(1,1,1,1,1,1,1,1),  // 78 - 7f 
            PkgInt.pack4bits(0,0,0,0,0,0,0,0),  // 80 - 87 
            PkgInt.pack4bits(0,0,0,0,0,0,0,0),  // 88 - 8f 
            PkgInt.pack4bits(0,0,0,0,0,0,0,0),  // 90 - 97 
            PkgInt.pack4bits(0,0,0,0,0,0,0,0),  // 98 - 9f 
            PkgInt.pack4bits(0,2,2,2,2,2,2,2),  // a0 - a7 
            PkgInt.pack4bits(2,2,2,2,2,3,3,3),  // a8 - af 
            PkgInt.pack4bits(2,2,2,2,2,2,2,2),  // b0 - b7 
            PkgInt.pack4bits(2,2,2,2,2,2,2,2),  // b8 - bf 
            PkgInt.pack4bits(2,2,2,2,2,2,2,2),  // c0 - c7 
            PkgInt.pack4bits(2,3,2,2,2,2,2,2),  // c8 - cf 
            PkgInt.pack4bits(2,2,2,2,2,2,2,2),  // d0 - d7 
            PkgInt.pack4bits(2,2,2,2,2,2,2,2),  // d8 - df 
            PkgInt.pack4bits(2,2,2,2,2,2,2,2),  // e0 - e7 
            PkgInt.pack4bits(2,2,2,2,2,2,2,2),  // e8 - ef 
            PkgInt.pack4bits(2,2,2,2,2,2,2,2),  // f0 - f7 
            PkgInt.pack4bits(2,2,2,2,2,2,2,0)   // f8 - ff 
        };

        private static int[] euckrStateTable = new int[]
        {
            PkgInt.pack4bits(ERROR,START, 3,ERROR,ERROR,ERROR,ERROR,ERROR),//00-07 
            PkgInt.pack4bits(ITSME,ITSME,ITSME,ITSME,ERROR,ERROR,START,START) //08-0f 
        };

        private static int[] euckrCharLenTable = new int[]
        {
            0, 1, 2, 0
        };
    }
}
