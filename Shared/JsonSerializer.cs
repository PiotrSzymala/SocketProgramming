using Newtonsoft.Json;

namespace Shared;

public static class JsonSerializer
{
    public static string Serialize(MyMessage message)
    {
        var result = JsonConvert.SerializeObject(message);
        
        return result;
    }
}