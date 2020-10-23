using System;
using System.Collections.Generic;
using System.Linq;
using Glasswall.PolicyManagement.Common.Configuration.Validation.Errors;

namespace Glasswlal.PolicyManagement.Business.Configuration
{
    [Serializable]
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