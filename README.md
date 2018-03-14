# collector-common-validation

A collection of string validation tools available as Nuget packages. Currently includes the following validation packages:

| Validation subject | Countries | Nuget package name |
| ------------------ | --------- | ------------------ |
| National Identifiers (Social Security Numbers) | Denmark, Finland, Norway, Sweden | Collector.Common.Validation.NationalIdentifier |
| Bank Account Numbers | Sweden | Collector.Common.Validation.AccountNumber

## Installing

Download the package of choice from Nuget:

```Powershell
PM> install-package Collector.Common.Validation.NationalIdentifier
```

## Usage

### Account Number

Identify which bank an account number belongs to.

```c#
var number = "8123-57654321";
var validator = new SwedishAccountNumberValidator(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "banks.se.json"));
var result = validator.Identify(number);

// Alternately
if (validator.TryIdentify(number, out var result2))
{
   // success
}
```

Validate account number. 
OBS! Account number validation is a work in progress. It is by no means production ready.

```c#
var number = "8123-57654321";
var validator = new SwedishAccountNumberValidator(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "banks.se.json"));
var result = validator.Validate(number);

// Alternately
if (validator.TryValidate(number, out var result2))
{
   // success
}
```

### National Identifiers

Validate a number for a specific country (returns a bool):

```c#
var nationalIdentifier = "200301046835";
var validator = new SwedishNationalIdentifierValidator();
var result = validator.IsValid(nationalIdentifier);

// true
```

or 

```c#
var nationalIdentifier = "200301046835";
var validator = NationalIdentifierValidator.GetValidator(CountryCode.SE);
var result = validator.IsValid(nationalIdentifier);

// true
```

Normalize national identifiers to a single format:

```c#
var nationalIdentifier = "20030104-6835";
var validator = new SwedishIdentifierValidator();
var result = validator.Normalize(nationalIdentifier);

// "200301046835"
```

Check for validity or normalize an identifier for any of the supported countries:

```c#
var result = NationalIdentifierValidator.IsValidInAnyCountry(nationalIdentifier);
var normalizedIdentifier = NationalIdentifierValidator.NormalizeForAnyCountry(nationalIdentifier);
```

Validate a property with data annotations:

```c#
[NationalIdentifier]
public string NationalIdentifier { get; set; }

[NationalIdentifier(CountryCode.SE)]
public string NationalIdentifier { get; set; }
```

## Contributing

Extensions are very welcome! Get in touch with the Solutions Team and open a pull request.

## Acknowledgments

* Thanks to Said Outgajjouft for the initial work on National Identifiers.
* Account number validation is based on kontonummer.js by Jonas Persson (https://jop.io/)
