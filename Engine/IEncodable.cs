using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mammoth.Engine
{
    public interface IEncodable
    {
        byte[] encode();
        void decode(byte[] data);
    }
}
