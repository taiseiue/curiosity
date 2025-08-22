namespace Curiosity;

public interface ICommandClient
{
    Task ReceiveDirectionAsync(MotorDirection direction);
}