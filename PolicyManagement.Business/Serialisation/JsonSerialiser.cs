using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Glasswall.PolicyManagement.Common.Serialisation;

namespace Glasswall.PolicyManagement.Business.Serialisation
{
    public class JsonSerialiser : IJsonSerialiser
    {
        public Task<TObject> Deserialize<TObject>(MemoryStream input, CancellationToken ct)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));
            return Task.FromResult(Newtonsoft.Json.JsonConvert.DeserializeObject<TObject>(Encoding.UTF8.GetString(input.ToArray())));
        }

        public Task<string> Serialize<TObject>(TObject input, CancellationToken cancellationToken)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));
            return Task.FromResult(Newtonsoft.Json.JsonConvert.SerializeObject(input));
        }
    }
}