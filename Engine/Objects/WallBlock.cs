using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using StillDesign.PhysX;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.GamerServices;

using Mammoth.Engine.Input;
using Mammoth.Engine.Interface;
using Mammoth.Engine.Physics;
using Mammoth.Engine.Networking;



namespace Mammoth.Engine
{
    /// <summary>
    /// Special object case that has a model and can be drawn, but not an actor
    /// as it is part of the one actor of a room.
    /// </summary>
    public class WallBlock : PhysicalObject, IEncodable, IRenderable
    {
        private Vector3 dimensions, localPosition, nonPhysicalPosition; ///

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            Renderer r = (Renderer)this.Game.Services.GetService(typeof(IRenderService));

            r.DrawRenderable(this);
        }

        /// <summary>
        /// Create a wallblock with the given parameters and id
        /// </summary>
        /// <param name="id">ID # to give this</param>
        /// <param name="parameters">Describes the fields of the wallblock</param>
        /// <param name="game">The current gam,e</param>
        public WallBlock(int id, ObjectParameters parameters, Game game)
            : base(game)
        {
            this.ID = id;
            this.Game = game;
            Vector3 pos = new Vector3();
            foreach (String attribute in parameters.GetAttributes())
            {
                
                switch (attribute)
                {
                    case "X":
                        pos.X = (float)parameters.GetDoubleValue(attribute);
                        break;
                    case "Y":
                        pos.Y = (float)parameters.GetDoubleValue(attribute);
                        break;
                    case "Z":
                        pos.Z = (float)parameters.GetDoubleValue(attribute);
                        break;
                    case "Special_Type":
                        Specialize(parameters.GetStringValue(attribute));
                        break;

                }
                
            }

            PhysicsManagerService physics = (PhysicsManagerService) this.Game.Services.GetService(typeof(IPhysicsManagerService));
            this.PositionOffset = new Vector3(0.0f, 0.0f, 0.0f);

            /*ActorDescription boxActorDesc = new ActorDescription();
            boxActorDesc.Shapes.Add(new BoxShapeDescription()
            {
                Size = new Vector3(dimensions.X, dimensions.Y, dimensions.Z),
                LocalPosition = localPosition
            });

            
            this.Actor = physics.CreateActor(boxActorDesc);
            this.Position = pos;*/
            this.NonPhysicalPosition = pos;
            // this.
        }

        public WallBlock(int id, Game game)
            : base(game)
        {
            this.Game = game;
            this.ID = id;
        }

        /// <summary>
        /// Populate certain values from an XML file based on type of this object
        /// </summary>
        /// <param name="attribute">Type of wallblock</param>
        private void Specialize(String attribute)
        {
            XmlHandler handler = new XmlHandler();
            handler.ChangeFile("static_objects.xml");
            handler.GetElement("VARIANT", "NAME", attribute);
            while (!handler.IsClosingTag("VARIANT"))
            {
                handler.GetNextElement();
                String name = handler.GetElementName();
                switch (name)
                {
                    case "DIMENSION":
                        HandleDimension(handler);
                        break;
                    case "MODEL":
                        HandleModel(handler);
                        break;
                    case "LOCAL_POSITION":
                        HandleLocalPosition(handler);
                        break;

                }

            }

        }

        public byte[] Encode()
        {
            Networking.Encoder tosend = new Networking.Encoder();

            // tosend.AddElement("Position", Position);
            //tosend.AddElement("Orientation", Orientation);
            //tosend.AddElement("Velocity", Velocity);

            return tosend.Serialize();
        }

        public void Decode(byte[] serialized)
        {
            Networking.Encoder props = new Networking.Encoder(serialized);

            //Position = (Vector3)props.GetElement("Position");
            //Orientation = (Quaternion)props.GetElement("Orientation");
            //Velocity = (Vector3)props.GetElement("Velocity");
        }


        public override String getObjectType()
        {
            return "WallBlock";
        }

        private void HandleDimension(XmlHandler handler)
        {
            ObjectParameters parameters = handler.GetAttributes();
            foreach (String attribute in parameters.GetAttributes())
            {
                switch (attribute)
                {
                    case "WIDTH":
                        dimensions.X = (float)parameters.GetDoubleValue(attribute);
                        break;
                    case "HEIGHT":
                        dimensions.Y = (float)parameters.GetDoubleValue(attribute);
                        break;
                    case "LENGTH":
                        dimensions.Z = (float)parameters.GetDoubleValue(attribute);
                        break;
                }
            }
        }

        private void HandleLocalPosition(XmlHandler handler)
        {
            localPosition = new Vector3();
            ObjectParameters parameters = handler.GetAttributes();
            foreach (String attribute in parameters.GetAttributes())
            {
                switch (attribute)
                {
                    case "X":
                        localPosition.X = (float)parameters.GetDoubleValue(attribute);
                        break;
                    case "Y":
                        localPosition.Y = (float)parameters.GetDoubleValue(attribute);
                        break;
                    case "Z":
                        localPosition.Z = (float)parameters.GetDoubleValue(attribute);
                        break;
                }
            }
        }

        private void HandleModel(XmlHandler handler)
        {
            String modelPath = handler.reader.ReadElementContentAsString();
            Renderer r = (Renderer)this.Game.Services.GetService(typeof(IRenderService));
            this.Model3D = r.LoadModel(modelPath);
            //TODO: Actually make this a model
        }







        #region IRenderable Members

        Vector3 IRenderable.Position
        {
            get { return this.NonPhysicalPosition; }
        }

        Vector3 positionOffset = new Vector3();

        public Vector3 PositionOffset
        {
            get { return positionOffset; }
            set {positionOffset = value;}
        }

        public Vector3 Dimensions
        {
            get { return dimensions; }
            set { dimensions = value; }
        }

        public Vector3 LocalPosition
        {
            get { return localPosition; }
            set { localPosition = value; }
        }

        public Vector3 NonPhysicalPosition
        {
            get { return nonPhysicalPosition; }
            set { nonPhysicalPosition = value; }
        }

        public override Quaternion Orientation
        {
            get { return new Quaternion(); }
        }

        public Model Model3D
        {
            get;
            set;
        }

        #endregion

        public override void CollideWith(PhysicalObject obj)
        {
            throw new NotImplementedException();
        }
    }
} ///////