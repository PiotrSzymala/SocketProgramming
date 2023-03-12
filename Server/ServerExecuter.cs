using System.Net.Sockets;
using System.Text;
using Shared;

namespace Server;

public static class ServerExecuter
{
    private static byte[] Message { get; set; }
    public static void ExecuteServer()
        {
            using Socket listener = new Socket(Config.IpAddr.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);

            try
            {
                listener.Bind(Config.LocalEndPoint);
                listener.Listen(10);

                Console.WriteLine("Waiting connection ... ");

                using Socket clientSocket = listener.Accept();

                Console.WriteLine("Connected");

                Message = Encoding.ASCII.GetBytes("\nEnter command (type \"help\" to check available commands): \n\n");
                clientSocket.Send(Message);

                bool flag = true;

                while (flag)
                {
                    byte[] bytes = new Byte[1024];
                    string data = string.Empty;

                    int numByte = clientSocket.Receive(bytes);
                    
                    data += Encoding.ASCII.GetString(bytes,
                        0, numByte);

                    var received = JsonDeserializer.Deserialize(data);

                    flag = ChooseOption(received, clientSocket,  flag);
                }
            }

            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

    private static bool ChooseOption(MyMessage received, Socket clientSocket, bool flag)
    {
        byte[] toSend;
        switch (received.Message.ToLower())
        {
            case "uptime":
                toSend = Commands.UptimeCommand();
                clientSocket.Send(toSend);
                break;

            case "info":
                toSend = Commands.InfoCommand();
                clientSocket.Send(toSend);
                break;

            case "help":
                toSend = Commands.HelpCommand();
                clientSocket.Send(toSend);
                break;

            case "stop":
                toSend = Commands.StopCommand();
                clientSocket.Send(toSend);
                
                clientSocket.Shutdown(SocketShutdown.Both);
                clientSocket.Close();

                flag = false;

                break;

            default:
                toSend = Commands.WrongCommand();
                clientSocket.Send(toSend);
                break;
        }

        return flag;
    }
}