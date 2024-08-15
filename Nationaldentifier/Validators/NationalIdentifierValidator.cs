using System;
using System.Collections.Generic;
using Collector.Common.Validation.NationalIdentifier.Interface;

namespace Collector.Common.Validation.NationalIdentifier.Validators
{
    public abstract class NationalIdentifierValidator : INationalIdentifierValidator
    {
        private static readonly Dictionary<CountryCode, NationalIdentifierValidator>
            SignMethodToValidatorMapping = new Dictionary<CountryCode, NationalIdentifierValidator>
            {
                {CountryCode.SE, new SwedishNationalIdentifierValidator()},
                {CountryCode.NO, new NorwegianNationalIdentifierValidator()},
                {CountryCode.FI, new FinnishNationalIdentifierValidator()},
                {CountryCode.DK, new DanishNationalIdentifierValidator()}
            };

        public abstract CountryCode CountryCode { get; }

        public static INationalIdentifierValidator GetValidator(CountryCode countryCode)
        {
            if (SignMethodToValidatorMapping.ContainsKey(countryCode))
                return SignMethodToValidatorMapping[countryCode];
            throw new ArgumentOutOfRangeException(nameof(countryCode), countryCode,
                "There is no validation implemented for the given country.");
        }

        public static bool IsValidInAnyCountry(string nationalIdentifier)
        {
            foreach (var validator in SignMethodToValidatorMapping)
            {
                if (validator.Value.IsValid(nationalIdentifier))
                {
                    return true;
                }
            }

            return false;
        }

        public static string NormalizeForAnyCountry(string nationalIdentifier)
        {
            foreach (var validator in SignMethodToValidatorMapping)
            {
                if (validator.Value.IsValid(nationalIdentifier))
                {
                    return validator.Value.Normalize(nationalIdentifier);
                }
            }

            throw new ArgumentException(ErrorMessages.GetInvalidIdentifierMessage(nationalIdentifier), nameof(nationalIdentifier));
        }

        public abstract bool IsValid(string nationalIdentifier);

        public abstract string Normalize(string nationalIdentifier);

        internal static bool IsValidDate(int year, int month, int day)
        {
            if (year <= 0 || year > 9999)
            {
                return false;
            }

            var maxDaysInMonth = new[] {31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31};
            if (DateTime.IsLeapYear(year))
                maxDaysInMonth[1] = 29;

            return month > 0 && month < 13 && day > 0 && day <= maxDaysInMonth[month - 1];
        }

        internal static long GetDigitAtPosition(long input, int position)
        {
            var divisor = (long) Math.Pow(10, position);
            var lowestDigitContainsDigitWeWant = input / divisor;
            return lowestDigitContainsDigitWeWant % 10; // Take the remainder of 10 to extract the digit we want
        }
    }
}