using Newtonsoft.Json;

namespace Client;

public class JsonSerializer
{
    public string Serialize(MyMessage message)
    {
        var result = JsonConvert.SerializeObject(message);
        
        return result;
    }
}