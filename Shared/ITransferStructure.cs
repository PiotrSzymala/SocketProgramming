namespace Shared;

public interface ITransferStructure
{
    public void Send<T>(T data);
    public int Receive(byte[] data);
}