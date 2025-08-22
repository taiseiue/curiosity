using Microsoft.AspNetCore.SignalR;

namespace Curiosity.Hub;

public class CommandHub : Hub<IDirectionClient>, IDirectionHub
{
    public Task SetDirectionAsync(MotorDirection direction)
    {
        Console.WriteLine($"Setting direction to {direction}");
        return Clients.All.ReceiveDirectionAsync(direction);
    }
    public Task SetDirectionAsync(byte direction)
    {
        Console.WriteLine($"Setting direction to {(MotorDirection)direction}");
        return SetDirectionAsync((MotorDirection)direction);
    }
}
