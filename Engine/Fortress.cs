using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mammoth.Engine
{
    public class Fortress : Object
    { 
        private double width, height, length, x, y, z;
        private List<Room> roomList;
        private Room[][][] roomGrid;

        public override String getObjectType()
        {
            return "Fortress";
        }











        
    }
}
