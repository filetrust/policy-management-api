using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Glasswall.PolicyManagement.Common.Configuration.Validation.Errors;

namespace Glasswall.PolicyManagement.Business.Configuration
{
    [Serializable]
    [ExcludeFromCodeCoverage]
    public class ConfigurationBindException : Exception
    {
        public ConfigurationBindException()
        {
            
        }

        public ConfigurationBindException(IEnumerable<ConfigurationParserError> errors)
            : base("Error binding configuration: " + string.Join(Environment.NewLine, errors.Select(error => $"{error.ParamName} - {error.Reason}")))
        {
        }
    }
}