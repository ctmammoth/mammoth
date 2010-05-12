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
using Mammoth.Engine.Graphics;

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
        /// These objects are renderable, so they must be drawn. They are drawn in the usual
        /// way, using the renderer. 
        /// </summary>
        /// <param name="gameTime">The current gameTime</param>
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            Renderer r = (Renderer)this.Game.Services.GetService(typeof(IRenderService));

            r.DrawRenderable(this);
        }

        /// <summary>
        /// Constructor used to create an object with blank attributes that
        /// will be filled in during the encoding/decoding process.
        /// </summary>
        /// <param name="game"></param>
        public RealStaticObject(Game game) : base(game)
        {   
            
        }

        /// <summary>
        /// Main constructor, delegates to methods depending on whether this object is being 
        /// created locally or from the network
        /// </summary>
        /// <param name="id">ID number of this object</param>
        /// <param name="parameters">list of parameters to distinguish this object
        /// (null if it from the network)</param>
        /// <param name="game">the current name</param>
        /// <param name="isFromNetwork">if this object is from the network or not</param>
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


        /// <summary>
        /// Create a local object. This takes a list of parameters and initializes the object to,
        /// as opposed to getting these values from the network
        /// </summary>
        /// <param name="id">ID number to give this number</param>
        /// <param name="parameters">Values to initialize fields</param>
        /// <param name="game">the current game</param>
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

            // Make the object and register it with the modelDB
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
            
            
            

        }

        /// <summary>
        /// Creates an object from the network. The values of this object will
        /// be filled in with the decode, so this just gives it an ID number and
        /// the current game
        /// </summary>
        /// <param name="id">ID number to give the object.</param>
        /// <param name="game">The current game</param>
        public void CreateFromNetwork(int id, Game game)
        {
            this.Game = game;
            this.ID = id;
        }

        /// <summary>
        /// Fill in certain fields based on the type of the object by reading them
        /// in from an XML file
        /// </summary>
        /// <param name="attribute">The specialized type of the object</param>
        private void Specialize(String attribute)
        {
            XmlHandler handler = new XmlHandler();
            handler.ChangeFile("static_objects.xml");
            handler.GetElement("VARIANT", "NAME", attribute);
            this.TypeName = attribute;
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
                    case "POSITION_OFFSET":
                        HandlePositionOffset(handler);
                        break;

                }

            }

        }


        /// <summary>
        /// Sends crucial information over the network so that other players/ the server
        /// can know about it.
        /// </summary>
        /// <returns>a byte array with all of the necessary data to send across the 
        /// network to recreate this object</returns>
        public byte[] Encode()
        {
            Networking.Encoder tosend = new Networking.Encoder();
            //Console.WriteLine("Encoding a Static Object"); 
            tosend.AddElement("X", Position.X);
            tosend.AddElement("Y", Position.Y);
            tosend.AddElement("Z", Position.Z);
            tosend.AddElement("TypeName", TypeName);            
            
            // tosend.AddElement("Velocity", Velocity);
            return tosend.Serialize();
        }


        /// <summary>
        /// Reads data sent across the network and assigns the fields of this object based on that data
        /// </summary>
        /// <param name="serialized">data from the network</param>
        public void Decode(byte[] serialized)
        {

            Networking.Encoder props = new Networking.Encoder(serialized);

            Vector3 pos = new Vector3();
            if (props.UpdatesFor("Position"))
                pos = (Vector3)props.GetElement("Position", pos);
            
            if (props.UpdatesFor("TypeName"))
                TypeName = (String)props.GetElement("TypeName", TypeName);
            if (props.UpdatesFor("PositionOffset"))
                PositionOffset = (Vector3)props.GetElement("PositionOffset", PositionOffset);
            if (props.UpdatesFor("LocalPosition"))
                LocalPosition = (Vector3)props.GetElement("LocalPosition", LocalPosition);
            Specialize(TypeName);

            // PhysicsManagerService physics = (PhysicsManagerService)this.Game.Services.GetService(typeof(IPhysicsManagerService));


            ActorDescription boxActorDesc = new ActorDescription();
            boxActorDesc.Shapes.Add(new BoxShapeDescription()
            {
                Size = new Vector3(dimensions.X, dimensions.Y, dimensions.Z),
                LocalPosition = localPosition
            });
            boxActorDesc.GlobalPose = Matrix.CreateTranslation(pos + this.positionOffset);
            this.positionOffset = new Vector3();


            // this.Actor = physics.CreateActor(boxActorDesc);

            IModelDBService mdb = (IModelDBService)this.Game.Services.GetService(typeof(IModelDBService));
            mdb.registerObject(this);
            

            
        }


        /// <summary>
        /// Returns the type of this object, as a string. (Currently the same as the TypeOf(), but
        /// this won't always be the case
        /// </summary>
        /// <returns>String of the object type</returns>
        public override String getObjectType()
        {
            return "RealStaticObject";
        }

        /// <summary>
        /// Returns the specialized type of the object (e.g. Health Crate,, Wall Block, etc.)
        /// </summary>
        /// <returns></returns>
        public String GetTypeName()
        {
            return TypeName;
        }


        /// <summary>
        /// Reads the dimension information from an XML file and initializes the object's dimension
        /// field with this information
        /// </summary>
        /// <param name="handler">XML handler working on the current document</param>
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

        /// <summary>
        /// Reads the localPosition information from an XML file and initializes the object's dimension
        /// field with this information
        /// </summary>
        /// <param name="handler">XML handler working on the current document</param>
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

        /// <summary>
        /// Reads the position offset information from an XML file and initializes the object's dimension
        /// field with this information
        /// </summary>
        /// <param name="handler">XML handler working on the current document</param>
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

        /// <summary>
        /// Reads the model information from an XML file and initializes the object's dimension
        /// field with this information
        /// </summary>
        /// <param name="handler">XML handler working on the current document</param>
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
