using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TCPServer
{
    class CommandManager
    {
        public static bool TryExecute(byte[] input, ClientObject user, ServerObject server)
        {
            if (input.SequenceEqual(new byte[64]))
            {
                return false;
            }
            
            Command command = Deserialize(input, out var arg);
            switch (command)
            {
                case Command.Print:
                    Console.WriteLine(arg);
                    return true;
                case Command.SendMessage:
                    return SendMessage(arg, server);
                case Command.Broadcast:
                    return BroadcastMessage(arg, server);
                case Command.SetId:
                    return SendId(user, server);
                default:
                    return false;
            }
        }

        public static Command Deserialize(byte[] input, out string arg)
        {
            Command command = (Command)BitConverter.ToInt32(input, 0);
            arg = Encoding.Unicode.GetString(input, 4, input.Length - 4);
            return command;
        }
        public static byte[] Serialize(Command command, string arg)
        {
            byte[] result = new byte[arg.Length * sizeof(char) + sizeof(Command)];
            BitConverter.GetBytes((int)command).CopyTo(result, 0);
            for (int i = 0; i < arg.Length; i++)
            {
                BitConverter.GetBytes(arg[i]).CopyTo(result, sizeof(Command) + i * sizeof(char));
            }
            
            return result;
        }


        public static bool SendMessage(string arg, ServerObject server)
        {
            var splited = arg.Split(' ');
            if (splited.Length < 2) 
            {
                return false;
            }
            var id = splited[0];
            StringBuilder builder = new StringBuilder();
            for (int i = 1; i < splited.Length; i++)
            {
                builder.Append(splited[i] + ' ');
            }
            builder.Remove(builder.ToString().Length - 1, 1);
            var message = builder.ToString();
            var bytes = Serialize(Command.Print, message);
            server.SendMessage(bytes, id);
            return true;
        }

        public static bool BroadcastMessage(string arg, ServerObject server)
        {
            var splited = arg.Split(' ');
            if (splited.Length < 2)
            {
                return false;
            }
            var id = splited[0];
            StringBuilder builder = new StringBuilder();
            for (int i = 1; i < splited.Length; i++)
            {
                builder.Append(splited[i] + ' ');
            }
            builder.Remove(builder.ToString().Length - 1, 1);
            var message = builder.ToString();
            var bytes = Serialize(Command.Print, message);
            server.BroadcastMessage(bytes, id);
            return true;
        }

        public static bool SendId(ClientObject user, ServerObject server)
        {
            var message = Serialize(Command.SetId, user.Id);
            server.SendMessage(message, user.Id);
            return true;
        }
    }

}
