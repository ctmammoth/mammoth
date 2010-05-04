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
                Mammoth.Engine.Engine game;
                if (args.Length == 2)
                    game = new Mammoth.Engine.Engine(false);
                else
                    game = new Mammoth.Engine.Engine(true);
                game.Run();
            }
            else if (args[0].Equals("server"))
            {
                Server.Server server = new Mammoth.Server.Server();
                server.Run();
            }
            else if (args[0].Equals("content_test"))
            {
                Mammoth.Engine.Engine game = new Mammoth.Engine.Engine();
                ObjectFactories.content_test();
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

