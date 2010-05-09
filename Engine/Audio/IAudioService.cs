using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mammoth.Engine.Audio
{
    public interface IAudioService
    {
        void playMusic(string toPlay);
        void playSound(string toPlay);
        void loopSound(string toPlay);
        void stopSound(string toStop);
    }
}
