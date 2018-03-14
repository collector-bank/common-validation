namespace Collector.Common.Validation.AccountNumber.Validators
{
    internal static class Mod10
    {
        internal const int ID = 10;

        internal static bool Validate(string number)
        {
            const int ON = 1;

            var sum = 0;
            var toggle = 1;
            var len = number.Length;
            var products = new[] {0, 2, 4, 6, 8, 1, 3, 5, 7, 9};

            while (len > 0)
            {
                var digit = (int) char.GetNumericValue(number[--len]);
                sum += (toggle ^= 1) == ON ? products[digit] : digit;
            }

            return sum % 10 == 0;
        }
    }
}