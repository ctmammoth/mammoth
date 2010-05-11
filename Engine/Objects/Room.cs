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
    /// Represents a room in a fortress
    /// </summary>
    public class Room : PhysicalObject, IEncodable, IRenderable
    {
        private double width, height, length;
        private double x, y, z;
        private List<BaseObject> objectList;
        private String roomType;
        ActorDescription boxActorDesc;
        PhysicsManagerService physics;
        IModelDBService modelDB;
        PossibleObjects possible;
        List<IEncodable> items;

        /// <summary>
        /// Returns the type of this object. Currently is the 
        /// TypeOf, but that can be changed later
        /// </summary>
        /// <returns>The type of object that this is</returns>
        public override string getObjectType()
        {
            return "Room";
        }

        /// <summary>
        /// Inititialize a blank room, which can then later be decoded.
        /// </summary>
        /// <param name="id">ID # to give this room</param>
        /// <param name="game">The current game</param>
        public Room(int id, Game game)
            : base(game)
        {
            items = new List<IEncodable>();
            this.ID = id;
            this.Game = game;
            possible = new PossibleObjects();

            boxActorDesc = new ActorDescription()
            {
                /*BodyDescription = new BodyDescription()
                {
                    Mass = 1000.0f
                }*/
            };
            physics = (PhysicsManagerService)this.Game.Services.GetService(typeof(IPhysicsManagerService));
            modelDB = (IModelDBService)this.Game.Services.GetService(typeof(IModelDBService));
            
        }


        /// <summary>
        /// Send the crucial information about this room over the network so it can later be
        /// recreated as-is
        /// </summary>
        /// <returns>serialized Data to send across the network</returns>
        public byte[] Encode()
        {
            Networking.Encoder tosend = new Networking.Encoder();

            //IGameLogic g = (IGameLogic)this.Game.Services.GetService(typeof(IGameLogic));
            //int myID = ID >> 25;
            //GameStats = new GameStats(NumKills, NumCaptures, NumDeaths, myID, g);

            //Console.WriteLine("Encoding: " + GameStats.ToString());

            tosend.AddElement("x", x);
            tosend.AddElement("y", y);
            tosend.AddElement("z", z);
            tosend.AddElement("roomType", roomType);
            int numItems = items.Count;
            for (int i = 0; i < numItems; i++)
            {
                RealStaticObject item = (RealStaticObject)items.ElementAt(i);
                tosend.AddElement("X" + i, item.Position.X);
                tosend.AddElement("Y" + i, item.Position.Y);
                tosend.AddElement("Z" + i, item.Position.Z);
                tosend.AddElement("Type" + i, item.GetTypeName());
            }
            tosend.AddElement("numItems", numItems);            
            return tosend.Serialize();
        }


        /// <summary>
        /// Takes serialized info from the network and reconstructs a room based on this info
        /// </summary>
        /// <param name="data"></param>
        public void Decode (byte[] data)
        {
            Console.WriteLine("ROOM DECODING");
            Networking.Encoder props = new Networking.Encoder(data);

            ObjectParameters parameters = new ObjectParameters();

            if (props.UpdatesFor("x"))
                parameters.AddAttribute("X", ((double)props.GetElement("x", x)).ToString());
            if (props.UpdatesFor("y"))
                parameters.AddAttribute("Y", ((double)props.GetElement("y", y)).ToString());
            if (props.UpdatesFor("z"))
                parameters.AddAttribute("Z", ((double)props.GetElement("z", z)).ToString());
            if (props.UpdatesFor("roomType"))
                parameters.AddAttribute("Special_Type", (String)props.GetElement("roomType", roomType));
            int numItems = 0;
            if (props.UpdatesFor("numItems"))
               numItems = (int)props.GetElement("numItems", numItems);
            for (int i = 0; i < numItems; i++)
            {
                BaseObject item = null;
                item = new RealStaticObject(this.Game);

                ObjectParameters parameters2 = new ObjectParameters();
                parameters2.AddAttribute("X", ((float)props.GetElement("X" + i,"0")).ToString() );
                parameters2.AddAttribute("Y", ((float)props.GetElement("Y" + i, "0")).ToString() );
                parameters2.AddAttribute("Z", ((float)props.GetElement("Z" + i, "0")).ToString() );
                parameters2.AddAttribute("Special_Type", (String)props.GetElement("Type" + i, "Stair_Room"));

                item = ObjectFactories.CreateObject("Static_Object",modelDB.getNextOpenID(),parameters2,Game);                              
                modelDB.registerObject(item);
            }

            SpawnRoomFromNetwork(parameters);
        }

        /// <summary>
        /// Takes  decoded room  data from the Network, and fills in the rest of the necessary parameters
        /// </summary>
        /// <param name="parameters">Parameters taken from the decode</param>
        protected void SpawnRoomFromNetwork(ObjectParameters parameters)
        {
            
            boxActorDesc = new ActorDescription()
            {
                /*BodyDescription = new BodyDescription()
                {
                    Mass = 1000.0f
                }*/
            };

            
            items = new List<IEncodable>();
            physics = (PhysicsManagerService)this.Game.Services.GetService(typeof(IPhysicsManagerService));
            modelDB = (IModelDBService)this.Game.Services.GetService(typeof(IModelDBService));
            foreach (String attribute in parameters.GetAttributes())
            {
                switch (attribute)
                {
                    case "X":
                        x = parameters.GetDoubleValue(attribute);
                        break;
                    case "Y":
                        y = parameters.GetDoubleValue(attribute);
                        break;
                    case "Z":
                        z = parameters.GetDoubleValue(attribute);
                        break;
                    case "Special_Type":
                        SpecializeFromServer(parameters.GetStringValue(attribute));
                        break;
                }
            }
            BuildWalls(x, y, z);
            this.Actor = physics.CreateActor(boxActorDesc);
            

        }

        /// <summary>
        /// Contruct a room locally, to the specification of the given parameters
        /// </summary>
        /// <param name="id">ID # to give this room</param>
        /// <param name="parameters">Values to give its fields</param>
        /// <param name="game">the current game</param>
        public Room(int id, ObjectParameters parameters, Game game)
            : base(game)
        {
            possible = new PossibleObjects();
            boxActorDesc = new ActorDescription()
            {
                /*BodyDescription = new BodyDescription()
                {
                    Mass = 1000.0f
                }*/
            };

            this.Game = game;
            this.ID = id;
            items = new List<IEncodable>();
            physics = (PhysicsManagerService)this.Game.Services.GetService(typeof(IPhysicsManagerService));
            modelDB = (IModelDBService)this.Game.Services.GetService(typeof(IModelDBService));
            foreach (String attribute in parameters.GetAttributes()) 
            {
                switch(attribute) 
                {
                    case "X":
                        x = parameters.GetDoubleValue(attribute);
                        break;
                    case "Y":
                        y = parameters.GetDoubleValue(attribute);
                        break;
                    case "Z":
                        z = parameters.GetDoubleValue(attribute);
                        break;
                    case "Special_Type":
                        Specialize(parameters.GetStringValue(attribute));
                        break;
                }
            }



            BuildWalls(x, y, z);

            // TODO: put something in the corner to shoot you through that hole in the ceiling.
            /*ForceFieldLinearKernelDescription forceToApply = new ForceFieldLinearKernelDescription()
            {
                Constant = new Vector3(0.0f, 10.0f, 0.0f),
            };

            ForceFieldDescription holeForceField = new ForceFieldDescription()
            {
                Kernel = forceToApply,
                Pose = Matrix.CreateTranslation(new Vector3(3.0f, 0.0f, 3.0f))
            };

            BoxForceFieldShapeDescription boostBoxShapeDesc = new BoxForceFieldShapeDescription()
            {
                Size = new Vector3(0.5f, 0.5f, 0.5f)
            };

            ForceFieldShape boostShape = new ForceFieldShape()
            {
                Pose = Matrix.CreateTranslation(new Vector3(3.0f, 0.0f, 3.0f))
            };
            boostShape.ShapeGroup.CreateShape(boostBoxShapeDesc);*/

            this.Actor = physics.CreateActor(boxActorDesc);
        }

        /// <summary>
        /// Given the special type of room, differentiate this
        /// room based on that type
        /// </summary>
        /// <param name="attribute">The special type of room this is</param>
        private void Specialize(String attribute)
        {
            XmlHandler handler = new XmlHandler();
            handler.ChangeFile("rooms.xml");
            handler.GetElement("ROOM", "NAME", attribute);
            roomType = attribute;
            while (!handler.IsClosingTag("ROOM"))
            {
                handler.GetNextElement();
                String name = handler.GetElementName();
                switch (name)
                {
                    case "ITEMS":
                        HandleItems(handler);
                        break;
                    case "POSSIBLE":
                        HandlePossible(handler);
                        break;
                    case "PARAMETERS":
                        HandleParameters(handler);
                        break;
                }

            }

        }

        private static int team1height = 1;
        private static int team2height = 1;


        /// <summary>
        /// Creates a room on the tower of rooms that comprises each fortress,
        /// depending on which team the player is on
        /// </summary>
        /// <param name="team">Which teams fortress to augment</param>
        /// <param name="modelDB">The current modelDB in the game</param>
        /// <param name="game">the current game</param>
        /// <returns>a new room on top of the tower</returns>
        public static Room NewTowerRoom(String team, IModelDBService modelDB, Game game)
        {
            ObjectParameters stairRoom = new ObjectParameters();
            switch (team)
            {
                    //TODO: Dont hardcode
                case ("Team 1"):
                    stairRoom.AddAttribute("X", "-50");
                    stairRoom.AddAttribute("Y", (21 * team1height - 23).ToString());
                    stairRoom.AddAttribute("Z", "-50");
                    team1height++;
                    break;
                
                case ("Team 2"):
                    stairRoom.AddAttribute("X", "193");
                    stairRoom.AddAttribute("Y", (21 * team2height - 29).ToString());
                    stairRoom.AddAttribute("Z", "118");
                    team2height++;
                    break;     
                    

            }
            
            stairRoom.AddAttribute("Special_Type", "STAIR_ROOM");
            return new Room(modelDB.getNextOpenID(), stairRoom, game);
        }


        /// <summary>
        /// Specialize the room based on its type, but skip the 
        /// dynamic generation step (as we are getting that informaton in the decode)
        /// </summary>
        /// <param name="attribute">Type of room that this is</param>
        private void SpecializeFromServer(String attribute)
        {
            XmlHandler handler = new XmlHandler();
            handler.ChangeFile("rooms.xml");
            handler.GetElement("ROOM", "NAME", attribute);
            roomType = attribute;
            while (!handler.IsClosingTag("ROOM"))
            {
                handler.GetNextElement();
                String name = handler.GetElementName();
                switch (name)
                {
                    case "ITEMS":
                        HandleItems(handler);
                        break;                   
                }

            }

        }


        /// <summary>
        /// Based on the type of room that this is, generate the items
        /// that must be in this room type, based on the xml
        /// </summary>
        /// 
        /// <param name="handler">XML handler currently  reading the room info</param>
        private void HandleItems(XmlHandler handler)
        {
            handler.GetNextElement();
            while (!handler.IsClosingTag("ITEMS"))
            {
                String itemType = null, specialType = null;
                String X = null, Y = null, Z = null;

                ObjectParameters parameters = handler.GetAttributes();
                foreach (String attribute in parameters.GetAttributes())
                {
                    switch (attribute)
                    {
                        case "TYPE":
                            itemType = parameters.GetStringValue(attribute);
                            break;
                        case "Special_Type":
                            specialType = parameters.GetStringValue(attribute);
                            break;
                        case "X":
                            X = (this.x +  parameters.GetDoubleValue(attribute)).ToString();
                            break;
                        case "Y":
                            Y = (this.y + parameters.GetDoubleValue(attribute)).ToString();
                            break;
                        case "Z":
                            Z = (this.z + parameters.GetDoubleValue(attribute)).ToString();
                            break;
                    }
                }

                ObjectParameters itemParameters = new ObjectParameters();
                itemParameters.AddAttribute("X", X);
                itemParameters.AddAttribute("Y", Y);
                itemParameters.AddAttribute("Z", Z);
                itemParameters.AddAttribute("Special_Type", specialType);

                BaseObject item = ObjectFactories.CreateObject(itemType, modelDB.getNextOpenID(), itemParameters, this.Game);
                //Console.WriteLine(item.Position);
                modelDB.registerObject(item);
                handler.GetNextElement();
            }

            
            
            
        }

        /// <summary>
        /// Based on the type of room that this is, generate information
        /// to construct all of the items that can potentially be created, but don't actually
        /// create them yet
        /// 
        /// </summary>
        /// <param name="handler">XML handler currently  reading the room info</param>
        private void HandlePossible(XmlHandler handler)
        {
            handler.GetNextElement();
            while (!handler.IsClosingTag("POSSIBLE"))
            {
                ObjectParameters parameters = handler.GetAttributes();
                String type = parameters.GetStringValue("Special_Type");
                possible.AddPossibleObject(type, parameters);
                handler.GetNextElement();
            }

        }

        /// <summary>
        /// Based on the type of room that this is, read the parameters for 
        /// maximum and minimum of each type, then randomly pick
        /// a number in this range of those objects and create them
        /// 
        /// </summary>
        /// <param name="handler">XML handler currently  reading the room info</param>
        private void HandleParameters(XmlHandler handler)
        {
            handler.GetNextElement();
            while (!handler.IsClosingTag("PARAMETERS"))
            {
                String type = null;
                ObjectParameters parameters = null;
                if (handler.GetElementName().Equals("RANGE"))
                {
                    parameters = handler.GetAttributes();
                    type = parameters.GetStringValue("TYPE");
                    int min = (int) parameters.GetDoubleValue("MIN");
                    int max = (int)parameters.GetDoubleValue("MAX");
                    Random random = new Random();
                    int num = random.Next(max) + min;
                    for (int i = 0; i < num; i++)
                    {
                        ObjectParameters possibleParams = possible.GetRandomParameter(type);
                        possibleParams.ReplaceAttribute("X", (possibleParams.GetDoubleValue("X") + this.x).ToString());
                        possibleParams.ReplaceAttribute("Y", (possibleParams.GetDoubleValue("Y") + this.y).ToString());
                        possibleParams.ReplaceAttribute("Z", (possibleParams.GetDoubleValue("Z") + this.z).ToString());

                        

                        IEncodable obj = (IEncodable) ObjectFactories.CreateObject(possibleParams.GetStringValue("TYPE"), modelDB.getNextOpenID(), possibleParams, this.Game);
                        items.Add(obj);
                        modelDB.registerObject((BaseObject)obj);

                    }

                }
                handler.GetNextElement();
                
                
            }
        }

        

       
        
        /// <summary>
        /// Create the walls and ceiling for the room. This will eventually
        /// be in XML as to create differently sized rooms, but for now, each 
        /// room has the same skeleton (but different innards)
        /// </summary>
        /// <param name="X">X position of room</param>
        /// <param name="Y">Y position of room</param>
        /// <param name="Z">Z position of room</param>
        public void BuildWalls(Double X, Double Y, Double Z)
        {
            BuildWall("X", X + 3, "Z", Z + 18, Y);
            BuildWall("X", X + 3, "Z", Z - 3, Y);
            BuildWall("Z", Z + 0, "X", X + 0, Y);
            BuildWall("Z", Z + 0, "X", X + 21, Y);
            BuildCeiling(X,Y,Z);
            
            // this.Position = new Vector3((float)X, (float)Y, (float)Z);
        }

        /// <summary>
        /// Build a wall along a certain axis
        /// </summary>
        /// <param name="alongAxis">Build along this access</param>
        /// <param name="alongOffset">Position of the along axis to start </param>
        /// <param name="oppositeAxis">Opposite of along axis (X or Z)</param>
        /// <param name="oppositeOffset">POsition on Opposite Axis to start</param>
        /// <param name="height">height to start building the wall at</param>
        public void BuildWall(String alongAxis, double alongOffset, String oppositeAxis, double oppositeOffset, double height)
        { 
            ObjectParameters parameters;

            for (int i = 0; i < 36; i++)
            {
                parameters = new ObjectParameters();
                Double number1 = (3 * (i / 6));
                Double number2 = (3 * (i % 6));


                parameters.AddAttribute(alongAxis, (alongOffset + number1).ToString());
                parameters.AddAttribute("Y", (number2+(height)).ToString() );
                parameters.AddAttribute(oppositeAxis, oppositeOffset.ToString());
                parameters.AddAttribute("Special_Type", "BRICK");


                if (!((i>11&&i<15)||(i>17&&i<21)))
                {
                    int crateId = modelDB.getNextOpenID();
                    WallBlock block = (WallBlock)ObjectFactories.CreateObject("WallBlock", crateId, parameters, this.Game);
                    modelDB.registerObject(block);

                    
                    boxActorDesc.Shapes.Add(new BoxShapeDescription()
                    {
                        Size = block.Dimensions,
                        LocalPosition = block.LocalPosition + block.NonPhysicalPosition
                    });

                }

            }
        }

        /// <summary>
        /// Builds a ceiling of blocks at the proper position (X,Y,Z) is 
        /// upper left hand corner
        /// </summary>
        /// <param name="X">X value</param>
        /// <param name="Y">Y Value</param>
        /// <param name="Z">Z value</param>
        public void BuildCeiling(Double X, Double Y, Double Z)
        {
            ObjectParameters parameters = new ObjectParameters();
            for (int i = 0; i < 64; i++)
            {
                parameters = new ObjectParameters();
                Double number1 = (3 * (i / 8));
                Double number2 = (3 * (i % 8));


                parameters.AddAttribute("X", (X + number1).ToString());
                parameters.AddAttribute("Y", (Y+18.0f).ToString());
                parameters.AddAttribute("Z", (Z + number2 - 3).ToString());
                parameters.AddAttribute("Special_Type", "BRICK");

                
                if (!(i==0||i==7||i==63||i==56||i==9||i==54||i==53||i==52||i==51||i==50))
                {
                    int crateId = modelDB.getNextOpenID();
                    WallBlock crate1 = (WallBlock)ObjectFactories.CreateObject("WallBlock", crateId, parameters, this.Game);
                    modelDB.registerObject(crate1);

                    if (boxActorDesc == null)
                    {
                        Console.WriteLine(); // breakpoint
                    }
                    
                    if (boxActorDesc.Shapes == null)
                    {
                        Console.WriteLine(); // breakpoint
                    }

                    
                    boxActorDesc.Shapes.Add(new BoxShapeDescription()
                    {
                        Size = crate1.Dimensions,
                        LocalPosition = crate1.LocalPosition + crate1.NonPhysicalPosition
                    });
                    


                }
            }

        }

        /// <summary>
        /// Used to store information about objects that can possibly be created, sorted
        /// by item type, so you can specify parameters for how many of each type
        /// </summary>
        private class PossibleObjects
        {
            private Dictionary<String, List<ObjectParameters>> typeLists;            
            List<ObjectParameters> lis = new List<ObjectParameters>();

            public PossibleObjects()
            {
                typeLists = new Dictionary<string, List<ObjectParameters>>();

            }
            
            /// <summary>
            /// Add description for a new object of the given type
            /// </summary>
            /// <param name="type">Type of object</param>
            /// <param name="parameters">Object info</param>
            public void AddPossibleObject(String type, ObjectParameters parameters) 
            {
                if (!typeLists.ContainsKey(type)) 
                {
                    typeLists.Add(type,new List<ObjectParameters>());
                }
                List<ObjectParameters> parameterList = new List<ObjectParameters>();
                typeLists.TryGetValue(type, out parameterList);
                parameterList.Add(parameters); 
               
            }

            /// <summary>
            /// Gets a random description from the list of 
            /// descriptions for that type, and remove it from the list
            /// </summary>
            /// <param name="type">Type of object you want</param>
            /// <returns>Randomly chosen descritpion of this type</returns>
            public ObjectParameters GetRandomParameter(String type)
            {
                List<ObjectParameters> parameterList = new List<ObjectParameters>();
                typeLists.TryGetValue(type, out parameterList);
                int size = parameterList.Count();
                Random random = new Random();
                int index = random.Next(size);
                ObjectParameters result = parameterList.ElementAt(index);
                parameterList.RemoveAt(index);
                return result;
            }




        }




        #region IRenderable Members

        public Microsoft.Xna.Framework.Vector3 PositionOffset
        {
            get { return Vector3.Zero; }
        }

        public Microsoft.Xna.Framework.Graphics.Model Model3D
        {
            get { throw new NotImplementedException(); }
        }

        #endregion
    }
}
