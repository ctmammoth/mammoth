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
            //Calculate the unit vector pointing the direction in which the bullet will be thrown.
            Vector3 forward = Vector3.Transform(Vector3.Forward, HeadOrient) * 1000.0f;
            forward.Normalize();

            //Calculate initial position of bullet to shoot from
            Vector3 position = Position + (Vector3.Up * Height / 4.0f);
            Vector3 offset = Vector3.Multiply(forward, 2.0f);
            position = Vector3.Add(position, offset);

            // Make sure the bullet isn't spawned in the player: shift it by a bit
            Bullet b = new Bullet(Game, position, forward, ID >> 25);

            // Give this projectile an ID and register it into the ModelDB
            IModelDBService modelDB = (IModelDBService)this.Game.Services.GetService(typeof(IModelDBService));
            b.ID = modelDB.getNextOpenID();
            modelDB.registerObject(b);

            //Testing prints
            Console.WriteLine("Position vec: " + position);
            Console.WriteLine("Throwing bullet with position: " + b.Position);
            Console.WriteLine("Player position: " + Position);

            //Send the bullet to all other clients
            IServerNetworking net = (IServerNetworking)this.Game.Services.GetService(typeof(INetworkingService));
            net.sendThing(b);
        }
    }
}
