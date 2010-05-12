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
            //Console.WriteLine("Proxy Player " + this.ClientID + " joined " + this.Team.ToString());

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
        /// Causes a room to be spawned for this player's team.  Places it on top of the team's other rooms.
        /// </summary>
        /// <param name="roomPos">The position in which to spawn the room.</param>
        protected override void SpawnRoom(Vector3 roomPos)
        {
            //Console.WriteLine("ProxyInputPlayer is spawning a room.");

            IModelDBService modelDB = (IModelDBService)this.Game.Services.GetService(typeof(IModelDBService));

            ObjectParameters stairRoom = new ObjectParameters();
            stairRoom.AddAttribute("X", "-50");
            stairRoom.AddAttribute("Y", "-2");
            stairRoom.AddAttribute("Z", "-50");
            stairRoom.AddAttribute("Special_Type", "STAIR_ROOM");            
            Room room = Room.NewTowerRoom(this.PlayerStats.YourTeam.ToString(), modelDB, Game);

            // Register the room in the model DB
            room.ID = modelDB.getNextOpenID();
            modelDB.registerObject(room);

            IServerNetworking net = (IServerNetworking)this.Game.Services.GetService(typeof(INetworkingService));
            // Send the room
            net.sendThing(room);
        }

        /// <summary>
        /// Overrides InputPlayer's Throw() in order to allow shooting with respect to player's position and orientation.  The 
        /// current gun's Shoot() method is called.
        /// </summary>
        /// <param name="gameTime">The game time.  Used by the gun to control the rate of fire.</param>
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
        /// <param name="gameTime">The game time.</param>
        protected override void Reload(GameTime time)
        {
            //Console.WriteLine("Proxyplayer is reloading.");
            if (CurWeapon != null)
                CurWeapon.Reload(time);
        }

        /// <summary>
        /// Overrides InputPlayer's SwitchWeapon() in order to allow switching.
        /// </summary>
        /// <param name="newWeapon">The index of the weapon in the player's array of weapons.</param>
        protected override void SwitchWeapon(int newWeapon)
        {
            base.SwitchWeapon(newWeapon);

            if (newWeapon - 1 <= Items.Length && Items[newWeapon - 1] != null)
                CurWeapon = Items[newWeapon - 1];
        }

        /// <summary>
        /// Causes the player to take damage from a damager.
        /// </summary>
        /// <param name="damage">The amount of damage the player should take.</param>
        /// <param name="inflicter">The object causing the player to take damage.  This is used so the player can
        /// know by whom they were killed.</param>
        public override void TakeDamage(float damage, IDamager inflicter)
        {
            if (this.Health > 0)
                this.Health -= damage;

            if (this.Health <= 0)
            {
                Projectile p = (Projectile)inflicter;

                //update teams kills
                GameLogic g = (GameLogic)this.Game.Services.GetService(typeof(GameLogic));
                g.AwardKill(p.Creator);
                //Console.WriteLine("Player " + ClientID + " was killed by Player " + p.Creator);

                //update players kills
                IModelDBService mdb = (IModelDBService)this.Game.Services.GetService(typeof(IModelDBService));
                ProxyInputPlayer pip = (ProxyInputPlayer) mdb.getObject(p.Creator << 25);
                pip.NumKills++;

                //tell player to die
                Die();
            }
        }

        /// <summary>
        /// Kills this player and sends a death event.
        /// </summary>
        public override void Die()
        {
            IServerNetworking server = (IServerNetworking)Game.Services.GetService(typeof(INetworkingService));

            base.Die();

            server.sendEvent("Death", this.ClientID.ToString());
        }

        /// <summary>
        /// Causes the player to respond to a trigger such as walking into a flag.
        /// </summary>
        /// <param name="obj">The trigger to which this player should respond.</param>
        public override void RespondToTrigger(PhysicalObject obj)
        {
            //Console.WriteLine("ProxyPlayer is responding to a trigger.");

            // Check whether a Flag was trigger
            if (obj is Objects.Flag)
                // Check whether the Flag is not being carried
                if (((Objects.Flag)obj).Owner == null)
                {
                    // If this player is not carrying a Flag
                    if (Flag == null)
                    {
                        // Only pick up flags not owned by your team
                        if (((Objects.Flag) obj).Team != this.PlayerStats.YourTeam.TeamID)
                        {
                            Flag = (Objects.Flag)obj;
                            Flag.Owner = this;
                            //Console.WriteLine("ProxyPlayer picked up a flag!");
                        }
                    }
                    else
                    {
                        // Otherwise drop the Flag being carried: since there are only two flags, we can assume that the flag
                        // that was triggered is owned by your team.  Note:
                        // HACK: should check if it's at the spawn point and owned by your team, to be safe.
                        //Console.WriteLine("Dropping off a carried flag at another flag!");
                        GameLogic g = (GameLogic)this.Game.Services.GetService(typeof(GameLogic));
                        g.AwardCapture(this.ClientID);
                        this.NumCaptures++;
                        // Keep a reference to the flag so it can be sent
                        Objects.Flag flag = this.Flag;
                        // Drop the flag
                        Flag.GetDropped();
                        this.Flag = null;

                        IServerNetworking server = (IServerNetworking)Game.Services.GetService(typeof(INetworkingService));
                        // Send the dropped flag
                        server.sendThing(flag);
                    }
                }
        }


    }
}
