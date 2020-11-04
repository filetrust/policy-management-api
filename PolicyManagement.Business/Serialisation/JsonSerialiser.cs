using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Glasswall.PolicyManagement.Common.Serialisation;
using Glasswlal.PolicyManagement.Business.Services;

namespace Glasswlal.PolicyManagement.Business.Serialisation
{
    public class JsonSerialiser : IJsonSerialiser
    {
        public Task<TObject> Deserialize<TObject>(MemoryStream ms, CancellationToken ct)
        {
            return Task.FromResult(Newtonsoft.Json.JsonConvert.DeserializeObject<TObject>(Encoding.UTF8.GetString(ms.ToArray())));
        }

        public Task<string> Serialise<TObject>(TObject model, CancellationToken token)
        {
            return Task.FromResult(Newtonsoft.Json.JsonConvert.SerializeObject(model));
        }
    }
}