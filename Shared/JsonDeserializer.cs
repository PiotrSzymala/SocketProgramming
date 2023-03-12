using Newtonsoft.Json;

namespace Shared;

public static class JsonDeserializer
{
    public static MyMessage Deserialize(string input)
    {
        var message = JsonConvert.DeserializeObject<MyMessage>(input);

        return message;
    }
}