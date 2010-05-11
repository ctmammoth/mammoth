using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

using Mammoth.Engine.Networking;

namespace Mammoth.Engine
{
    /// <summary>
    /// Represents a fortress, comprised of many rooms. Currently, the fortresses are
    /// created using individual and independent rooms, and this class is unused.
    /// </summary>
    public class Fortress : BaseObject, IEncodable
    {
        #region Fields

        private double width, height, length, x, y, z;
        private List<Room> roomList;
        private Room[][][] roomGrid;

        #endregion

        public Fortress(Game game)
            : base(game)
        {

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
