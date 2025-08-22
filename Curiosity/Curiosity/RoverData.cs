namespace Curiosity;

public class RoverData
{
    /// <summary>
    /// ローバーのID
    /// </summary>
    public string Id { get; set; }
    public int Voltage { get; set; }
    public int BatteryLevel { get; set; }
    public bool IsCharging { get; set; }
    public int Uptime { get; set; }
    public float Temperature { get; set; }
    public float Pressure { get; set; }
    public float Humidity { get; set; }
    public float Distance { get; set; }
    public bool IsCatching { get; set; }
    public byte LedBrightness { get; set; }
    public byte Motor { get; set; }
    public MotorDirection Direction
    {
        get
        {
            return (MotorDirection)Motor;
        }
        set
        {
            Motor = (byte)value;
        }
    }

    public MotorState MotorFrontLeft
    {
        get
        {
            byte mask = 0b10000000;
            return GetMotorState(mask);
        }
        set
        {
            Motor = SetBitWithBool(Motor, 7, value != MotorState.Off);
            Motor = SetBitWithBool(Motor, 6, value == MotorState.Reverse);
        }
    }
    public MotorState MotorBackLeft
    {
        get
        {
            byte mask = 0b00100000;
            return GetMotorState(mask);
        }
        set
        {
            Motor = SetBitWithBool(Motor, 5, value != MotorState.Off);
            Motor = SetBitWithBool(Motor, 4, value == MotorState.Reverse);
        }
    }

    public MotorState MotorFrontRight
    {
        get
        {
            byte mask = 0b00001000;
            return GetMotorState(mask);
        }
        set
        {
            Motor = SetBitWithBool(Motor, 3, value != MotorState.Off);
            Motor = SetBitWithBool(Motor, 2, value == MotorState.Reverse);
        }
    }
    public MotorState MotorBackRight
    {
        get
        {
            byte mask = 0b00000010;
            return GetMotorState(mask);
        }
        set
        {
            Motor = SetBitWithBool(Motor, 1, value != MotorState.Off);
            Motor = SetBitWithBool(Motor, 0, value == MotorState.Reverse);
        }
    }
    private MotorState GetMotorState(byte mask)
    {
        if ((Motor ^ mask) / mask <= 0)
            return MotorState.Off;
        mask >>= 1;
        if ((Motor ^ mask) / mask > 0)
            return MotorState.Reverse;
        return MotorState.On;
    }
    private byte SetBitWithBool(byte value, byte bit_pos, bool new_value)
    {
        value &= (byte)~(1 << bit_pos);
        if (new_value)
        {
            value |= (byte)(1u << bit_pos);
        }

        return value;
    }
}
public enum MotorState
{
    Off,
    On,
    Reverse
}
public enum MotorDirection
{
    Forward = 0b10101010,
    Backward = 0b01010101,
    Left = 0b01101001,
    Right = 0b10010110,
    LeftForward = 0b00101000,
    LeftBackward = 0b01000001,
    RightForward = 0b10000010,
    RightBackward = 0b00010100,
    TurnLeft = 0b10100101,
    TurnRight = 0b01101001,
    Stop = 0b00000000
}