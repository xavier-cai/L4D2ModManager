using System;
using System.Collections.Generic;
using System.Text;

namespace Mozilla.UniversalCharacterDetection.Prober.StateMachine
{
    public class UCS2BESMModel : SMModel
    {
        public static int UCS2BE_CLASS_FACTOR = 6;

        public UCS2BESMModel() : base(new PkgInt(PkgInt.INDEX_SHIFT_4BITS, PkgInt.SHIFT_MASK_4BITS, PkgInt.BIT_SHIFT_4BITS, PkgInt.UNIT_MASK_4BITS, ucs2beClassTable), UCS2BE_CLASS_FACTOR, new PkgInt(PkgInt.INDEX_SHIFT_4BITS, PkgInt.SHIFT_MASK_4BITS, PkgInt.BIT_SHIFT_4BITS, PkgInt.UNIT_MASK_4BITS, ucs2beStateTable), ucs2beCharLenTable, Constants.CHARSET_UTF_16BE)
        {

        }

        private static int[] ucs2beClassTable = new int[]
        {
            PkgInt.pack4bits(0,0,0,0,0,0,0,0),  // 00 - 07 
            PkgInt.pack4bits(0,0,1,0,0,2,0,0),  // 08 - 0f 
            PkgInt.pack4bits(0,0,0,0,0,0,0,0),  // 10 - 17 
            PkgInt.pack4bits(0,0,0,3,0,0,0,0),  // 18 - 1f 
            PkgInt.pack4bits(0,0,0,0,0,0,0,0),  // 20 - 27 
            PkgInt.pack4bits(0,3,3,3,3,3,0,0),  // 28 - 2f 
            PkgInt.pack4bits(0,0,0,0,0,0,0,0),  // 30 - 37 
            PkgInt.pack4bits(0,0,0,0,0,0,0,0),  // 38 - 3f 
            PkgInt.pack4bits(0,0,0,0,0,0,0,0),  // 40 - 47 
            PkgInt.pack4bits(0,0,0,0,0,0,0,0),  // 48 - 4f 
            PkgInt.pack4bits(0,0,0,0,0,0,0,0),  // 50 - 57 
            PkgInt.pack4bits(0,0,0,0,0,0,0,0),  // 58 - 5f 
            PkgInt.pack4bits(0,0,0,0,0,0,0,0),  // 60 - 67 
            PkgInt.pack4bits(0,0,0,0,0,0,0,0),  // 68 - 6f 
            PkgInt.pack4bits(0,0,0,0,0,0,0,0),  // 70 - 77 
            PkgInt.pack4bits(0,0,0,0,0,0,0,0),  // 78 - 7f 
            PkgInt.pack4bits(0,0,0,0,0,0,0,0),  // 80 - 87 
            PkgInt.pack4bits(0,0,0,0,0,0,0,0),  // 88 - 8f 
            PkgInt.pack4bits(0,0,0,0,0,0,0,0),  // 90 - 97 
            PkgInt.pack4bits(0,0,0,0,0,0,0,0),  // 98 - 9f 
            PkgInt.pack4bits(0,0,0,0,0,0,0,0),  // a0 - a7 
            PkgInt.pack4bits(0,0,0,0,0,0,0,0),  // a8 - af 
            PkgInt.pack4bits(0,0,0,0,0,0,0,0),  // b0 - b7 
            PkgInt.pack4bits(0,0,0,0,0,0,0,0),  // b8 - bf 
            PkgInt.pack4bits(0,0,0,0,0,0,0,0),  // c0 - c7 
            PkgInt.pack4bits(0,0,0,0,0,0,0,0),  // c8 - cf 
            PkgInt.pack4bits(0,0,0,0,0,0,0,0),  // d0 - d7 
            PkgInt.pack4bits(0,0,0,0,0,0,0,0),  // d8 - df 
            PkgInt.pack4bits(0,0,0,0,0,0,0,0),  // e0 - e7 
            PkgInt.pack4bits(0,0,0,0,0,0,0,0),  // e8 - ef 
            PkgInt.pack4bits(0,0,0,0,0,0,0,0),  // f0 - f7 
            PkgInt.pack4bits(0,0,0,0,0,0,4,5)   // f8 - ff 
        };

        private static int[] ucs2beStateTable = new int[]
        {
            PkgInt.pack4bits(5, 7, 7,ERROR, 4, 3,ERROR,ERROR),//00-07 
            PkgInt.pack4bits(ERROR,ERROR,ERROR,ERROR,ITSME,ITSME,ITSME,ITSME),//08-0f 
            PkgInt.pack4bits(ITSME,ITSME, 6, 6, 6, 6,ERROR,ERROR),//10-17 
            PkgInt.pack4bits(6, 6, 6, 6, 6,ITSME, 6, 6),//18-1f 
            PkgInt.pack4bits(6, 6, 6, 6, 5, 7, 7,ERROR),//20-27 
            PkgInt.pack4bits(5, 8, 6, 6,ERROR, 6, 6, 6),//28-2f 
            PkgInt.pack4bits(6, 6, 6, 6,ERROR,ERROR,START,START) //30-37 
        };

        private static int[] ucs2beCharLenTable = new int[]
        {
            2, 2, 2, 0, 2, 2
        };
    }
}
