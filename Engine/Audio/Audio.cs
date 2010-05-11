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
        private Dictionary<string, List<Song>> _playlists;
        private Dictionary<string, SoundEffect> _sounds;
        private Dictionary<string, float> _volumes;
        private Dictionary<string, SoundEffectInstance> _loopedSounds;
        private int _currentSong;
        private string _currentPlaylist;

        /// <summary>
        /// Construct the audio service by initializing collections
        /// and calling the methods to load sounds and music.
        /// </summary>
        /// <param name="game"></param>
        public Audio(Game game)
            : base(game)
        {
            game.Services.AddService(typeof(IAudioService), this);
            _playlists = new Dictionary<string, List<Song>>();
            _sounds = new Dictionary<string, SoundEffect>();
            _loopedSounds = new Dictionary<string, SoundEffectInstance>();
            _volumes = new Dictionary<string, float>();
            loadSongs();
            loadSounds();
            MediaPlayer.Volume = 0.5f;
        }

        /// <summary>
        /// Uses the content manager to load all the songs and
        /// put them into playlists.
        /// </summary>
        private void loadSongs()
        {
            ContentManager manager = this.Game.Content;

            _playlists.Add("Main_Menu", new List<Song>());
            _playlists["Main_Menu"].Add(manager.Load<Song>("songs/onestop"));
        }

        /// <summary>
        /// Uses the content manager to load all the sound effects.
        /// </summary>
        private void loadSounds()
        {
            ContentManager manager = this.Game.Content;

            _sounds.Add("Gunshot", manager.Load<SoundEffect>("sounds/gunshot"));
            _sounds.Add("SMGShot", manager.Load<SoundEffect>("sounds/machinegun"));
            _volumes.Add("SMGShot", 1.0f);
            _sounds.Add("Ambient", manager.Load<SoundEffect>("sounds/ambient"));
            _volumes.Add("Ambient", 0.3f);
            _sounds.Add("Reload", manager.Load<SoundEffect>("sounds/reload"));
            _volumes.Add("Reload", 1.0f);
            _sounds.Add("Grunt", manager.Load<SoundEffect>("sounds/grunt"));
            _sounds.Add("Scream", manager.Load<SoundEffect>("sounds/scream"));
            _volumes.Add("Scream", 1.0f);
            _sounds.Add("Heartbeat", manager.Load<SoundEffect>("sounds/heartbeat"));
            _volumes.Add("Heartbeat", 1.0f);
            _sounds.Add("GunEmpty", manager.Load<SoundEffect>("sounds/gunempty"));
            _sounds.Add("ShotgunFire", manager.Load<SoundEffect>("sounds/shotgunfire"));
            _sounds.Add("ShotgunFireLoad", manager.Load<SoundEffect>("sounds/shotgunfireload"));
        }

        #region IAudioService Members

        /// <summary>
        /// Plays a specified playlist, or stops the music
        /// if the playlist doesn't exist.
        /// </summary>
        /// <param name="toPlay"></param>
        public void playMusic(string toPlay)
        {
            if (!_playlists.ContainsKey(toPlay))
            {
                MediaPlayer.Stop();
                _currentPlaylist = null;
            }
            else
            {
                _currentSong = 0;
                _currentPlaylist = toPlay;
                MediaPlayer.Play(_playlists[toPlay][0]);
            }
        }

        /// <summary>
        /// Plays a specified sound effect once if 
        /// it exists.
        /// </summary>
        /// <param name="toPlay"></param>
        public void playSound(string toPlay)
        {
            if (!_sounds.ContainsKey(toPlay))
                return;
            // If a specific volume is specified, use it
            if (_volumes.ContainsKey(toPlay))
                _sounds[toPlay].Play(_volumes[toPlay], 0.0f, 0.0f);
            else
                _sounds[toPlay].Play(0.5f, 0.0f, 0.0f);
        }

        /// <summary>
        /// Plays a specified sound effect in a loop
        /// (until stopSound is called on it).
        /// </summary>
        /// <param name="toPlay"></param>
        public void loopSound(string toPlay)
        {
            if (_loopedSounds.ContainsKey(toPlay) || !_sounds.ContainsKey(toPlay))
                return;
            SoundEffectInstance sei = _sounds[toPlay].CreateInstance();
            _loopedSounds.Add(toPlay, sei);
            // If a specific volume is specified, use it
            if (_volumes.ContainsKey(toPlay))
                sei.Volume = _volumes[toPlay];
            else
                sei.Volume = 0.5f;
            sei.Play();
        }

        /// <summary>
        /// Stops the specified looped sound if it's playing.
        /// </summary>
        /// <param name="toStop"></param>
        public void stopSound(string toStop)
        {
            if (!_loopedSounds.ContainsKey(toStop))
                return;
            SoundEffectInstance sei = _loopedSounds[toStop];
            _loopedSounds.Remove(toStop);
            sei.Stop();
        }

        #endregion IAudioService Members

        /// <summary>
        /// Manages music and looped sounds by moving forward in (or
        /// looping back to the start of) the current playlist if the
        /// current song has stopped and replaying looped sounds if
        /// they have stopped.
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (MediaPlayer.State != MediaState.Playing && _currentPlaylist != null)
            {
                _currentSong = (_currentSong + 1) % _playlists[_currentPlaylist].Count;
                MediaPlayer.Play(_playlists[_currentPlaylist][_currentSong]);
            }
            foreach (SoundEffectInstance sei in _loopedSounds.Values)
            {
                if (sei.State != SoundState.Playing)
                    sei.Play();
            }
        }
    }
}
