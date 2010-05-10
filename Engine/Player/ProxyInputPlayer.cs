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
            //Initialize game
            GameLogic g = (GameLogic)this.Game.Services.GetService(typeof(GameLogic));
            g.InitiateGame();

            //add proxy player to team
            this.ClientID = clientID;
            this.Team = g.AddToTeam(this.ClientID);
            Console.WriteLine("Proxy Player " + this.ClientID + " joined " + this.Team.ToString());

            //add stats to GameStats
            g.UpdatePlayerStats(this.ClientID, PlayerStats);
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
                Vector3 forward = Vector3.Transform(Vector3.Forward, this.HeadOrient);
                Vector3 position = this.Position + (Vector3.Up * this.Height / 4.0f);
                // This might not be quite correct?
                position += forward;

                CurWeapon.Shoot(position, this.HeadOrient, ID, gameTime);
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

        /// <summary>
        /// Overrides InputPlayer's SwitchWeapon() in order to allow switching.
        /// </summary>
        protected override void SwitchWeapon(int newWeapon)
        {
            base.SwitchWeapon(newWeapon);

            if (newWeapon - 1 <= Items.Length && Items[newWeapon - 1] != null)
                CurWeapon = Items[newWeapon - 1];
        }


        public override void TakeDamage(float damage, IDamager inflicter)
        {
            this.Health -= damage;

            if (this.Health <= 0)
            {
                Projectile p = (Projectile)inflicter;

                //update teams kills
                GameLogic g = (GameLogic)this.Game.Services.GetService(typeof(GameLogic));
                g.AwardKill(p.Creator);
                Console.WriteLine("Player " + ClientID + " was killed by Player " + p.Creator);

                //update players kills
                IModelDBService mdb = (IModelDBService)this.Game.Services.GetService(typeof(IModelDBService));
                ProxyInputPlayer pip = (ProxyInputPlayer) mdb.getObject(p.Creator << 25);
                pip.NumKills++;

                //tell player to die
                Die();
            }
        }
        public override void Die()
        {
            base.Die();

            IServerNetworking server = (IServerNetworking)Game.Services.GetService(typeof(INetworkingService));

            // Drop the flag being carried
            if (Flag != null)
            {
                // Keep a reference to the flag that's being dropped
                Objects.Flag droppedFlag = this.Flag;

                // Drop the Flag
                Flag.GetDropped();
                
                // Send the dropped Flag
                server.sendThing(this.Flag);
            }

            server.sendEvent("Death", this.ClientID.ToString());
        }

        public override void RespondToTrigger(PhysicalObject obj)
        {
            Console.WriteLine("ProxyPlayer is responding to a trigger.");

            // If a Flag was triggered, pick it up
            if (obj is Objects.Flag)
                if (Flag == null)
                {
                    // TODO: only pick up flags not owned by your team
                    Flag = (Objects.Flag)obj;
                    Flag.Owner = this;
                    Console.WriteLine("ProxyPlayer picked up a flag!");
                }
                else
                {
                    Console.WriteLine("Dropping off a carried flag at another flag!");
                    Flag.GetDropped();
                    this.Flag = null;
                }
        }


    }
}
