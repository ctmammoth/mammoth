using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;

namespace Mammoth.Engine.Audio
{
    public class Audio : GameComponent, IAudioService
    {
        private Dictionary<string, Song> _songs;

        public Audio(Game game)
            : base(game)
        {
            game.Services.AddService(typeof(IAudioService), this);
            _songs = new Dictionary<string, Song>();
            loadSongs();
        }

        private void loadSongs()
        {
            ContentManager manager = this.Game.Content;

            _songs.Add("Main_Menu", manager.Load<Song>("songs/funkytown"));
        }

        #region IAudioService Members

        public void playMusic(string toPlay)
        {
            MediaPlayer.Play(_songs[toPlay]);
        }

        #endregion
    }
}
