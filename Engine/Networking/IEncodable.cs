using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mammoth.Engine.Networking
{
    public interface IEncodable
    {
        byte[] Encode();
        void Decode(byte[] serialized);

        #region Properties

        //int ID
        //{
        //    get;
        //}

        #endregion
    }
}
