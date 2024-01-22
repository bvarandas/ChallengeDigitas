using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace ChallengeCrf.Api.Hubs;

public class BrokerHub : Hub
{
    public Task ConnectToMessageBroker()
    {
        Groups.AddToGroupAsync(Context.ConnectionId, "CrudMessage");

        return Task.CompletedTask;
    }

    public async Task SendMessage(string user, string message)
    {
        await Clients.Caller.SendAsync("ReceiveMessage", user, message);
    }
}
