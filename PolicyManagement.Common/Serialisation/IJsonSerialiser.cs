using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Glasswall.PolicyManagement.Common.Serialisation
{
    public interface IJsonSerialiser
    {
        Task<TObject> Deserialize<TObject>(MemoryStream ms, CancellationToken ct);
        Task<string> Serialise<TObject>(TObject model, CancellationToken cancellationToken);
    }
}