using System;
using System.Collections.Generic;
using System.Text;

namespace Mozilla.UniversalCharacterDetection.Prober.StateMachine
{
    public class GB18030SMModel : SMModel
    {
        public static int GB18030_CLASS_FACTOR = 7;

        public GB18030SMModel() : base(new PkgInt(PkgInt.INDEX_SHIFT_4BITS, PkgInt.SHIFT_MASK_4BITS, PkgInt.BIT_SHIFT_4BITS, PkgInt.UNIT_MASK_4BITS, gb18030ClassTable), GB18030_CLASS_FACTOR, new PkgInt(PkgInt.INDEX_SHIFT_4BITS, PkgInt.SHIFT_MASK_4BITS, PkgInt.BIT_SHIFT_4BITS, PkgInt.UNIT_MASK_4BITS, gb18030StateTable), gb18030CharLenTable, Constants.CHARSET_GB18030)
        {

        }

        private static int[] gb18030ClassTable = new int[]
        {
            PkgInt.pack4bits(1,1,1,1,1,1,1,1),  // 00 - 07 
            PkgInt.pack4bits(1,1,1,1,1,1,0,0),  // 08 - 0f 
            PkgInt.pack4bits(1,1,1,1,1,1,1,1),  // 10 - 17 
            PkgInt.pack4bits(1,1,1,0,1,1,1,1),  // 18 - 1f 
            PkgInt.pack4bits(1,1,1,1,1,1,1,1),  // 20 - 27 
            PkgInt.pack4bits(1,1,1,1,1,1,1,1),  // 28 - 2f 
            PkgInt.pack4bits(3,3,3,3,3,3,3,3),  // 30 - 37 
            PkgInt.pack4bits(3,3,1,1,1,1,1,1),  // 38 - 3f 
            PkgInt.pack4bits(2,2,2,2,2,2,2,2),  // 40 - 47 
            PkgInt.pack4bits(2,2,2,2,2,2,2,2),  // 48 - 4f 
            PkgInt.pack4bits(2,2,2,2,2,2,2,2),  // 50 - 57 
            PkgInt.pack4bits(2,2,2,2,2,2,2,2),  // 58 - 5f 
            PkgInt.pack4bits(2,2,2,2,2,2,2,2),  // 60 - 67 
            PkgInt.pack4bits(2,2,2,2,2,2,2,2),  // 68 - 6f 
            PkgInt.pack4bits(2,2,2,2,2,2,2,2),  // 70 - 77 
            PkgInt.pack4bits(2,2,2,2,2,2,2,4),  // 78 - 7f 
            PkgInt.pack4bits(5,6,6,6,6,6,6,6),  // 80 - 87 
            PkgInt.pack4bits(6,6,6,6,6,6,6,6),  // 88 - 8f 
            PkgInt.pack4bits(6,6,6,6,6,6,6,6),  // 90 - 97 
            PkgInt.pack4bits(6,6,6,6,6,6,6,6),  // 98 - 9f 
            PkgInt.pack4bits(6,6,6,6,6,6,6,6),  // a0 - a7 
            PkgInt.pack4bits(6,6,6,6,6,6,6,6),  // a8 - af 
            PkgInt.pack4bits(6,6,6,6,6,6,6,6),  // b0 - b7 
            PkgInt.pack4bits(6,6,6,6,6,6,6,6),  // b8 - bf 
            PkgInt.pack4bits(6,6,6,6,6,6,6,6),  // c0 - c7 
            PkgInt.pack4bits(6,6,6,6,6,6,6,6),  // c8 - cf 
            PkgInt.pack4bits(6,6,6,6,6,6,6,6),  // d0 - d7 
            PkgInt.pack4bits(6,6,6,6,6,6,6,6),  // d8 - df 
            PkgInt.pack4bits(6,6,6,6,6,6,6,6),  // e0 - e7 
            PkgInt.pack4bits(6,6,6,6,6,6,6,6),  // e8 - ef 
            PkgInt.pack4bits(6,6,6,6,6,6,6,6),  // f0 - f7 
            PkgInt.pack4bits(6,6,6,6,6,6,6,0)   // f8 - ff 
        };

        private static int[] gb18030StateTable = new int[]
        {
            PkgInt.pack4bits(ERROR,START,START,START,START,START, 3,ERROR),//00-07 
            PkgInt.pack4bits(ERROR,ERROR,ERROR,ERROR,ERROR,ERROR,ITSME,ITSME),//08-0f 
            PkgInt.pack4bits(ITSME,ITSME,ITSME,ITSME,ITSME,ERROR,ERROR,START),//10-17 
            PkgInt.pack4bits(4,ERROR,START,START,ERROR,ERROR,ERROR,ERROR),//18-1f 
            PkgInt.pack4bits(ERROR,ERROR, 5,ERROR,ERROR,ERROR,ITSME,ERROR),//20-27 
            PkgInt.pack4bits(ERROR,ERROR,START,START,START,START,START,START) //28-2f 
        };

        private static int[] gb18030CharLenTable = new int[]
        {
            0, 1, 1, 1, 1, 1, 2
        };
    }
}
