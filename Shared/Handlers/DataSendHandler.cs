namespace Shared.Handlers;

public class DataSendHandler
{
    private IDataSender _dataSender;

    public DataSendHandler(IDataSender dataSender)
    {
        _dataSender = dataSender;
    }

    public byte[] Send(string context)
    {
        return _dataSender.SendData(context);
    }
}