namespace Collector.Common.Validation.AccountNumber.Validators
{
    internal static class Mod11
    {
        internal const int ID = 11;

        internal static bool Validate(string number)
        {
            var sum = 0;
            var len = number.Length;
            var weights = new[] {1, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1};
            var next = weights.Length;

            while (len > 0)
            {
                var digit = (int) char.GetNumericValue(number[--len]);
                sum += weights[--next] * digit;
            }

            return sum % 11 == 0;
        }
    }
}