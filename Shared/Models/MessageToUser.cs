namespace Shared.Models;

public class MessageToUser
{
    public string MessageAuthor { get; set; }

    private string _messageContent;
    public string MessageContent
    {
        get
        {
            return _messageContent;
        }
        set
        {
            if (value.Length > 255)
            {
                _messageContent = value.Substring(0, 254);
            }
            else
            {
                _messageContent = value;
            }
        }
    }

    public MessageToUser()
    {
        
    }
}