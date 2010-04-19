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
            using (Mammoth.Engine.Engine game = new Mammoth.Engine.Engine())
            {
                game.Run();
            }
        }

        /// One thing that could be done is to make some kind of GameLogic class that
        /// subclasses GameComponent and handles all of the state changes and the like
        /// in the game.  It could deal with loading data for new matches (in a thread,
        /// I suppose), as well as changing state variables so that the engine can correctly
        /// handle things such as menus.
    }
}

