using System.Collections.Generic;
using Glasswall.PolicyManagement.Common.Configuration.Validation.Errors;

namespace Glasswall.PolicyManagement.Common.Configuration.Validation
{
    public interface IConfigurationItemValidator
    {
        bool TryParse(string key, string rawValue, List<ConfigurationParserError> validationErrors, out object parsed);
    }
}