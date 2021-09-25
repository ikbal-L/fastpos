using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastPosFrontend.Helpers
{
    public class NumerationMask
    {
        static readonly char[] ROUNDS;
        static NumerationMask()
        {
            ROUNDS = Enumerable.Range('A', 26).Select(num=> (char) num).ToArray();
        }

        public static string Mask(int number, string format,int roundCap = 100)
        {
            var round = number / roundCap;
            var roundChar = ROUNDS.ElementAt(round);
            var maskedValue = number % roundCap;
            var formatedMaskedValue = maskedValue.ToString(format);
            return $"{roundChar}-{formatedMaskedValue}";
        }
    }
}
