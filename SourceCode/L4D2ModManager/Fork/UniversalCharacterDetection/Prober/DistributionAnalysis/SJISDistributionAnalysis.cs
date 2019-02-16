using System;
using System.Collections.Generic;
using System.Text;

namespace Mozilla.UniversalCharacterDetection.Prober.DistributionAnalysis
{
    public class SJISDistributionAnalysis : JISDistributionAnalysis
    {
        public static int HIGHBYTE_BEGIN_1 = 0x81;
        public static int HIGHBYTE_END_1 = 0x9F;
        public static int HIGHBYTE_BEGIN_2 = 0xE0;
        public static int HIGHBYTE_END_2 = 0xEF;
        public static int LOWBYTE_BEGIN_1 = 0x40;
        public static int LOWBYTE_BEGIN_2 = 0x80;

        public SJISDistributionAnalysis() : base()
        {
        }

        protected override int getOrder(byte[] buf, int offset)
        {
            int order = -1;

            int highbyte = buf[offset] & 0xFF;
            if (highbyte >= HIGHBYTE_BEGIN_1 && highbyte <= HIGHBYTE_END_1)
            {
                order = 188 * (highbyte - HIGHBYTE_BEGIN_1);
            }
            else if (highbyte >= HIGHBYTE_BEGIN_2 && highbyte <= HIGHBYTE_END_2)
            {
                order = 188 * (highbyte - HIGHBYTE_BEGIN_2 + 31);
            }
            else
            {
                return -1;
            }
            int lowbyte = buf[offset + 1] & 0xFF;
            order += lowbyte - LOWBYTE_BEGIN_1;
            if (lowbyte >= LOWBYTE_BEGIN_2)
            {
                --order;
            }

            return order;
        }
    }
}
