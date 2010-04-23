using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mammoth.Engine
{
    public interface IEncodable
    {
        byte[] Encode();
        void Decode(byte[] serialized);
    }
}
