namespace Shared;

public interface ILogger
{
    public void WriteError(Exception error);
}