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
            using (Mammoth.Engine.Engine game = Mammoth.Engine.Engine.Instance)
            {
                game.Run();
            }
        }
    }
}

