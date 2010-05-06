using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;

namespace Mammoth.Engine.Audio
{
    public class Audio : GameComponent, IAudioService
    {
        private Dictionary<string, List<Song>> _songs;
        private Dictionary<string, SoundEffect> _sounds;
        private Dictionary<string, float> _volumes;
        private List<SoundEffectInstance> _loopedSounds;
        private int _currentSong;
        private string _currentPlaylist;

        public Audio(Game game)
            : base(game)
        {
            game.Services.AddService(typeof(IAudioService), this);
            _songs = new Dictionary<string, List<Song>>();
            _sounds = new Dictionary<string, SoundEffect>();
            _loopedSounds = new List<SoundEffectInstance>();
            _volumes = new Dictionary<string, float>();
            loadSongs();
            loadSounds();
            MediaPlayer.Volume = 0.7f;
        }

        private void loadSongs()
        {
            ContentManager manager = this.Game.Content;

            _songs.Add("Main_Menu", new List<Song>());
            _songs["Main_Menu"].Add(manager.Load<Song>("songs/onestop"));

            _songs.Add("In_Game", new List<Song>());
            _songs["In_Game"].Add(manager.Load<Song>("songs/momentviolence"));
            _songs["In_Game"].Add(manager.Load<Song>("songs/minority"));
            _songs["In_Game"].Add(manager.Load<Song>("songs/superman"));
        }

        private void loadSounds()
        {
            ContentManager manager = this.Game.Content;

            _sounds.Add("Gunshot", manager.Load<SoundEffect>("sounds/gunshot"));
            _sounds.Add("Ambient", manager.Load<SoundEffect>("sounds/ambient"));
            _sounds.Add("Reload", manager.Load<SoundEffect>("sounds/reload"));
            _volumes.Add("Reload", 1.0f);
        }

        #region IAudioService Members

        public void playMusic(string toPlay)
        {
            _currentSong = 0;
            _currentPlaylist = toPlay;
            Song s;
            MediaPlayer.Play(_songs[toPlay][0]);
        }

        public void playSound(string toPlay)
        {
            if (_volumes.ContainsKey(toPlay))
                _sounds[toPlay].Play(_volumes[toPlay], 0.0f, 0.0f);
            else
                _sounds[toPlay].Play(0.7f, 0.0f, 0.0f);
        }

        public void loopSound(string toPlay)
        {
            SoundEffectInstance sei = _sounds[toPlay].CreateInstance();
            _loopedSounds.Add(sei);
            if (_volumes.ContainsKey(toPlay))
                sei.Volume = _volumes[toPlay];
            else
                sei.Volume = 0.7f;
            sei.Play();
        }

        #endregion IAudioService Members

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (MediaPlayer.State != MediaState.Playing)
            {
                _currentSong = (_currentSong + 1) % _songs[_currentPlaylist].Count;
                //MediaPlayer.Play(_songs[_currentPlaylist][_currentSong]);
            }
            foreach (SoundEffectInstance sei in _loopedSounds)
            {
                if (sei.State != SoundState.Playing)
                    sei.Play();
            }
        }
    }
}
