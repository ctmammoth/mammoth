using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

using Mammoth.Engine.Networking;

namespace Mammoth.Engine
{
    public class Room : BaseObject, IEncodable
    {
        private double width, height, length;
        private double x, y, z;
        private List<BaseObject> objectList;
        private String roomType;

        public override string getObjectType()
        {
            return "Room";
        }

        public void InitializeDefault(int id)
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

        public Room(Game game, int id, ObjectParameters parameters)
            : base(game)
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






        


    }
}
