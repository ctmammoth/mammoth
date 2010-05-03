using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using StillDesign.PhysX;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Mammoth.Engine
{
    public class Crate : BaseObject
    {
        private Vector3 position, dimensions;
        
        


        public Crate(int id, ObjectParameters parameters)
        {
            this.objectId = id;            
            foreach (String attribute in parameters.GetAttributes()) 
            {
                switch(attribute) 
                {
                    case "X":
                        position.X = parameters.GetDoubleValue(attribute);                        
                        break;
                    case "Y":
                        position.Y = parameters.GetDoubleValue(attribute);
                        break;
                    case "Z":
                        position.Z = parameters.GetDoubleValue(attribute);
                        break;
                    case "Crate_Type":
                        specialize(parameters.GetStringValue(attribute));
                }
            }
        }

            private void Specialize(String attribute) 
            {
                XmlHandler handler = new XmlHandler();
                handler.ChangeFile("static_objects.xml");
                handler.GetElement("VARIANT", "NAME", attribute);
                while (!handler.IsClosingTag("VARIANT"))
                {
                    handler.GetNextElement();
                    String name = handler.GetName();
                    switch (name)
                    {
                        case "DIMENSION":
                            HandleDimension(handler);
                            break;
                        case "MODEL":
                            HandleModel(handler);
                            break;
                            
                    }

                } 

            }

            public override void InitializeDefault(int id)
            {
                ID = id;
                x = 0;
                y = 0;
                z = 0;
                width = 0;
                length = 0;
                height = 0;
            }

            public override byte[] Encode()
            {
                Networking.Encoder tosend = new Networking.Encoder();

                tosend.AddElement("Position", Position);
                tosend.AddElement("Orientation", Orientation);
                tosend.AddElement("Velocity", Velocity);

                return tosend.Serialize();
            }

            public override void Decode(byte[] serialized)
            {
                Networking.Encoder props = new Networking.Encoder(serialized);

                Position = (Vector3)props.GetElement("Position");
                Orientation = (Quaternion)props.GetElement("Orientation");
                Velocity = (Vector3)props.GetElement("Velocity");
            }


            public override String getObjectType()
            {
                return "Crate";
            }

            private void HandleDimension(XmlHandler handler)
            {
                ObjectParameters parameters = handler.getAttributes();
                foreach (String attribute in parameters)
                {
                    switch (attribute)
                    {
                        case "WIDTH":
                            width = parameters.GetDoubleValue(attribute);
                            break;
                        case "HEIGHT":
                            height = parameters.GetDoubleValue(attribute);
                            break;
                        case "LENGTH":
                            length = parameters.GetDoubleValue(attribute);
                            break;
                    }
                }                
            }

            private void HandleModel(XmlHandler handler)
            {
                String modelPath = handler.reader.ReadContentAsString();
                //TODO: Actually make this a model
            }


        



    }
}
