using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TCPServer
{
    class CommandManager
    {
        public static bool TryExecute(byte[] input, ClientObject user)
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

                default:
                    return false;
            }
        }

        private static Command Deserialize(byte[] input, out string arg)
        {
            Command command = (Command)BitConverter.ToInt32(input, 0);
            arg = Encoding.Unicode.GetString(input, 4, input.Length - 4);
            return command;
        }
        private static byte[] Serialize(Command command, string arg)
        {
            byte[] result = new byte[arg.Length * sizeof(char) + sizeof(Command)];
            BitConverter.GetBytes((int)command).CopyTo(result, 0);
            for (int i = 0; i < arg.Length; i++)
            {
                BitConverter.GetBytes(arg[i]).CopyTo(result, sizeof(Command) + i * sizeof(char));
            }
            
            return result;
        }
    }

}
