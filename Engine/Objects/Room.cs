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

        public override string getObjectType()
        {
            return "Room";
        }

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
            //tosend.AddElement("items", items);
            return tosend.Serialize();
        }

        public void Decode (byte[] data)
        {
            Networking.Encoder props = new Networking.Encoder(data);

            if (props.UpdatesFor("x"))
                x = (float)props.GetElement("x", x);
            if (props.UpdatesFor("y"))
                y = (float)props.GetElement("y", y);
            if (props.UpdatesFor("z"))
                z = (float)props.GetElement("z", z);
            if (props.UpdatesFor("roomType"))
                roomType = (String)props.GetElement("roomType", roomType);
            if (props.UpdatesFor("items"))
                items = (List<IEncodable>)props.GetElement("items", items);
           
        }

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

            
            
            // TODO: Handle those items
        }

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
                    int num = random.Next(max);
                    //HACK: this is dumb, but it works in practice 
                    while (num < min)
                    {
                        num = random.Next(max);
                    }
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

        private void HandlePossible(XmlHandler handler)
        {
            handler.GetNextElement();
            while (!handler.IsClosingTag("POSSIBLE"))
            {                
                ObjectParameters parameters = handler.GetAttributes();
                String type = parameters.GetStringValue("Special_Type");
                possible.AddPossibleObject(type,parameters);
                handler.GetNextElement();
            }
            
        }

        public void initialize()
        {
            

            

        }
        

        public void BuildWalls(Double X, Double Y, Double Z)
        {
            BuildWall("X", X + 3, "Z", Z + 18, Y);
            BuildWall("X", X + 3, "Z", Z - 3, Y);
            BuildWall("Z", Z + 0, "X", X + 0, Y);
            BuildWall("Z", Z + 0, "X", X + 21, Y);
            BuildCeiling(X,Y,Z);
            
            // this.Position = new Vector3((float)X, (float)Y, (float)Z);
        }

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

        private class PossibleObjects
        {
            private Dictionary<String, List<ObjectParameters>> typeLists;            
            List<ObjectParameters> lis = new List<ObjectParameters>();

            public PossibleObjects()
            {
                typeLists = new Dictionary<string, List<ObjectParameters>>();

            }
            
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
