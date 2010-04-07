using System;
using System.Collections.Generic;
using System.Text;

namespace Mammoth.Engine
{
    public interface IEncodable
    {
        void Encode();

        void Decode();

        void ConstructFromNetwork();
    }
}
