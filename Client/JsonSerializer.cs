using Newtonsoft.Json;
using Shared;

namespace Client;

public class JsonSerializer
{
    public static string Serialize(MyMessage message)
    {
        var result = JsonConvert.SerializeObject(message);
        
        return result;
    }
}