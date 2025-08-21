namespace Curiosity;

public interface ICommandClient
{
    Task ReceiveCommandAsync(Command command);
}