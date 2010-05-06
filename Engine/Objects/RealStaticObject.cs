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
    public class RealStaticObject : PhysicalObject, IEncodable, IRenderable
    {
        private Vector3 dimensions, localPosition; ///

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            Renderer r = (Renderer)this.Game.Services.GetService(typeof(IRenderService));

            r.DrawRenderable(this);
        }



        public Game Game
        {
            get;
            protected set;
        }

        public RealStaticObject(int id, ObjectParameters parameters, Game game)
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
                    case "Crate_Type":
                        Specialize(parameters.GetStringValue(attribute));
                        break;

                }

            }

            PhysicsManagerService physics = (PhysicsManagerService)this.Game.Services.GetService(typeof(IPhysicsManagerService));
            this.PositionOffset = new Vector3(0.0f, 0.0f, 0.0f);

            ActorDescription boxActorDesc = new ActorDescription();
            boxActorDesc.Shapes.Add(new BoxShapeDescription()
            {
                Size = new Vector3(dimensions.X, dimensions.Y, dimensions.Z),
                LocalPosition = localPosition
            });

            
            this.Actor = physics.CreateActor(boxActorDesc);
            this.Position = pos;            
            // this.
        }

        public RealStaticObject(int id, Game game)
        {
            this.Game = game;
            InitializeDefault(id);
        }

        private void Specialize(String attribute)
        {
            XmlHandler handler = new XmlHandler();
            handler.ChangeFile("../../../static_objects.xml");
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

        public override void InitializeDefault(int id)
        {
            ID = id;
            //pos.X = 0;
            //Position.Y = 0;
            //Position.Z = 0;
            dimensions.X = 0;
            dimensions.Y = 0;
            dimensions.Z = 0;
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
            return "Crate";
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
            get { return this.Position; }
        }

        Vector3 positionOffset = new Vector3();

        public Vector3 PositionOffset
        {
            get { return positionOffset; }
            set { positionOffset = value; }
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



        public Quaternion Orientation
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
