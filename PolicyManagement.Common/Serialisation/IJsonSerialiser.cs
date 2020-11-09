using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Glasswall.PolicyManagement.Common.Serialisation
{
    public interface IJsonSerialiser
    {
        Task<TObject> Deserialize<TObject>(MemoryStream input, CancellationToken ct);
        Task<string> Serialize<TObject>(TObject input, CancellationToken cancellationToken);
    }
}