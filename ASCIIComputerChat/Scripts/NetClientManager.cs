using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ASCIIComputerChat.Scripts
{
    public class NetClientManager
    {
        Random random = new Random();
        public NetClient NetClient { get; private set; }
        public string Username { get; set; }
        public string IP { get; private set; }
        public const int PORT = 14725;

        public NetConnectionStatus ConnectionStatus { get { return NetClient.ConnectionStatus; } }

        public NetClientManager()
        {
            NetPeerConfiguration config = new NetPeerConfiguration("ASCIIComputerChat");
            NetClient = new NetClient(config);
            NetClient.Start();

        }

        public void SetIP(string ip)
        {
            this.IP = ip;
        }

        public void Connect()
        {
            if (Username == String.Empty) Username = "user_" + random.Next(100, 999);
            NetClient.Connect(IP, PORT);
        }
        
        public void SendMessage(string message)
        {
            if (NetClient.ConnectionStatus == NetConnectionStatus.Connected)
            {
                var messageToSend = NetClient.CreateMessage();
                messageToSend.Write(Username+ ": " +message);
                NetClient.SendMessage(messageToSend, NetDeliveryMethod.ReliableOrdered);
            }
        }

        public void RegisterMessagesCallback(SendOrPostCallback sendOrPostCallback)
        {
            NetClient.RegisterReceivedCallback(sendOrPostCallback);
        }
    }
}
