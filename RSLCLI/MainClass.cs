
using RedisSharpLite;


class MainClass
{
    static void Main()
    {
        while (true)
        {
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write("redis > ");
            string? command = Console.ReadLine();
            if (string.IsNullOrEmpty(command)) { continue; }

            if (command == "$debug")
            {
                Class2.debugPrint = !Class2.debugPrint;
                Console.WriteLine("Debug print is " + Class2.debugPrint);
                continue;
            }
            else if (command == "$clear")
            {
                Console.Clear();
                continue;
            }

            string response = Class2.Execute(command);
            if (!Class2.debugPrint)
            {
                Console.Write("\n" + response + "\n");
            }
        }
    }
}