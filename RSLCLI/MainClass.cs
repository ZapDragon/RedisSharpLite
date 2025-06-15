
using System.Net;
using RedisSharpLite;


class MainClass
{
    static private bool debugPrint = false;
    static void Main()
    {
        string ip = "172.24.0.7";
        int port = 6379;
        string database = "0";
        IPEndPoint ep = new IPEndPoint(IPAddress.Parse(ip), port);
        RSL.setEndPoint(ep);

        while (true)
        {
            string head = $"Redis# [{ip}:{port}] [{database}]> ";
            Console.SetCursorPosition(0, Console.CursorTop);

            Console.Write(head);
            Console.Title = head;

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
                database = command.Split(' ')[1];
                RSL.setDatabase(database);
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