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
    public class Room : BaseObject, IEncodable, IRenderable
    {
        private double width, height, length;
        private double x, y, z;
        private List<BaseObject> objectList;
        private String roomType;

        public Game Game
        {
            get;
            protected set;
        }

        public override string getObjectType()
        {
            return "Room";
        }

        public Room(int id, Game game)
        {
            this.ID = id;
            this.Game = game;
        }


        public override void InitializeDefault(int id)
        {
            // TODO: Implement this
            throw new NotImplementedException();
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

        public Room(int id, ObjectParameters parameters)
        {
            this.ID = id;            
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
                    case "Room_Type":
                        Specialize(parameters.GetStringValue(attribute));
                        break;
                }
            }
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
            // TODO: Handle those items
        }

        private void HandleParameters(XmlHandler handler)
        {
            // TODO: Handle those parameters
        }

        public void initialize()
        {
            populate();

            

        }

        public void populate()
        {
            //TODO: fix this?
            //List<BaseObject> objects = XmlHandler.CreateFromXml(roomType);
        }

        public void BuildWall()
        {

            IModelDBService modelDB = (IModelDBService)this.Game.Services.GetService(typeof(IModelDBService));

            ObjectParameters parameters;
            for (int i = 0; i < 25; i++)
            {
                parameters = new ObjectParameters();
                Double number1 = (3.6 * (i / 5));
                Double number2 = (3.6 * (i % 5));


                parameters.AddAttribute("X", number1.ToString());
                parameters.AddAttribute("Y", number2.ToString());
                parameters.AddAttribute("Z", "14.4");
                parameters.AddAttribute("Crate_Type", "DARK");


                if ((!(i > 9 && i < 12)))
                {
                    int crateId = modelDB.getNextOpenID();
                    Crate crate1 = (Crate)ObjectFactories.CreateObject("Crate", crateId, parameters, this.Game);
                    modelDB.registerObject(crate1);
                }

            }
        }


        private class PossibleObjects
        {
            Dictionary<String, List<PossibleObject>> objects;

            private void AddObject(XmlHandler handler)
            {
                ObjectParameters attributes = handler.GetAttributes();

            }


        }
        private class PossibleObject 
        {
        }










        #region IRenderable Members

        public Microsoft.Xna.Framework.Vector3 Position
        {
            get { throw new NotImplementedException(); }
        }

        public Microsoft.Xna.Framework.Vector3 PositionOffset
        {
            get { throw new NotImplementedException(); }
        }

        public Microsoft.Xna.Framework.Quaternion Orientation
        {
            get { throw new NotImplementedException(); }
        }

        public Microsoft.Xna.Framework.Graphics.Model Model3D
        {
            get { throw new NotImplementedException(); }
        }

        #endregion
    }
}
