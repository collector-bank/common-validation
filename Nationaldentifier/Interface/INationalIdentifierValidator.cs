﻿namespace Collector.Common.Validation.NationalIdentifier.Interface
{
    public interface INationalIdentifierValidator
    {
        bool   IsValid(string nationalIdentifier);
        string Normalize(string nationalIdentifier);
    }
}