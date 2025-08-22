using System.Net;
using Curiosity;

public class APIClient
{
    public APIClient(string baseUrl)
    {
        BaseUrl = baseUrl;
    }
    public async Task<bool> PostData(RoverData data, byte[] imageData)
    {
        using var streamContent = new StreamContent(new MemoryStream(imageData));
        using var form = new MultipartFormDataContent
        {
            { new StringContent(data.Id), "id" },
            { new StringContent(data.Voltage.ToString()), "voltage" },
            { new StringContent(data.BatteryLevel.ToString()), "batteryLevel" },
            { new StringContent(data.IsCharging.ToString()), "isCharging" },
            { new StringContent(data.Uptime.ToString()), "uptime" },
            { new StringContent(data.Temperature.ToString()), "temperature" },
            { new StringContent(data.Pressure.ToString()), "pressure" },
            { new StringContent(data.Humidity.ToString()), "humidity" },
            { new StringContent(data.Distance.ToString()), "distance" },
            { new StringContent(data.IsCatching.ToString()), "isCatching" },
            { new StringContent(data.LedBrightness.ToString()), "ledBrightness" },
            { new StringContent(data.MotorFrontLeft.ToString()), "motorFrontLeft" },
            { new StringContent(data.MotorBackLeft.ToString()), "motorBackLeft" },
            { new StringContent(data.MotorFrontRight.ToString()), "motorFrontRight" },
            { new StringContent(data.MotorBackRight.ToString()), "motorBackRight" },
            { streamContent, "image", "capture.jpg" }
        };

        HttpResponseMessage response = await _httpClient.PostAsync($"{BaseUrl}/api/v0/post", form);
        return response.IsSuccessStatusCode;
    }
    public string BaseUrl { get; }
    private HttpClient _httpClient = new HttpClient();
}