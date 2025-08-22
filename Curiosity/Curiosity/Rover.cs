using System.Text;
using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using Curiosity;

public class Rover
{
    const string ServiceGuid = "0c0e1df3-0007-4d0e-9ff6-7c8eb9e98f2f";
    const string CmdGuid = "0d58e717-57c0-4b0e-9723-c5e41e095897";
    const string TmpGuid = "522bf40e-a020-4d92-b4c5-172fa16c667a";
    IAdapter _adapter = CrossBluetoothLE.Current.Adapter;
    ICharacteristic? _cmdCharacteristic;
    ICharacteristic? _tmpCharacteristic;
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
        var service = await _device.GetServiceAsync(Guid.Parse(ServiceGuid));
        _cmdCharacteristic = await service.GetCharacteristicAsync(Guid.Parse(CmdGuid));
        _tmpCharacteristic = await service.GetCharacteristicAsync(Guid.Parse(TmpGuid));
        _tmpCharacteristic.ValueUpdated += (s, a) =>
        {
            Data = RoverData.Parse(Encoding.UTF8.GetString(a.Characteristic.Value));
        };
        await _tmpCharacteristic.StartUpdatesAsync();
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
    public async Task<bool> SendCommandAsync(Curiosity.Command command)
    {
        if (!IsConnected || _cmdCharacteristic == null)
            return false;

        var commandByte = (byte)command;
        var commandArray = new byte[] { commandByte };
        await _cmdCharacteristic.WriteAsync(commandArray);

        return true;
    }
}