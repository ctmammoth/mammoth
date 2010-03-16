using System;

namespace Mammoth
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (Mammoth game = new Mammoth())
            {
                game.Run();
            }
        }
    }
}

