using System;

namespace TankWars
{
    class Program
    {
        static void Main(string[] args)
        {
            Settings settings = new Settings(@"..\..\..\..\Resources\settings.xml");
            ServerController serverController = new ServerController(settings);
            serverController.Start();
            Console.Read();
        }
    }
}
