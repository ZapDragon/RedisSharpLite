
using System.Net;
using RedisSharpLite;


class MainClass
{
    static private bool debugPrint = false;
    static void Main()
    {
        RSL.setEndPoint(new IPEndPoint(IPAddress.Parse("172.24.0.7"), 6379));

        while (true)
        {
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write("redis > ");
            string? command = Console.ReadLine();
            if (string.IsNullOrEmpty(command)) { continue; }

            if (command == "debug")
            {
                bool debugPrint = RSL.toggleDebug();
                Console.WriteLine("Debug print is " + debugPrint);
                continue;
            }
            else if (command == "clear")
            {
                Console.Clear();
                continue;
            }
            else if (command.StartsWith("select"))
            {
                RSL.setDatabase(command.Split(' ')[1]);
            }

            string response = RSL.ExecuteOnce(command);
            if (!debugPrint)
            {
                if (Console.CursorLeft > 0) { Console.WriteLine(); }
                Console.Write(response);
                if (Console.CursorLeft > 0) { Console.WriteLine(); }
            }
        }
    }
}