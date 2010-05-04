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

        

        public void initialize()
        {
            populate();

            

        }

        public void populate()
        {
            //TODO: fix this?
            //List<BaseObject> objects = XmlHandler.CreateFromXml(roomType);
        }




        


    }
}
