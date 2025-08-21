
using Microsoft.AspNetCore.SignalR.Client;
using TypedSignalR.Client;


namespace Curiosity;

public class HubClient : ICommandClient, IHubConnectionObserver, IDisposable
{
    public event CommandReceivedEventHandler? CommandReceived;
    public HubConnection? Connection { get; private set; }
    public static async Task<HubClient> ConnectAsync(string address)
    {
        HubConnection connection = new HubConnectionBuilder().WithUrl(address).Build();
        var hub = connection.CreateHubProxy<ICommandHub>();

        HubClient client = new()
        {
            Connection = connection
        };
        connection.Register<ICommandClient>(client);

        await connection.StartAsync();
        return client;
    }

    public void Dispose()
    {
        Connection.DisposeAsync().GetAwaiter().GetResult();
    }

    public Task OnClosed(Exception? exception)
    {
        throw new NotImplementedException();
    }

    public Task OnReconnected(string? connectionId)
    {
        throw new NotImplementedException();
    }

    public Task OnReconnecting(Exception? exception)
    {
        throw new NotImplementedException();
    }

    public Task ReceiveCommandAsync(Command command)
    {
        CommandReceived?.Invoke(this, command);
        return Task.CompletedTask;
    }
}
public delegate void CommandReceivedEventHandler(HubClient sender, Command cmd);