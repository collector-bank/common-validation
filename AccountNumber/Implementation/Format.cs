using System;

namespace Collector.Common.Validation.AccountNumber.Implementation
{
    internal static class Format
    {
        internal static string ClearingNumber(string clearingNumber)
        {
            if (clearingNumber == null)
            {
                throw new ArgumentNullException(nameof(clearingNumber), "Clearing number must have a value");
            }

            var length = clearingNumber.Length;
            if (length == 4)
            {
                return clearingNumber;
            }
            else if (length == 5)
            {
                return clearingNumber.Insert(3, "-");
            }

            throw new ArgumentOutOfRangeException(nameof(clearingNumber), "Clearing number must be four or five digits");
        }
    }
}
