using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mammoth.Engine
{
    public class Crate : BaseObject
    {
        private int width, height, length, x, y, z;

        public Crate(int id, ObjectParameters parameters)
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
                    case "Crate_Type":
                        specialize(parameters.GetStringValue(attribute));
                }
            }
        }

            private void specialize(String attribute) 
            {
                XmlHandler handler = new XmlHandler();
                handler.ChangeFile("static_objects.xml");
                handler.GetElement("VARIANT", "NAME", attribute);
                while (!handler.IsClosingTag("VARIANT"))
                {
                    handler.GetNextElement();
                }
                


            }

        



    }
}
