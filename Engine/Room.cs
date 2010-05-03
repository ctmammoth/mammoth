using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mammoth.Engine
{
    public class Room : BaseObject
    {
        private double width, height, length;
        private List<BaseObject> objectList;
        private String roomType;

        public override string getObjectType()
        {
            return "Room";
        }

        public override void InitializeDefault(int id)
        {
            // TODO: Implement this
            throw new NotImplementedException();
        }

        public override Byte[] Encode()
        {
            // TODO: Implement this
            throw new NotImplementedException();
        }

        public override void Decode (Byte[] data)
        {
            // TODO: Implement this
            throw new NotImplementedException();
        }

        public Room(int id, ObjectParameters parameters)
        {
            this.objectId = id;            
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
                        specialize(parameters.GetStringValue(attribute));
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
                String name = handler.GetName();
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
                ObjectParameters attributes = handler.getAttributes();

            }


        }
        private class PossibleObject 
        {
        }






        


    }
}
