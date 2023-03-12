using System.Net.Sockets;
using System.Text;
using Shared;

namespace Client;

public static class ClientExecuter
{
   public static void ExecuteClient()
        {
            try
            {
                using Socket sender = new Socket(Config.IpAddr.AddressFamily,
                    SocketType.Stream, ProtocolType.Tcp);

                try
                {
                    sender.Connect(Config.LocalEndPoint);
                    
                    Console.WriteLine($"Socket connected to -> {sender.RemoteEndPoint}");


                    byte[] initialCommand = new byte[1024];

                    int initComm = sender.Receive(initialCommand);

                    string encodingInitComm = Encoding.ASCII.GetString(initialCommand, 0, initComm);
                    Console.WriteLine(encodingInitComm);
                    
                    MyMessage messageToSent = new MyMessage();
                    do
                    {
                        messageToSent.Message = Console.ReadLine().ToLower();

                        var toSend = JsonSerializer.Serialize(messageToSent);

                        byte[] messageSent = Encoding.ASCII.GetBytes(toSend);
                        
                        sender.Send(messageSent);
                        
                        byte[] messageReceived = new byte[1024];
                        
                        int byteRecv = sender.Receive(messageReceived);
                        
                        string fromServerResult = Encoding.ASCII.GetString(messageReceived,
                            0, byteRecv);
                        
                     var messageFromServer = JsonDeserializer.Deserialize(fromServerResult);

                        Console.WriteLine($"Message from Server -> {messageFromServer.Message}");
                        
                    } while (messageToSent.Message != "stop");
                    
                    sender.Shutdown(SocketShutdown.Both);
                    sender.Close();
                }

                catch (ArgumentNullException ane)
                {
                    Console.WriteLine($"ArgumentNullException : {ane}");
                }

                catch (SocketException se)
                {
                    Console.WriteLine($"SocketException : {se}");
                }

                catch (Exception e)
                {
                    Console.WriteLine($"Unexpected exception : {e}");
                }
            }

            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
}