using Newtonsoft.Json;

namespace Client;

public class MyMessage
{
    [JsonProperty("message")]
    public string Message { get; set; }
}