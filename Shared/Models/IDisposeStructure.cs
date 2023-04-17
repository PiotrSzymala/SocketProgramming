namespace Shared.Models;

public interface IDisposeStructure
{
    public void Shutdown();
    public void Close();
}