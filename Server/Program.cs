// A C# Program for Server

using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Client;

namespace Server
{
    class Program
    {
        public static readonly DateTime ServerStartCount = DateTime.Now;

        private static byte[] message;

// Main Method
        static void Main(string[] args)
        {
            ExecuteServer();
        }

        public static void ExecuteServer()
        {
            // Establish the local endpoint
            // for the socket. Dns.GetHostName
            // returns the name of the host
            // running the application.
            IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddr, 11111);

            // Creation TCP/IP Socket using
            // Socket Class Constructor
            using Socket listener = new Socket(ipAddr.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);

            try
            {
                // Using Bind() method we associate a
                // network address to the Server Socket
                // All client that will connect to this
                // Server Socket must know this network
                // Address
                listener.Bind(localEndPoint);

                // Using Listen() method we create
                // the Client list that will want
                // to connect to Server
                listener.Listen(10);
                Console.WriteLine("Waiting connection ... ");

                // Suspend while waiting for
                // incoming connection Using
                // Accept() method the server
                // will accept connection of client
                using Socket clientSocket = listener.Accept();

                    message = Encoding.ASCII.GetBytes($"Enter command (type \"help\" to check available commands): ");
                    clientSocket.Send(message);

                    bool flag = true;
                    
                while (flag)
                {


                    // Data buffer

                    
                        byte[] bytes = new Byte[1024];
                        string data = null;
                        MyMessage received = new MyMessage();

                        int numByte = clientSocket.Receive(bytes);


                        data += Encoding.ASCII.GetString(bytes,
                            0, numByte);

                        received = JsonDeserializer.Deserialize(data);


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
                                clientSocket.Shutdown(SocketShutdown.Both);
                                clientSocket.Close();

                                flag = false;
                                
                                break;

                            default:
                                toSend = Encoding.ASCII.GetBytes( "wrong command");
                                clientSocket.Send(toSend);
                                break;
                        }
                    
                        
                    // Send a message to Client
                    // using Send() method

                    // Close client Socket using the
                    // Close() method. After closing,
                    // we can use the closed Socket
                    // for a new Client Connection
                    // clientSocket.Shutdown(SocketShutdown.Both);
                    // clientSocket.Close();
                }
            }

            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}