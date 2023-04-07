namespace Shared;

public interface ITransferStructure
{
    public void Send(byte[] data);
    public int Receive(byte[] data);

    public void Shutdown();
    public void Close();
}