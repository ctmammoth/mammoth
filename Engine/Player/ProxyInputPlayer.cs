﻿using System;
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

        #endregion

        /// <summary>
        /// Constructs a new ProxyPlayer by associating the ProxyPlayer to a client.
        /// </summary>
        /// <param name="game">The game.</param>
        /// <param name="clientID">The ID of the client this player is simulating on the server</param>
        public ProxyInputPlayer(Game game, int clientID): base(game)
        {
            this.ClientID = clientID;
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
        protected override void Throw()
        {
            // Shoot if the player currently has a weapon
            if (CurWeapon != null)
            {
                //Calculate initial position of bullet to shoot from
                Vector3 direction = Vector3.Transform(Vector3.Forward, HeadOrient) * 1000.0f;
                direction.Normalize();
                Vector3 position = Position + (Vector3.Up * Height / 4.0f);
                Vector3 offset = Vector3.Multiply(direction, 2.0f);
                position = Vector3.Add(position, offset);

                CurWeapon.Shoot(position, direction, ID);
            }
        }
    }
}
