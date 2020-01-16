using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;

namespace Lab4
{
    static class GenFibonachi
    {
        public static BigInteger Generate(int number)
        {
            return generate(number);
        }

        private static BigInteger generate(BigInteger number)
        {
            if (number < 2) return 1;

            return generate(number - 1) + generate(number - 2);
        }
    }
}