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
    /// Represents an object that has a model and single physical Actor
    /// 
    /// </summary>
    public class RealStaticObject : PhysicalObject, IEncodable, IRenderable
    {
        private Vector3 dimensions, localPosition; 
        private String typeName;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameTime">The current gameTime</param>
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            Renderer r = (Renderer)this.Game.Services.GetService(typeof(IRenderService));

            r.DrawRenderable(this);
        }

        public RealStaticObject(int id, ObjectParameters parameters, Game game, bool isFromNetwork)
            : base(game)
        {
            if (isFromNetwork)
            {
                CreateFromNetwork(id, game);
            }
            else
            {
                CreateFromLocal(id, parameters, game);
            }


        }


        public void CreateFromLocal(int id, ObjectParameters parameters, Game game)
        {
            this.ID = id;
            this.Game = game;
            Vector3 pos = new Vector3();
            this.PositionOffset = new Vector3(0.0f, 0.0f, 0.0f);
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

            PhysicsManagerService physics = (PhysicsManagerService)this.Game.Services.GetService(typeof(IPhysicsManagerService));
            

            ActorDescription boxActorDesc = new ActorDescription();
            boxActorDesc.Shapes.Add(new BoxShapeDescription()
            {
                Size = new Vector3(dimensions.X, dimensions.Y, dimensions.Z),
                LocalPosition = localPosition                 
            });
            boxActorDesc.GlobalPose = Matrix.CreateTranslation(pos + this.positionOffset);
            this.positionOffset = new Vector3();

            
            this.Actor = physics.CreateActor(boxActorDesc);
            
            
            // this.Position = pos;            
            // this.
        }

        public void CreateFromNetwork(int id, Game game)
        {
            this.Game = game;
            this.ID = id;
        }

        private void Specialize(String attribute)
        {
            XmlHandler handler = new XmlHandler();
            handler.ChangeFile("static_objects.xml");
            handler.GetElement("VARIANT", "NAME", attribute);
            while (!handler.IsClosingTag("VARIANT"))
            {
                handler.GetNextElement();
                String name = handler.GetElementName();
                this.TypeName = name;
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
                    case "POSITION_OFFSET":
                        HandlePositionOffset(handler);
                        break;

                }

            }

        }

        public byte[] Encode()
        {
            Networking.Encoder tosend = new Networking.Encoder();
            Console.WriteLine("Encoding a Static Object");

            tosend.AddElement("Position", Position);
            tosend.AddElement("Orientation", Orientation);
            tosend.AddElement("TypeName", TypeName);
            tosend.AddElement("PositionOffset", PositionOffset);
            tosend.AddElement("LocalPosition", LocalPosition);
            
            // tosend.AddElement("Velocity", Velocity);

            return tosend.Serialize();
        }

        public void Decode(byte[] serialized)
        {

            Networking.Encoder props = new Networking.Encoder(serialized);

            if (props.UpdatesFor("Position"))
                Position = (Vector3)props.GetElement("Position", Position);
            if (props.UpdatesFor("Orientation"))
                Orientation = (Quaternion)props.GetElement("Orientation", Orientation);
            if (props.UpdatesFor("TypeName"))
                Position = (Vector3)props.GetElement("TypeName", TypeName);
            if (props.UpdatesFor("PositionOffset"))
                Orientation = (Quaternion)props.GetElement("PositionOffset", PositionOffset);
            if (props.UpdatesFor("LocalPosition"))
                Position = (Vector3)props.GetElement("LocalPosition", LocalPosition);
            
        }


        public override String getObjectType()
        {
            return "RealStaticObject";
        }

        public String GetTypeName()
        {
            return TypeName;
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

        private void HandlePositionOffset(XmlHandler handler)
        {
            this.PositionOffset = new Vector3();
            ObjectParameters parameters = handler.GetAttributes();
            foreach (String attribute in parameters.GetAttributes())
            {
                switch (attribute)
                {
                    case "X":
                        this.positionOffset.X = (float)parameters.GetDoubleValue(attribute);                        
                        break;
                    case "Y":
                        this.positionOffset.Y = (float)parameters.GetDoubleValue(attribute);
                        break;
                    case "Z":
                        this.positionOffset.Z = (float)parameters.GetDoubleValue(attribute);
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

        public String TypeName
        {
            get { return typeName; }
            set { typeName = value; }
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
