using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASCIIComputerChatServer
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = new NetPeerConfiguration("ASCIIComputerChat") { Port=14725};
            var server = new NetServer(config);
            Console.WriteLine("Starting Server on " + config.BroadcastAddress.MapToIPv4() + " Port " + config.Port + "...");
            server.Start();
            if (server.Status == NetPeerStatus.Running)
                Console.WriteLine("Server is running!");
            else
            {
                Console.WriteLine(server.Status);
                Console.WriteLine("Server couldn't start!");
            }

            NetIncomingMessage message;
            while (true)
            {
                while ((message = server.ReadMessage()) != null)
                {
                    switch (message.MessageType)
                    {
                        case NetIncomingMessageType.Data:
                            var data = message.ReadString();
                            var messageToSend = server.CreateMessage();
                            messageToSend.Write(data);
                            server.SendToAll(messageToSend, NetDeliveryMethod.ReliableOrdered);
                            break;
                        case NetIncomingMessageType.StatusChanged:
                            // handle connection status messages
                            switch (message.SenderConnection.Status)
                            {
                                case NetConnectionStatus.Connected:
                                    Console.WriteLine("Connected " + (message.SenderConnection.RemoteEndPoint));
                                    break;
                                default:
                                    Console.WriteLine(message.SenderConnection.Status);
                                    break;
                            }
                            break;
                        default:
                            Console.WriteLine("unhandled message with type: "
                                + message.MessageType);
                            break;
                    }
                }
            }
        }
    }
}
