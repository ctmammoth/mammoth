using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mammoth.Engine.Networking
{
    public interface IEncodable
    {
        /// <summary>
        /// Encodes this object into a byte array.
        /// </summary>
        /// <returns>A serialized version of the object.</returns>
        byte[] Encode();

        /// <summary>
        /// Sets this objects fields based on the passed in serialized form.
        /// </summary>
        /// <param name="serialized">The byte array describing this object</param>
        void Decode(byte[] serialized);
    }
}
