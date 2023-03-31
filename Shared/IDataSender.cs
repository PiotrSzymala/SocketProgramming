namespace Shared;

public interface IDataSender
{
    byte[] SendData(string messageContext);
}