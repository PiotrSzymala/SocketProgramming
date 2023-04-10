using Shared;

namespace SocketProgramming.Tests;

public class LoggerImplementationTests
{
    [Fact]
    public void LoggerTest()
    {
        Logger logger = new Logger();

        try
        {
           // throw new Exception("Testing");
            throw new IndexOutOfRangeException();
        }
        catch (IndexOutOfRangeException e)
        {
            logger.WriteError(e);
        }
        catch (Exception e)
        {
            logger.WriteError(e);
        }
    }  
}