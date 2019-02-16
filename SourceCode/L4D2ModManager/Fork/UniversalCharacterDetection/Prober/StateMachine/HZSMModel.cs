using System;
using System.Collections.Generic;
using System.Text;

namespace Mozilla.UniversalCharacterDetection.Prober.StateMachine
{
    public class HZSMModel : SMModel
    {
        public static int HZS_CLASS_FACTOR = 6;

        public HZSMModel() : base(new PkgInt(PkgInt.INDEX_SHIFT_4BITS, PkgInt.SHIFT_MASK_4BITS, PkgInt.BIT_SHIFT_4BITS, PkgInt.UNIT_MASK_4BITS, hzsClassTable), HZS_CLASS_FACTOR, new PkgInt(PkgInt.INDEX_SHIFT_4BITS, PkgInt.SHIFT_MASK_4BITS, PkgInt.BIT_SHIFT_4BITS, PkgInt.UNIT_MASK_4BITS, hzsStateTable), hzsCharLenTable, Constants.CHARSET_HZ_GB_2312)
        {

        }

        private static int[] hzsClassTable = new int[]
        {
            PkgInt.pack4bits(1,0,0,0,0,0,0,0),  // 00 - 07 
            PkgInt.pack4bits(0,0,0,0,0,0,0,0),  // 08 - 0f 
            PkgInt.pack4bits(0,0,0,0,0,0,0,0),  // 10 - 17 
            PkgInt.pack4bits(0,0,0,1,0,0,0,0),  // 18 - 1f 
            PkgInt.pack4bits(0,0,0,0,0,0,0,0),  // 20 - 27 
            PkgInt.pack4bits(0,0,0,0,0,0,0,0),  // 28 - 2f 
            PkgInt.pack4bits(0,0,0,0,0,0,0,0),  // 30 - 37 
            PkgInt.pack4bits(0,0,0,0,0,0,0,0),  // 38 - 3f 
            PkgInt.pack4bits(0,0,0,0,0,0,0,0),  // 40 - 47 
            PkgInt.pack4bits(0,0,0,0,0,0,0,0),  // 48 - 4f 
            PkgInt.pack4bits(0,0,0,0,0,0,0,0),  // 50 - 57 
            PkgInt.pack4bits(0,0,0,0,0,0,0,0),  // 58 - 5f 
            PkgInt.pack4bits(0,0,0,0,0,0,0,0),  // 60 - 67 
            PkgInt.pack4bits(0,0,0,0,0,0,0,0),  // 68 - 6f 
            PkgInt.pack4bits(0,0,0,0,0,0,0,0),  // 70 - 77 
            PkgInt.pack4bits(0,0,0,4,0,5,2,0),  // 78 - 7f 
            PkgInt.pack4bits(1,1,1,1,1,1,1,1),  // 80 - 87 
            PkgInt.pack4bits(1,1,1,1,1,1,1,1),  // 88 - 8f 
            PkgInt.pack4bits(1,1,1,1,1,1,1,1),  // 90 - 97 
            PkgInt.pack4bits(1,1,1,1,1,1,1,1),  // 98 - 9f 
            PkgInt.pack4bits(1,1,1,1,1,1,1,1),  // a0 - a7 
            PkgInt.pack4bits(1,1,1,1,1,1,1,1),  // a8 - af 
            PkgInt.pack4bits(1,1,1,1,1,1,1,1),  // b0 - b7 
            PkgInt.pack4bits(1,1,1,1,1,1,1,1),  // b8 - bf 
            PkgInt.pack4bits(1,1,1,1,1,1,1,1),  // c0 - c7 
            PkgInt.pack4bits(1,1,1,1,1,1,1,1),  // c8 - cf 
            PkgInt.pack4bits(1,1,1,1,1,1,1,1),  // d0 - d7 
            PkgInt.pack4bits(1,1,1,1,1,1,1,1),  // d8 - df 
            PkgInt.pack4bits(1,1,1,1,1,1,1,1),  // e0 - e7 
            PkgInt.pack4bits(1,1,1,1,1,1,1,1),  // e8 - ef 
            PkgInt.pack4bits(1,1,1,1,1,1,1,1),  // f0 - f7 
            PkgInt.pack4bits(1,1,1,1,1,1,1,1)   // f8 - ff 
        };

        private static int[] hzsStateTable = new int[]
        {
            PkgInt.pack4bits(START,ERROR, 3,START,START,START,ERROR,ERROR),//00-07 
            PkgInt.pack4bits(ERROR,ERROR,ERROR,ERROR,ITSME,ITSME,ITSME,ITSME),//08-0f 
            PkgInt.pack4bits(ITSME,ITSME,ERROR,ERROR,START,START, 4,ERROR),//10-17 
            PkgInt.pack4bits(5,ERROR, 6,ERROR, 5, 5, 4,ERROR),//18-1f 
            PkgInt.pack4bits(4,ERROR, 4, 4, 4,ERROR, 4,ERROR),//20-27 
            PkgInt.pack4bits(4,ITSME,START,START,START,START,START,START) //28-2f 
        };

        private static int[] hzsCharLenTable = new int[]
        {
            0, 0, 0, 0, 0, 0
        };
    }
}
