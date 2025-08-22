namespace Curiosity;

public interface ICommandHub
{
    Task SetDirectionAsync(MotorDirection direction);
}
