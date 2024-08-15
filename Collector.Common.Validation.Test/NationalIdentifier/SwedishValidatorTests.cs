using System;
using System.Collections;
using Collector.Common.Validation.NationalIdentifier;
using Collector.Common.Validation.NationalIdentifier.Validators;
using NUnit.Framework;

namespace Collector.Common.Validation.Test.NationalIdentifier
{
    public class SwedishValidatorTests
    {
        private DateTimeOffset                     _defaultTestTime;
        private SwedishNationalIdentifierValidator _sut;

        [SetUp]
        public void SetUp()
        {
            _sut             = new SwedishNationalIdentifierValidator();
            _defaultTestTime = new DateTimeOffset(new DateTime(2010, 01, 01), TimeSpan.Zero);
        }

        [TestCaseSource(typeof(SwedishIdentifiers), nameof(SwedishIdentifiers.Valid))]
        public void IsValid_NationalIdentifierIsValid_ReturnTrue(string nationalIdentifier)
        {
            SystemTime.TestTime = _defaultTestTime;

            var actual = _sut.IsValid(nationalIdentifier);

            Assert.That(actual, Is.True);
        }

        [TestCaseSource(typeof(SwedishIdentifiers), nameof(SwedishIdentifiers.Invalid))]
        public void IsValid_NationalIdentifierIsInvalid_ReturnFalse(string nationalIdentifier)
        {
            SystemTime.TestTime = _defaultTestTime;

            var actual = _sut.IsValid(nationalIdentifier);

            Assert.That(actual, Is.False);
        }
        
        [TestCaseSource(typeof(SwedishIdentifiers), nameof(SwedishIdentifiers.Normalized))]
        public void Normalize_NationalIdentifierIsValid_ReturnNormalizedValue(
            string nationalIdentifier, string expectedValue)
        {
            SystemTime.TestTime = _defaultTestTime;

            var actual = _sut.Normalize(nationalIdentifier);

            Assert.That(actual, Is.EqualTo(expectedValue));
        }


        [TestCaseSource(typeof(SwedishIdentifiers), nameof(SwedishIdentifiers.Invalid))]
        public void Normalize_NationalIdentifierIsInvalid_ThrowArgumentException(string nationalIdentifier)
        {
            SystemTime.TestTime = _defaultTestTime;

            var ex = Assert.Throws<ArgumentException>(() => _sut.Normalize(nationalIdentifier));
            Assert.That(ex.ParamName, Is.EqualTo("nationalIdentifier"), nameof(ex.ParamName));
            Assert.That(ex.Message,
                Does.Contain(nationalIdentifier ?? string.Empty).And.Contain(_sut.CountryCode.ToString()),
                nameof(ex.Message));
        }

        public class SwedishIdentifiers
        {
            public static IEnumerable Valid
            {
                get
                {
                    yield return "19640101-8780";
                    yield return "1964-01-01-8780";
                    yield return "196401-01-8780";
                    yield return "196401018780";
                    yield return "19800520-2216";
                    yield return "6401018780";
                    yield return "640101-8780";
                    yield return "800520+2216";
                    yield return "320229-4173";   // 1932 has leapyear
                    yield return "19320229-4173"; // 1932 has leapyear
                    yield return "040229-2262";   // 2004 has leapyear
                    yield return "20040229-2262"; // 2004 has leapyear
                    yield return "840161-2729";   // Valid temporary number with 01 days
                    yield return "840191-0313";   // Valid temporary number with 31 days
                    yield return "040289-5049";   // Valid temporary number with leapyear
                }
            }

            public static IEnumerable Invalid
            {
                get
                {
                    yield return null;
                    yield return "xx";          // Invalid
                    yield return "311280=8856"; // Invalid Format
                    yield return "330019-2071"; // Invalid month
                    yield return "331319-2092"; // Invalid month
                    yield return "330149-2074"; // Invalid day
                    yield return "330159-2048"; // Invalid day
                    yield return "330229-7316"; // 1933 does not have leapyear
                    yield return "050229-5009"; // 2005 does not have leapyear
                    yield return "581023-9574"; // Control number not correct
                    yield return "000000000000"; // Valid format. Invalid year, month, day etc.
                }
            }

            public static IEnumerable Normalized
            {
                get
                {
                    yield return new TestCaseData("196401018780", "196401018780");
                    yield return new TestCaseData("64-01-01-8780", "196401018780");
                    yield return new TestCaseData("19800520-2216", "198005202216");
                    yield return new TestCaseData("800520-2216", "198005202216");
                    yield return new TestCaseData("010520-2212", "200105202212");
                    yield return new TestCaseData("030520+2210", "190305202210");
                    yield return new TestCaseData("091230-7808", "200912307808");
                    yield return new TestCaseData("100102-4841", "201001024841");
                    yield return new TestCaseData("100102+4841", "191001024841");
                    yield return new TestCaseData("040229-2262", "200402292262"); // 2004 has leapyear
                    yield return new TestCaseData("840161-2729", "198401612729");
                    // Valid temporary number with 01 days
                }
            }
        }
    }
}