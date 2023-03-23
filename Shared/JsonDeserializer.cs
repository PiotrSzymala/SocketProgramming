using Newtonsoft.Json;
using Shared.Models;

namespace Shared;

public static class JsonDeserializer
{
    public static MyMessage Deserialize(string input)
    {
        var message = JsonConvert.DeserializeObject<MyMessage>(input);

        return message;
    }
}