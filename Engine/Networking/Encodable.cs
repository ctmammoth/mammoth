using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Encoder
{
    public interface Encodable
    {
        void Encode(Stream tosend);
        void Decode(Stream serialized);
    }
}
