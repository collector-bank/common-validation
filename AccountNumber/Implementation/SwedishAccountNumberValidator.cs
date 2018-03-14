using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Collector.Common.Validation.AccountNumber.Models;
using Newtonsoft.Json;
using Collector.Common.Validation.AccountNumber.Interface;
using Collector.Common.Validation.AccountNumber.Validators;

namespace Collector.Common.Validation.AccountNumber.Implementation
{
    public class SwedishAccountNumberValidator : IAccountNumberValidator
    {
        private const char CHAR_ZERO = '0';
        private const string NO_DIGITS_PATTERN = @"[^\d]";

        private readonly AccountValidationModel[] _banks;

        public SwedishAccountNumberValidator(string path)
        {
            _banks = JsonConvert.DeserializeObject<AccountValidationModel[]>(File.ReadAllText(path));
        }
        
        public BankAccountModel Identify(string number)
        {
            try
            {
                return IdentifyInternal(number);
            }
            catch (ArgumentException ex)
            {
                throw new ArgumentException("Bank could not be identified", nameof(number), ex);
            }
            catch (InvalidOperationException)
            {
                throw new ArgumentException("Bank could not be identified", nameof(number));
            }
        }

        public bool TryIdentify(string number, out BankAccountModel result)
        {
            try
            {
                result = Identify(number);
            }
            catch (ArgumentException)
            {
                result = null;

                return false;
            }

            return true;
        }

        public bool TryValidate(string number, out BankAccountModel result)
        {
            try
            {
                result = Validate(number);
            }
            catch (ArgumentException)
            {
                result = null;

                return false;
            }

            return true;
        }

        public BankAccountModel Validate(string number)
        {
            try
            {
                return ValidateInternal(number);
            }
            catch (ArgumentException ex)
            {
                throw new ArgumentException("Account number could not be validated", nameof(number), ex);
            }
            catch (InvalidOperationException)
            {
                throw new ArgumentException("Account number could not be validated", nameof(number));
            }
        }

        private BankAccountModel IdentifyInternal(string number)
        {
            if (number == null)
            {
                throw new ArgumentNullException(nameof(number), "Number must have a value");
            }

            var number2 = Regex.Replace(number, NO_DIGITS_PATTERN, string.Empty);
            if (string.IsNullOrEmpty(number2))
            {
                throw new ArgumentException("Number must contain digits", nameof(number));
            }

            // Get validation configurations matching clearing number in specified bank account number
            var length = number2.Length;
            var configs = _banks
                .Where(c =>
                    length >= c.Clearing &&
                    Regex.IsMatch(number2.Substring(0, c.Clearing), c.ClearingRegex)
                );

            // Allow multiple configurations, but only one bank name
            var name = configs.Select(c => c.Name).Distinct().Single();

            // Select first available config
            var config = configs.First();

            var accountNumber = string.Empty;
            if (length <= (config.Clearing + config.Account))
            {
                accountNumber = number2.Substring(config.Clearing);

                // Pad start of account number with zeroes to correct length
                if (accountNumber.Length < config.Account)
                {
                    accountNumber = accountNumber.PadLeft(config.Account, CHAR_ZERO);
                }
            }

            return new BankAccountModel(
                config.Name,
                Format.ClearingNumber(number2.Substring(0, config.Clearing)), 
                accountNumber
            );
        }

        /// <summary>
        /// Work in progress. Only tested with a small subset of Swedish account numbers.
        /// </summary>
        private BankAccountModel ValidateInternal(string number)
        {
            if (number == null)
            {
                throw new ArgumentNullException(nameof(number), "Number must have a value");
            }

            var number2 = Regex.Replace(number, NO_DIGITS_PATTERN, string.Empty);
            if (string.IsNullOrEmpty(number2))
            {
                throw new ArgumentException("Number must contain digits", nameof(number));
            }

            // Get validation configurations matching clearing number in specified bank account number
            var length = number2.Length;
            var configs = _banks
                .Where(c =>
                    length >= c.Clearing &&
                    length <= (c.Clearing + c.Account) &&
                    Regex.IsMatch(number2.Substring(0, c.Clearing), c.ClearingRegex)
                );

            foreach (var config in configs)
            {
                var clearingNumber = number2.Substring(0, config.Clearing);
                var accountNumber = number2.Substring(config.Clearing);

                // Pad start of account number with zeroes to correct length
                if (accountNumber.Length < config.Account)
                {
                    accountNumber = accountNumber.PadLeft(config.Account, CHAR_ZERO);
                }

                var number3 = clearingNumber + accountNumber;
                if (Regex.IsMatch(number3, config.NumberRegex))
                {
                    // Extract account number digits based on validation config
                    var control = number3.Substring(number3.Length - config.Control, config.Control);

                    // Validate number according to configuration
                    if (config.Modulo == Mod10.ID && Mod10.Validate(control) ||
                        config.Modulo == Mod11.ID && Mod11.Validate(control))
                        return new BankAccountModel(
                            config.Name,
                            Format.ClearingNumber(clearingNumber),
                            accountNumber
                        );
                }
            }

            throw new ArgumentException("Account number could not be validated");
        }
    }
}