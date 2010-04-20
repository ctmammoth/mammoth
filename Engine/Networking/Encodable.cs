using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Mammoth.Engine
{
    public interface Encodable
    {
        void Encode(Stream tosend);
        void Decode(Stream serialized);
    }
}
