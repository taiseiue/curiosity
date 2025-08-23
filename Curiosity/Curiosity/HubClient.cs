
using Microsoft.AspNetCore.SignalR.Client;
using TypedSignalR.Client;


namespace Curiosity;

public class HubClient : IDirectionClient, IHubConnectionObserver, IDisposable
{
    public event DirectionReceivedEventHandler? DirectionReceived;
    public HubConnection? Connection { get; private set; }
    public IDirectionHub? Hub { get; set; }
    public static async Task<HubClient> ConnectAsync(string address)
    {
        HubConnection connection = new HubConnectionBuilder().WithUrl(address).Build();
        var hub = connection.CreateHubProxy<IDirectionHub>();
        HubClient client = new()
        {
            Connection = connection,
            Hub = hub
        };
        connection.Register<IDirectionClient>(client);

        await connection.StartAsync();
        return client;
    }
    public async Task SetDirectionAsync(MotorDirection direction)
    {
        await Hub.SetDirectionAsync(direction);
    }
    public void Dispose()
    {
        Connection.DisposeAsync().GetAwaiter().GetResult();
    }

    public Task OnClosed(Exception? exception)
    {
        Connection.Register<IDirectionClient>(this);
        return Task.CompletedTask;
    }

    public Task OnReconnected(string? connectionId)
    {
        // throw new NotImplementedException();
        return Task.CompletedTask;
    }

    public Task OnReconnecting(Exception? exception)
    {
        //throw new NotImplementedException();
        return Task.CompletedTask;
    }

    public Task ReceiveDirectionAsync(MotorDirection direction)
    {
        DirectionReceived?.Invoke(this, direction);
        return Task.CompletedTask;
    }
}
public delegate void DirectionReceivedEventHandler(HubClient sender, MotorDirection direction);