namespace Shared;

public class Logger : ILogger
{
    private string _path;

    private static string _defaultPath =
        "/Users/piotrszymala/RiderProjects/SocketProgramming/Server/bin/Debug/net7.0/ErrorLogs/errorLog.txt";

    public Logger(string path)
    {
        _path = path;
    }

    public Logger()
    {
        if (!Directory.Exists("ErrorLogs"))
        {
            Directory.CreateDirectory("ErrorLogs");
        }

        _path = _defaultPath;
    }

    public void WriteError(Exception error) 
    {
        if (!File.Exists(_path))
        {
            using (File.Create(_path))
            {
            }
        }

        var currentTime = DateTime.Now;
        using var sw = new StreamWriter(_path,true);
        sw.WriteLine($"{currentTime.Date.ToShortDateString()}, {currentTime.Hour}:{currentTime.Minute}:{currentTime.Second}:{currentTime.Millisecond}");
        sw.WriteLine($"{error}");
        sw.WriteLine();
    }
}