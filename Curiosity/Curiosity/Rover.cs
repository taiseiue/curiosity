using System.Text;
using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using Curiosity;

public class Rover
{
    IAdapter _adapter = CrossBluetoothLE.Current.Adapter;
    ICharacteristic? idCharacteristic;
    ICharacteristic? voltageCharacteristic;
    ICharacteristic? batteryLevelCharacteristic;
    ICharacteristic? isChargingCharacteristic;
    ICharacteristic? uptimeCharacteristic;
    ICharacteristic? temperatureCharacteristic;
    ICharacteristic? pressureCharacteristic;
    ICharacteristic? humidityCharacteristic;
    ICharacteristic? distanceCharacteristic;
    ICharacteristic? motorCharacteristic;
    ICharacteristic? isCatchingCharacteristic;
    ICharacteristic? ledBrightnessCharacteristic;

    public Rover()
    {
        _adapter.DeviceConnected += (s, a) =>
        {
            // Handle device connected event
        };

        _adapter.DeviceDisconnected += async (s, a) =>
        {
            await StartConnect();
        };

    }
    public bool IsConnected => _adapter.ConnectedDevices.Count > 0;
    public bool Enable { get; set; } = true;
    public RoverData Data { get; set; } = new RoverData();
    public Guid DeviceGuid { get; set; } = new Guid("00000000-0000-0000-0000-84cca860fefa");
    private IDevice? _device;
    public async Task ConnectAsync()
    {
        _device = await _adapter.ConnectToKnownDeviceAsync(DeviceGuid);
        var service = await _device.GetServiceAsync(UUIDs.Service);
        idCharacteristic = await service.GetCharacteristicAsync(UUIDs.Id);
        voltageCharacteristic = await service.GetCharacteristicAsync(UUIDs.Voltage);
        batteryLevelCharacteristic = await service.GetCharacteristicAsync(UUIDs.BatteryLevel);
        isChargingCharacteristic = await service.GetCharacteristicAsync(UUIDs.IsCharging);
        uptimeCharacteristic = await service.GetCharacteristicAsync(UUIDs.Uptime);
        temperatureCharacteristic = await service.GetCharacteristicAsync(UUIDs.Temperature);
        pressureCharacteristic = await service.GetCharacteristicAsync(UUIDs.Pressure);
        humidityCharacteristic = await service.GetCharacteristicAsync(UUIDs.Humidity);
        distanceCharacteristic = await service.GetCharacteristicAsync(UUIDs.Distance);
        motorCharacteristic = await service.GetCharacteristicAsync(UUIDs.Motor);
        isCatchingCharacteristic = await service.GetCharacteristicAsync(UUIDs.IsCatching);
        ledBrightnessCharacteristic = await service.GetCharacteristicAsync(UUIDs.LedBrightness);

        idCharacteristic.ValueUpdated += (s, a) => Data.Id = Encoding.UTF8.GetString(a.Characteristic.Value);
        voltageCharacteristic.ValueUpdated += (s, a) => Data.Voltage = int.Parse(Encoding.UTF8.GetString(a.Characteristic.Value));
        batteryLevelCharacteristic.ValueUpdated += (s, a) => Data.BatteryLevel = int.Parse(Encoding.UTF8.GetString(a.Characteristic.Value));
        isChargingCharacteristic.ValueUpdated += (s, a) => Data.IsCharging = bool.Parse(Encoding.UTF8.GetString(a.Characteristic.Value));
        uptimeCharacteristic.ValueUpdated += (s, a) => Data.Uptime = int.Parse(Encoding.UTF8.GetString(a.Characteristic.Value));
        temperatureCharacteristic.ValueUpdated += (s, a) => Data.Temperature = float.Parse(Encoding.UTF8.GetString(a.Characteristic.Value));
        pressureCharacteristic.ValueUpdated += (s, a) => Data.Pressure = float.Parse(Encoding.UTF8.GetString(a.Characteristic.Value));
        humidityCharacteristic.ValueUpdated += (s, a) => Data.Humidity = float.Parse(Encoding.UTF8.GetString(a.Characteristic.Value));
        distanceCharacteristic.ValueUpdated += (s, a) => Data.Distance = float.Parse(Encoding.UTF8.GetString(a.Characteristic.Value));

        await idCharacteristic.StartUpdatesAsync();
        await voltageCharacteristic.StartUpdatesAsync();
        await batteryLevelCharacteristic.StartUpdatesAsync();
        await isChargingCharacteristic.StartUpdatesAsync();
        await uptimeCharacteristic.StartUpdatesAsync();
        await temperatureCharacteristic.StartUpdatesAsync();
        await pressureCharacteristic.StartUpdatesAsync();
        await humidityCharacteristic.StartUpdatesAsync();
        await distanceCharacteristic.StartUpdatesAsync();
    }
    public async Task StartConnect()
    {
        while (Enable && !IsConnected)
        {
            try
            {
                await ConnectAsync();
            }
            catch { }
        }
    }
    public async Task Move(MotorDirection direction)
    {
        var directionByte = (byte)direction;
        var directionArray = new byte[] { directionByte };
        await motorCharacteristic.WriteAsync(directionArray);
        Data.Direction = direction;
    }
    public async Task<bool> SendCommandAsync(Curiosity.Command command)
    {
        return true;
    }
}