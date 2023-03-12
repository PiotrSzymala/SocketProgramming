using Newtonsoft.Json;
using Shared;

namespace Client;

public static class JsonSerializer
{
    public static string Serialize(MyMessage message)
    {
        var result = JsonConvert.SerializeObject(message);
        
        return result;
    }
}