using Microsoft.AspNetCore.SignalR;

namespace Curiosity.Hub;

public class CommandHub : Hub<ICommandClient>, ICommandHub
{
    public Task SendCommandAsync(Command command)
    {
        return Clients.All.ReceiveCommandAsync(command);
    }
}
