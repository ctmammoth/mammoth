using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using StillDesign.PhysX;
using Mammoth.Engine.Physics;

namespace Mammoth.Engine.Objects
{
    /// <summary>
    /// Represents a flag that can be picked up by a player.
    /// </summary>
    public class Flag : PhysicalObject//, IRenderable
    {
        public Flag(Game game, Vector3 initialPosition)
            : base(game)
        {
            // Give this a sphere shape trigger
            SphereShapeDescription sDesc = new SphereShapeDescription()
            {
                Radius = 10.0f,
                Flags = ShapeFlag.TriggerEnable
            };

            // Make a body: flags should be kinematic since they should get their positions from the player carrying them.
            BodyDescription bDesc = new BodyDescription()
            {
                Mass = 10.0f,
                BodyFlags = BodyFlag.Kinematic
            };

            ActorDescription aDesc = new ActorDescription()
            {
                Shapes = { sDesc },
                BodyDescription = bDesc
            };

            IPhysicsManagerService physics = (IPhysicsManagerService)game.Services.GetService(typeof(IPhysicsManagerService));

            this.Actor = physics.CreateActor(aDesc, this);

            // Set the position to whereever the flag should be constructed
            this.Position = initialPosition;
        }
    }
}
