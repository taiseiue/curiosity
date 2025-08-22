using Microsoft.AspNetCore.SignalR;

namespace Curiosity.Hub;

public class CommandHub : Hub<IDirectionClient>, IDirectionHub
{
    public Task SetDirectionAsync(MotorDirection direction)
    {
        return Clients.All.ReceiveDirectionAsync(direction);
    }
}
