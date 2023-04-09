using Shared;

namespace SocektProgramming.Tests;

public class LoggerImplementatnionTests
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