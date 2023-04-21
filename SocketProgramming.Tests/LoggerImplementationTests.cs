using Shared;
using Xunit.Abstractions;

namespace SocketProgramming.Tests;

public class LoggerImplementationTests
{
    private readonly ITestOutputHelper _testOutputHelper;

    public LoggerImplementationTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public void LoggerTest()
    {
        Logger logger = new Logger();
        _testOutputHelper.WriteLine(Environment.CurrentDirectory);
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