using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

using Mammoth.Engine.Input;
using Mammoth.Engine.Networking;

namespace Mammoth.Engine
{
    /// <summary>
    /// A server side player that can updated by InputStates sent from clients.
    /// </summary>
    class ProxyInputPlayer : InputPlayer
    {
        #region Properties

        //CLIENT ID
        public int ClientID
        {
            get;
            protected set;
        }

        //TEAM
        public Team Team
        {
            get;
            set;
        }

        #endregion

        /// <summary>
        /// Constructs a new ProxyPlayer by associating the ProxyPlayer to a client.
        /// </summary>
        /// <param name="game">The game.</param>
        /// <param name="clientID">The ID of the client this player is simulating on the server</param>
        public ProxyInputPlayer(Game game, int clientID): base(game)
        {
            Console.WriteLine("Creating proxy player");
            this.ClientID = clientID;
            IGameLogic g = (IGameLogic)this.Game.Services.GetService(typeof(IGameLogic));
            this.Team = g.AddToTeam(this.ClientID);
            Console.WriteLine("Proxy Player " + this.ClientID + " joined " + this.Team.ToString());
        }

        /// <summary>
        /// Gets the input state for this player from IInputService then uses InputPlayer's update to update appropriatly.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            // Get the input state.
            IInputService input = (IInputService) this.Game.Services.GetService(typeof(IInputService));
            input.SetStateByClientID(this.ClientID);

            // Update player using emulated input state.
            base.Update(gameTime);

            //Once the player has been updated server side, send it to clients to display as RemotePlayer's
            IServerNetworking network = (IServerNetworking)this.Game.Services.GetService(typeof(INetworkingService));
            network.sendThing(this);
        }

        /// <summary>
        /// Overrides InputPlayer's Throw() in order to allow shooting with respect to player's position and orientation.
        /// </summary>
        protected override void Shoot(GameTime gameTime)
        {
            // Shoot if the player currently has a weapon
            if (CurWeapon != null)
            {
                //Calculate initial position of bullet to shoot from
                /*Vector3 direction = Vector3.Transform(Vector3.Forward, HeadOrient) * 1000.0f;
                direction.Normalize();
                Vector3 position = Position + (Vector3.Up * Height / 4.0f);
                Vector3 offset = Vector3.Multiply(direction, 2.0f);
                position = Vector3.Add(position, offset);*/

                Vector3 forward = Vector3.Transform(Vector3.Forward, this.HeadOrient);
                Vector3 position = this.Position + (Vector3.Up * this.Height / 4.0f);
                // This might not be quite correct?
                //position += forward;

                CurWeapon.Shoot(position, this.Orientation, ID, gameTime);
            }
        }

        /// <summary>
        /// Overrides InputPlayer's Reload() in order to allow reloading.
        /// </summary>
        protected override void Reload(GameTime time)
        {
            Console.WriteLine("Proxyplayer is reloading.");
            if (CurWeapon != null)
                CurWeapon.Reload(time);
        }

        public override void TakeDamage(float damage, IDamager inflicter)
        {
            Console.WriteLine("Proxy player took damage");
            if (this.Health <= 0)
            {
                Projectile p = (Projectile)inflicter;

                //update teams kills
                IGameLogic g = (IGameLogic)this.Game.Services.GetService(typeof(IGameLogic));
                int cid = p.Creator >> 25;
                g.AwardKill(cid);

                //update players kills
                IModelDBService mdb = (IModelDBService)this.Game.Services.GetService(typeof(IModelDBService));
                ProxyInputPlayer pip = (ProxyInputPlayer) mdb.getObject(p.Creator);
                pip.NumKills++;
            }
            base.TakeDamage(damage, inflicter);
        }
    }
}
