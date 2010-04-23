using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.IO;

namespace Mammoth.Engine
{
    class Car : IEncodable
    {
        public int max_speed;
        public int min_speed;
        public string color;
        //public Driver driver;

        //public Car(int maxspeed, int minspeed, string thecolor, Driver driveme)
        public Car(int maxspeed, int minspeed, string thecolor)
        {
            //max_speed = maxspeed; min_speed = minspeed; color = thecolor; driver = driveme;
            max_speed = maxspeed; min_speed = minspeed; color = thecolor;
        }

        public byte[] Encode()
        {
            //create new encoder
            Encoder thecode = new Encoder();

            //add attributes
            thecode.AddElement("max_speed", max_speed);
            thecode.AddElement("min_speed", min_speed);
            thecode.AddElement("color", color);

            return thecode.Serialize();
        }

        public void Decode(byte[] serialized)
        {
            Hashtable received = Encoder.Deserialize(serialized);

            max_speed = (int)received["max_speed"];
            min_speed = (int)received["min_speed"];
            color = (string)received["color"];
        }

        public void Print()
        {
            Console.WriteLine("Max_speed: " + max_speed);
            Console.WriteLine("Min_speed: " + min_speed);
            Console.WriteLine("Color: " + color);
        }
    }
}
