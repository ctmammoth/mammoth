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

        public override string getObjectType()
        {
            return "Room";
        }

        public Room(int id, Game game)
            : base(game)
        {
            this.ID = id;
            this.Game = game;
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


        public Byte[] Encode()
        {
            // TODO: Implement this
            throw new NotImplementedException();
        }

        public void Decode (Byte[] data)
        {
            // TODO: Implement this
            throw new NotImplementedException();
        }

        public Room(int id, ObjectParameters parameters, Game game)
            : base(game)
        {
            boxActorDesc = new ActorDescription()
            {
                /*BodyDescription = new BodyDescription()
                {
                    Mass = 1000.0f
                }*/
            };

            this.Game = game;
            this.ID = id;
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
            while (!handler.IsClosingTag("ROOM"))
            {
                handler.GetNextElement();
                String name = handler.GetElementName();
                switch (name)
                {
                    case "ITEMS":
                        HandleItems(handler);
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
            // TODO: Handle those parameters
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

                
                if (!(i==0||i==7||i==63||i==56||i==9||i==54||i==53||i==52||i==51))
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
