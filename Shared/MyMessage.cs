using Newtonsoft.Json;

namespace Shared;

public class MyMessage
{
    [JsonProperty("message")]
    public string? Message { get; set; }
}