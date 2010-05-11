using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StillDesign.PhysX;

using Mammoth.Engine.Physics;
using Mammoth.Engine.Objects;

namespace Mammoth.Engine
{
    /// <summary>
    /// A client-side representation of other players connected to the same server.
    /// </summary>
    class RemotePlayer : Player
    {
        #region Fields

        Texture2D nameTexture;

        #endregion

        /// <summary>
        /// Loads Models for these players and initializes PhysX for them.
        /// </summary>
        /// <param name="game">The game.</param>
        public RemotePlayer(Game game) : base(game)
        {
            //Load their model
            Renderer r = (Renderer)this.Game.Services.GetService(typeof(IRenderService));
            this.Model3D = r.LoadModel("soldier-low-poly");

            //Initialize PhysX for the RemotePlayer so it can be interacted with (collision, etc.)
            InitializePhysX();
        }

        /// <summary>
        /// Sets up the PhysX of a player and "syncs" with model.
        /// </summary>
        public void InitializePhysX()
        {
            //Gives the player a bounding box and a physical description
            IPhysicsManagerService physics = (IPhysicsManagerService)this.Game.Services.GetService(typeof(IPhysicsManagerService));
            ControllerDescription desc = new CapsuleControllerDescription(1, this.Height - 2.0f)
            {
                UpDirection = Axis.Y,
                Position = Vector3.UnitY * (this.Height - 1.0f) / 2.0f
            };

            //Describe the offset of the model to sync model center with PhysX Controller center
            this.PositionOffset = -1.0f * desc.Position;

            //Set controllet to one defined above
            this.Controller = physics.CreateController(desc, this);
        }

        /// <summary>
        /// Draws the remote player.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            IRenderService r = (IRenderService)this.Game.Services.GetService(typeof(IRenderService));

            if (nameTexture == null)
            {
                string team = this.PlayerStats.YourTeam;
                Color teamColor = Color.Black;
                if (team == "Team 1")
                    teamColor = Color.Red;
                else if (team == "Team 2")
                    teamColor = Color.Blue;

                Color transTeamColor = teamColor;
                transTeamColor.A = 0;
                // Render the name to a texture.
                nameTexture = r.RenderFont("Noob" + (this.ID >> 25), Vector2.Zero, teamColor, transTeamColor, r.DefaultFont, SpriteEffects.FlipHorizontally);
            }

            //Render the RemotePlayer
            r.DrawRenderable(this);

            // DRAW NAMES, NOOB!
            // This could probably be more efficient/better/something.
            r.DrawTexturedBillboard(this.Position + this.PositionOffset + new Vector3(0.0f, this.Height - 0.5f, 0.0f),
                                    new Vector2(nameTexture.Width, nameTexture.Height) / 40.0f,
                                    nameTexture);
        }

        public override void Dispose()
        {
            //Console.WriteLine("Disposing remote player with id: " + this.ID);
            //Console.WriteLine(new StackTrace());
            base.Dispose();
        }

        #region Properties
        private Gun CurWeapon
        {
            get;
            set;
        }
        #endregion
    }
}
