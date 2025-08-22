namespace Curiosity;

public class RoverData
{
    /// <summary>
    /// ローバーのID
    /// </summary>
    public string Id { get; set; }
    /// <summary>
    /// ローバーの稼働時間（秒単位）
    /// </summary>
    public int Uptime { get; set; }
    /// <summary>
    /// バッテリーの残量（0〜100の範囲）
    /// </summary>
    public int BatteryLevel { get; set; }
    /// <summary>
    /// ローバーが観測した温度（摂氏）
    /// </summary>
    public double Temperature { get; set; }
    /// <summary>
    /// ローバーが観測した気圧（hPa）
    /// </summary>
    public double Pressure { get; set; }
    /// <summary>
    /// ローバーが観測した湿度（%）
    /// </summary>
    public double Humidity { get; set; }
    /// <summary>
    /// ローバーが観測した測距儀との距離（mm）
    /// </summary>
    public double Distance { get; set; }
    public static RoverData Parse(string rdStr)
    {
        string[] parts = rdStr.Split(',');
        return new RoverData
        {
            Id = parts[0],
            Uptime = int.Parse(parts[1]),
            BatteryLevel = int.Parse(parts[2]),
            Temperature = double.Parse(parts[3]),
            Pressure = double.Parse(parts[4]),
            Humidity = double.Parse(parts[5]),
            Distance = double.Parse(parts[6])
        };
    }
}
