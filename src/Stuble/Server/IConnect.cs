using Stuble.Client;
using System.Threading.Tasks;

namespace Stuble.Server
{
    public interface IConnect
    {
        Task ConnectAsync(IStubleClient client);
    }
}
