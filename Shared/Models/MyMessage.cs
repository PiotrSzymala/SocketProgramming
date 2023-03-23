using Newtonsoft.Json;

namespace Shared.Models;

public class MyMessage
{
    [JsonProperty("message")]
    public string? Message { get; set; }
}