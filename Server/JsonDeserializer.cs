using Client;
using Newtonsoft.Json;

namespace Server;

public class JsonDeserializer
{
    public static MyMessage Deserialize(string input)
    {
        var message = JsonConvert.DeserializeObject<MyMessage>(input);

        return message;
    }
}