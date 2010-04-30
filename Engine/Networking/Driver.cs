using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Collections;

namespace Mammoth.Engine.Networking
{
    class Driver : IEncodable
    {
        public String name;
        public int age;
        public bool DUI;

        public Driver(String aname, int hisage, bool hasDUI)
        {
            name = aname; age = hisage; DUI = hasDUI;
        }

        public byte[] Encode()
        {
            Encoder encode = new Encoder();

            encode.AddElement("name", name);
            encode.AddElement("age", age);
            encode.AddElement("DUI", DUI);

            return encode.Serialize();
        }

        public void Decode(byte[] serialized)
        {
            Encoder e = new Encoder(serialized);

            name = (string)e.GetElement("name");
            age = (int)e.GetElement("age");
            DUI = (bool)e.GetElement("DUI");
        }

        public void Print()
        {
            Console.WriteLine("Name: " + name);
            Console.WriteLine("Age: " + age);
            Console.WriteLine("DUI: " + DUI);
        }

        public int ID
        {
            get { return 2222; }
        }
    }
}
