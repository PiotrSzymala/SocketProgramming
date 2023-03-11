using Client;
using Newtonsoft.Json;

namespace Server;

public class JsonDeserializer
{
    public MyMessage Deserialize(string input)
    {
        var message = JsonConvert.DeserializeObject<MyMessage>(input);

        return message;
    }
}