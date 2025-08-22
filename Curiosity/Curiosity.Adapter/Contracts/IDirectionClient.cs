namespace Curiosity;

public interface IDirectionClient
{
    Task ReceiveDirectionAsync(MotorDirection direction);
}