using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mammoth.Engine
{
    public class Fortress : BaseObject
    { 
        private double width, height, length, x, y, z;
        private List<Room> roomList;
        private Room[][][] roomGrid;

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

        public override void Decode(Byte[] data)
        {
            // TODO: Implement this
            throw new NotImplementedException();
        }

        public override String getObjectType()
        {
            return "Fortress";
        }











        
    }
}
