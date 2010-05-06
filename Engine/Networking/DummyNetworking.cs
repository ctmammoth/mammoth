using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Mammoth.Engine.Input;

namespace Mammoth.Engine.Networking
{
    public abstract class DummyNetworking : NetworkComponent
    {
        public DummyNetworking(Game game)
            : base(game)
        {

        }

        public override NetworkComponent.NetworkingType getType()
        {
            return NetworkingType.DUMMY;
        }

        public override void Update(GameTime gameTime)
        {
            return;
        }

        public override int ClientID
        {
            get { return 1; }
        }
    }

    public class DummyClientNetworking : DummyNetworking, IClientNetworking
    {
        #region IClientNetworking Members

        public DummyClientNetworking(Game game)
            : base(game)
        {
            
        }

        public void sendThing(IEncodable toSend)
        {
            return;
        }

        /// <summary>
        /// If the player shoots, creates a bullet in the correct place.
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            IInputService inputServer = (IInputService)this.Game.Services.GetService(typeof(IInputService));
            InputState state = inputServer.States.Peek();

            InputPlayer player = (InputPlayer)this.Game.Services.GetService(typeof(InputPlayer));

            if (state.KeyPressed(InputType.Fire))
            {
                IModelDBService modelDB = (IModelDBService)this.Game.Services.GetService(typeof(IModelDBService));
                Vector3 forward = Vector3.Transform(Vector3.Forward, player.HeadOrient) * 1000.0f;
                forward.Normalize();
                Vector3 position = player.Position + (Vector3.Up * player.Height / 4.0f);
                Vector3 offset = Vector3.Multiply(forward, 2.0f);
                position = Vector3.Add(position, offset);

                Bullet b = new Bullet(Game, position, forward, player.ID >> 25);

                // Give this projectile an ID
                b.ID = modelDB.getNextOpenID();
                modelDB.registerObject(b);
            }
        }

        public void joinGame()
        {
            return;
        }

        public void quitGame()
        {
            return;
        }

        #endregion
    }

    public class DummyServerNetworking : DummyNetworking, IServerNetworking
    {
        #region IServerNetworking Members

        public DummyServerNetworking(Game game)
            : base(game)
        {

        }

        public void sendThing(IEncodable toSend, int target)
        {
            return;
        }

        public void sendThing(IEncodable toSend)
        {
            return;
        }

        public void sendToAllBut(IEncodable toSend, int excludeTarget)
        {
            return;
        }

        public Queue<Mammoth.Engine.Input.InputState> getInputStateQueue(int playerID)
        {
            return null;
        }

        public void createSession()
        {
            return;
        }

        public void endGame()
        {
            return;
        }

        #endregion
    }
}
