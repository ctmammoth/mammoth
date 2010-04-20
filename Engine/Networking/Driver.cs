using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Collections;

namespace Mammoth.Engine
{
    class Driver : Encodable
    {
        public String name;
        public int age;
        public bool DUI;

        public Driver(String aname, int hisage, bool hasDUI)
        {
            name = aname; age = hisage; DUI = hasDUI;
        }

        public void Encode(Stream tosend)
        {
            Encoder encode = new Encoder();

            encode.AddElement("name", name);
            encode.AddElement("age", age);
            encode.AddElement("DUI", DUI);

            encode.Serialize(tosend);
        }

        public void Decode(Stream serialized)
        {
            Hashtable received = Encoder.Deserialize(serialized);

            name = (string)received["name"];
            age = (int)received["age"];
            DUI = (bool)received["DUI"];
        }
    }
}
