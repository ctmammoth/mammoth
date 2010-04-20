using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.IO;

namespace Mammoth.Engine
{
    class Car : Encodable
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

        public void Encode(Stream tosend)
        {
            //create new encoder
            Encoder thecode = new Encoder();

            //add attributes
            thecode.AddElement("max_speed", max_speed);
            thecode.AddElement("min_speed", min_speed);
            thecode.AddElement("color", color);

            thecode.Serialize(tosend);
        }

        public void Decode(Stream serialized)
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
