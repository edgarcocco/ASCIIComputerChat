using Lidgren.Network;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASCIIComputerChat.Scripts
{
    public static class CommandProcessor
    {
        static Game1 Game;

        public static void Initialize(Game game)
        {
            Game = game as Game1;
        }

        public static string ExecuteCommand(string commandArg)
        {
            commandArg += " ";
            var commandAndArgument = commandArg.Split(' ');
            var command = commandAndArgument[0];
            var argument = commandAndArgument[1];
            switch (command)
            {
                case "":
                    return "";
                case "help":
                    return HelpCommand();
                case "username":
                    return SetUsername(argument);
                case "ip":
                    return SetIP(argument);
                case "connect":
                    Connect();
                    switch (Game.netClientManager.ConnectionStatus)
                    {
                        case NetConnectionStatus.Disconnected:
                        case NetConnectionStatus.RespondedConnect:
                            return "Establishing Connection...";
                        default:
                            return "Unhandled message type of " + Game.netClientManager.ConnectionStatus;
                    }
                default:
                    return "'" + command + "' does not exist";
            }
        }
        public static string HelpCommand()
        {
            return "List of available commands:\n" +
                   "username 'text' - set your username\n" +
                   "ip 'number' - set IP you wish to connect\n" +
                   "connect - log you in the chat";
        }

        public static string SetIP(string ip)
        {
            if (!String.IsNullOrEmpty(ip))
                Game.netClientManager.SetIP(ip);
            else
                return "Error - argument empty";
            return "IP to connect to is - " + ip;
        }

        public static string SetUsername(string username)
        {
            Game.netClientManager.Username = username;
            return username + " has been set as your username";
        }

        public static void Connect()
        {
            Game.netClientManager.Connect();
        }
    }
} 