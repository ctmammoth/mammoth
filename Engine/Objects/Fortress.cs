using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mammoth.Engine.Networking;

namespace Mammoth.Engine
{
    public class Fortress : BaseObject, IEncodable
    { 
        private double width, height, length, x, y, z;
        private List<Room> roomList;
        private Room[][][] roomGrid;

        


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

        public void Decode(Byte[] data)
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
