using System.Net.Sockets;
using System.Text;
using Shared;

namespace Client;

public class ClientExecuter
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
                        int byteSent = sender.Send(messageSent);
                        
                        byte[] messageReceived = new byte[1024];
                        
                        int byteRecv = sender.Receive(messageReceived);
                        string result = Encoding.ASCII.GetString(messageReceived,
                            0, byteRecv);
                        Console.WriteLine($"Message from Server -> {result}");
                        
                    } while (messageToSent.Message != "stop");
                    
                    sender.Shutdown(SocketShutdown.Both);
                    sender.Close();
                }

                catch (ArgumentNullException ane)
                {
                    Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
                }

                catch (SocketException se)
                {
                    Console.WriteLine("SocketException : {0}", se.ToString());
                }

                catch (Exception e)
                {
                    Console.WriteLine("Unexpected exception : {0}", e.ToString());
                }
            }

            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
}