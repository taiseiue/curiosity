using Microsoft.AspNetCore.SignalR;

namespace Curiosity.Hub;

public class CommandHub : Hub<IDirectionClient>, IDirectionHub
{
    public Task SetDirectionAsync(MotorDirection direction)
    {
        Console.WriteLine($"Setting direction to {direction}");
        return Clients.All.ReceiveDirectionAsync(direction);
    }
}
