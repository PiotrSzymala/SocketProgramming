using Newtonsoft.Json;

namespace Shared.Models;

public class CommandFromUser
{
    [JsonProperty("command")]
    public string? Command { get; set; }
}