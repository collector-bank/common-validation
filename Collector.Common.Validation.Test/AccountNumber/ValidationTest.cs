using System;
using System.Collections.Generic;
using System.IO;
using Collector.Common.Validation.AccountNumber.Interface;
using Collector.Common.Validation.AccountNumber.Implementation;
using NUnit.Framework;

namespace Collector.Common.Validation.Test.AccountNumber
{
    public class ValidationTest
    {
        private IAccountNumberValidator _sut;

        [SetUp]
        public void SetUp()
        {
            _sut = new SwedishAccountNumberValidator(AppDomain.CurrentDomain.BaseDirectory + "banks.se.json");
        }

        [Theory]
        [TestCase("9551", "Avanza Bank")]
        [TestCase("9663", "Amfa Bank")]
        [TestCase("9686", "BlueStep Finans")]
        [TestCase("9472", "BNP")]
        [TestCase("9040", "Citibank")]
        [TestCase("1371", "Danske Bank")]
        [TestCase("2448", "Danske Bank")]
        [TestCase("9189", "Danske Bank")]
        [TestCase("9193", "DnB Bank")]
        [TestCase("9269", "DnB Bank")]
        [TestCase("9708", "Ekobanken")]
        [TestCase("9591", "Erik Penser Bankaktiebolag")]
        [TestCase("9422", "Forex Bank")]
        [TestCase("6142", "Handelsbanken")]
        [TestCase("9275", "ICA Banken")]
        [TestCase("9173", "IKANO Banken")]
        [TestCase("9679", "JAK Medlemsbank")]
        [TestCase("9397", "Landshypotek")]
        [TestCase("9636", "Lån och Spar Bank Sverige")]
        [TestCase("3408", "Länsförsäkringar Bank")]
        [TestCase("9067", "Länsförsäkringar Bank")]
        [TestCase("9021", "Länsförsäkringar Bank")]
        [TestCase("9230", "Marginalen Bank")]
        [TestCase("9641", "Nordax Bank")]
        [TestCase("1112", "Nordea")]
        [TestCase("1526", "Nordea")]
        [TestCase("2073", "Nordea")]
        [TestCase("3299", "Nordea")]
        [TestCase("3301", "Nordea")]
        [TestCase("3376", "Nordea")]
        [TestCase("3410", "Nordea")]
        [TestCase("3951", "Nordea")]
        [TestCase("4727", "Nordea")]
        [TestCase("3300", "Nordea")]
        [TestCase("3782", "Nordea")]
        [TestCase("9531", "Nordea")]
        [TestCase("9961", "Nordea")]
        [TestCase("9108", "Nordnet Bank")]
        [TestCase("9285", "Resurs Bank")]
        [TestCase("9892", "Riksgälden")]
        [TestCase("9099", "Royal Bank of Scotland")]
        [TestCase("9464", "Santander Consumer Bank")]
        [TestCase("9257", "SBAB")]
        [TestCase("5742", "SEB")]
        [TestCase("9123", "SEB")]
        [TestCase("9147", "SEB")]
        [TestCase("9169", "Skandiabanken")]
        [TestCase("9574", "Sparbanken Syd")]
        [TestCase("7384", "Swedbank")]
        [TestCase("9321", "Swedbank")]
        [TestCase("8748", "Swedbank")]
        [TestCase("87384", "Swedbank")]
        [TestCase("2382", "Ålandsbanken")]
        [TestCase("9561", "Avanza Bank")]
        [TestCase("9046", "Citibank")]
        [TestCase("9188", "Danske Bank")]
        [TestCase("6123", "Handelsbanken")]
        [TestCase("1119", "Nordea")]
        [TestCase("2030", "Nordea")]
        [TestCase("3311", "Nordea")]
        [TestCase("4123", "Nordea")]
        [TestCase("5482", "SEB")]
        [TestCase("9137", "SEB")]
        [TestCase("8123", "Swedbank")]
        [TestCase("8123-5", "Swedbank")]
        [TestCase("3407", "Länsförsäkringar Bank")]
        [TestCase("9631", "Lån och Spar Bank Sverige")]
        [TestCase("2300", "Ålandsbanken")]
        public void Identify_Bank_And_Expect_Name(string number, string expectedBankName)
        {
            var result = _sut.Identify(number);

            Assert.That(result.Name, Is.EqualTo(expectedBankName));
        }

        [Theory]
        [TestCase("81231234568", "Swedbank")]
        [TestCase("8123-57654321", "Swedbank")]
        public void Validate_AccountNumber_And_Expect_Exception(string number,
                                                                string expectedBankName)
        {
            Assert.Throws<ArgumentException>(() => _sut.Validate(number));
        }

        [Theory]
        [TestCase("11100000100", "Nordea")]
        [TestCase("33000009100", "Nordea")]
        public void Validate_AccountNumber_and_Expect_BankName(string number,
                                                               string expectedBankName)
        {
            var result = _sut.Validate(number);

            Assert.That(result.Name, Is.EqualTo(expectedBankName));
        }

        [Theory]
        [Explicit]
        [TestCaseSource(nameof(GetNumbers))]
        public void Validate_AccountNumbers_From_Source_Where_Bank_Is_Recognised(string number)
        {
            Assert.DoesNotThrow(() => _sut.Validate(number));
        }

        private static IEnumerable<object> GetNumbers()
        {
            var allRows = File.ReadAllLines(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "numbers.csv"));
            foreach (var line in allRows)
            {
                yield return new object[] {line};
            }
        }
    }
}