using System;
using System.Collections.Generic;
using System.Text;

namespace Mozilla.UniversalCharacterDetection
{
    public interface ICharsetListener
    {
        void Report(string charset);
    }
}
