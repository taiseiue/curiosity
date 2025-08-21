namespace Curiosity;

public interface ICommandHub
{
    Task SendCommandAsync(Command command);
}
