using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mammoth.Engine.Audio
{
    /// <summary>
    /// Provides methods for playing sounds and music.
    /// </summary>
    public interface IAudioService
    {
        /// <summary>
        /// Play a playlist (e.g. the Main_Menu playlist)
        /// </summary>
        /// <param name="toPlay"></param>
        void playMusic(string toPlay);

        /// <summary>
        /// Play a single sound effect.
        /// </summary>
        /// <param name="toPlay"></param>
        void playSound(string toPlay);

        /// <summary>
        /// Play a sound effect in a loop
        /// </summary>
        /// <param name="toPlay"></param>
        void loopSound(string toPlay);

        /// <summary>
        /// Stop a looped sound effect if it's playing
        /// </summary>
        /// <param name="toStop"></param>
        void stopSound(string toStop);
    }
}
