using System.Text.Json.Serialization;

public class BinanceResponse
{
    [JsonPropertyName("symbol")]
    public string Symbol { get; set; }

    [JsonPropertyName("price")]
    public string Price { get; set; }
}