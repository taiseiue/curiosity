namespace Curiosity;

public interface IDirectionHub
{
    Task SetDirectionAsync(MotorDirection direction);
    Task SetDirectionAsync(byte direction);
}
