using System;
using System.Collections.Generic;
using System.Text;

namespace Mozilla.UniversalCharacterDetection.Prober.StateMachine
{
    public class Big5SMModel : SMModel
    {
        public static int BIG5_CLASS_FACTOR = 5;

        public Big5SMModel() : base(new PkgInt(PkgInt.INDEX_SHIFT_4BITS, PkgInt.SHIFT_MASK_4BITS, PkgInt.BIT_SHIFT_4BITS, PkgInt.UNIT_MASK_4BITS, big5ClassTable), BIG5_CLASS_FACTOR, new PkgInt(PkgInt.INDEX_SHIFT_4BITS, PkgInt.SHIFT_MASK_4BITS, PkgInt.BIT_SHIFT_4BITS, PkgInt.UNIT_MASK_4BITS, big5StateTable), big5CharLenTable, Constants.CHARSET_BIG5)
        {

        }

        private static int[] big5ClassTable = new int[]
        {
            // PkgInt.pack4bits(0,1,1,1,1,1,1,1),  // 00 - 07 
            PkgInt.pack4bits(1,1,1,1,1,1,1,1),  // 00 - 07    //allow 0x00 as legal value
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
            PkgInt.pack4bits(4,4,4,4,4,4,4,4),  // 80 - 87 
            PkgInt.pack4bits(4,4,4,4,4,4,4,4),  // 88 - 8f 
            PkgInt.pack4bits(4,4,4,4,4,4,4,4),  // 90 - 97 
            PkgInt.pack4bits(4,4,4,4,4,4,4,4),  // 98 - 9f 
            PkgInt.pack4bits(4,3,3,3,3,3,3,3),  // a0 - a7 
            PkgInt.pack4bits(3,3,3,3,3,3,3,3),  // a8 - af 
            PkgInt.pack4bits(3,3,3,3,3,3,3,3),  // b0 - b7 
            PkgInt.pack4bits(3,3,3,3,3,3,3,3),  // b8 - bf 
            PkgInt.pack4bits(3,3,3,3,3,3,3,3),  // c0 - c7 
            PkgInt.pack4bits(3,3,3,3,3,3,3,3),  // c8 - cf 
            PkgInt.pack4bits(3,3,3,3,3,3,3,3),  // d0 - d7 
            PkgInt.pack4bits(3,3,3,3,3,3,3,3),  // d8 - df 
            PkgInt.pack4bits(3,3,3,3,3,3,3,3),  // e0 - e7 
            PkgInt.pack4bits(3,3,3,3,3,3,3,3),  // e8 - ef 
            PkgInt.pack4bits(3,3,3,3,3,3,3,3),  // f0 - f7 
            PkgInt.pack4bits(3,3,3,3,3,3,3,0)   // f8 - ff 
        };

        private static int[] big5StateTable = new int[]
        {
            PkgInt.pack4bits(ERROR,START,START, 3,ERROR,ERROR,ERROR,ERROR),//00-07 
            PkgInt.pack4bits(ERROR,ERROR,ITSME,ITSME,ITSME,ITSME,ITSME,ERROR),//08-0f 
            PkgInt.pack4bits(ERROR,START,START,START,START,START,START,START) //10-17 
        };

        private static int[] big5CharLenTable = new int[]
        {
            0, 1, 1, 2, 0
        };
    }
}
