using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mammoth.Engine
{
    public class Room : Object
    {
        private double width, height, length;
        private List<Object> objectList;
        private String roomType;

        public override string getObjectType()
        {
            return "Room";
        }

        

        public void initialize()
        {
            populate();

            

        }

        public void populate()
        {
            List<Object> objects = XmlHandler.CreateFromXml(roomType);
        }




        


    }
}
