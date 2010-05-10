using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mammoth.Engine.Networking;
using Mammoth.Engine.Objects;
using Mammoth.Engine.Audio;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Mammoth.Engine
{
    /// <summary>
    /// An InputPlayer that draws itself on the client-side, acting as the client's player.
    /// </summary>
    public class LocalInputPlayer : InputPlayer
    {
        /// <summary>
        /// Initializes the player and loads its model.
        /// </summary>
        /// <param name="game">The game</param>
        public LocalInputPlayer(Game game, int clientID): base(game)
        {
            //Load the Model
            Renderer r = (Renderer)this.Game.Services.GetService(typeof(IRenderService));

            this.ID = clientID << 25;
            this.Model3D = r.LoadModel("soldier-low-poly");
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (this.Health <= 20.0f)
            {
                Console.WriteLine("Playing heartbeat sound.");
                IAudioService audio = (IAudioService)this.Game.Services.GetService(typeof(IAudioService));
                audio.loopSound("Heartbeat");
            }
        }

        public override void Die()
        {
            base.Die();

            IAudioService audio = (IAudioService)this.Game.Services.GetService(typeof(IAudioService));
            audio.stopSound("Heartbeat");
            audio.playSound("Scream");
        }

        #region IEncodable Members

        public override void Decode(byte[] serialized)
        {
            Networking.Encoder props = new Networking.Encoder(serialized);

            if (props.UpdatesFor("Position"))
                Position = (Vector3)props.GetElement("Position", Position);
            if (props.UpdatesFor("Orientation"))
                Orientation = (Quaternion)props.GetElement("Orientation", Orientation);
            if (props.UpdatesFor("HeadOrient"))
                HeadOrient = (Quaternion)props.GetElement("HeadOrient", HeadOrient);
            if (props.UpdatesFor("Velocity"))
                Velocity = (Vector3)props.GetElement("Velocity", Velocity);
            if (props.UpdatesFor("Health"))
                Health = (float)props.GetElement("Health", Health);
            if (props.UpdatesFor("Flag"))
                if (Flag != null)
                    props.UpdateIEncodable("Flag", Flag);
                else
                {
                    Flag = new Flag(this.Game, Vector3.Zero, 0);
                    props.UpdateIEncodable("Flag", Flag);
                }

            //Reroute GameStats update to IGameStats
            if (props.UpdatesFor("GameStats"))
            {
                GameStats gstatus = (GameStats)this.Game.Services.GetService(typeof(IGameStats));
                props.UpdateIEncodable("GameStats", gstatus);
            }

            string gunType = (string)props.GetElement("GunType", "Revolver");
            if (CurWeapon == null || !((BaseObject)CurWeapon).getObjectType().Equals(gunType))
            {
                switch (gunType)
                {
                    case "Revolver":
                        CurWeapon = new Revolver(this.Game, this);
                        break;
                }
            }
            if (props.UpdatesFor("Gun"))
                props.UpdateIEncodable("Gun", CurWeapon);

            //Console.WriteLine("Decoding: " + GameStats.ToString());
        }

        #endregion
    }
}
