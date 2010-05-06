using System;

using Mammoth.Engine;

namespace Mammoth
{
    static class MammothMain
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            if (!(args.Length == 1 || args.Length == 2))
            {
                Console.WriteLine("Usage: add 'server' or 'client' to indicate mode.");
                return;
            }
            if (args[0].Equals("client"))
            {
                Client client = new Client();
                client.Run();
            }
            else if (args[0].Equals("server"))
            {
                Server.Server server = new Mammoth.Server.Server();
                server.Run();
            }
            else if (args[0].Equals("content_test"))
            {
                int x = 0;
                Client client = new Client();
                client.Run();
                // ObjectFactories.content_test(client);
            }
            else
            {
                Console.WriteLine("Usage: add 'server' or 'client' to indicate mode.");
                return;
            }
        }

        /// One thing that could be done is to make some kind of GameLogic class that
        /// subclasses GameComponent and handles all of the state changes and the like
        /// in the game.  It could deal with loading data for new matches (in a thread,
        /// I suppose), as well as changing state variables so that the engine can correctly
        /// handle things such as menus.
    }
}

