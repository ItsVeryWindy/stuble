using Microsoft.AspNetCore.SignalR;
using Stuble.Client;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Stuble.Server
{
    public class StubleHub : Hub<IStubleClient>
    {
        private readonly IEnumerable<IConnect> _connects;

        public StubleHub(IEnumerable<IConnect> connects)
        {
            _connects = connects;
        }

        public override async Task OnConnectedAsync()
        {
            foreach(var connect in _connects)
            {
                await connect.ConnectAsync(Clients.Caller);
            }

            await base.OnConnectedAsync();
        }
    }
}
