using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.IO;

namespace Mammoth.Engine.Networking
{
    class Car : IEncodable
    {
        public int max_speed;
        public int min_speed;
        public string color;
        public Driver driver;

        public Car(int maxspeed, int minspeed, string thecolor, Driver driveme)
        //public Car(int maxspeed, int minspeed, string thecolor)
        {
            max_speed = maxspeed; min_speed = minspeed; color = thecolor; driver = driveme;
            //max_speed = maxspeed; min_speed = minspeed; color = thecolor;
        }

        public Car(int maxspeed, int minspeed, string thecolor)
        {
            max_speed = maxspeed; min_speed = minspeed; color = thecolor; driver = new Driver("", 0, false);
        }

        public byte[] Encode()
        {
            //create new encoder
            Encoder thecode = new Encoder();

            //add attributes
            thecode.AddElement("max_speed", max_speed);
            thecode.AddElement("min_speed", min_speed);
            thecode.AddElement("color", color);
            thecode.AddElement("driver", driver);

            return thecode.Serialize();
        }

        public void Decode(byte[] serialized)
        {
            //create new decoder with overloaded constructor of Encoder
            Encoder e = new Encoder(serialized);

            //get primitives outs
            max_speed = (int)e.GetElement("max_speed");
            min_speed = (int)e.GetElement("min_speed");
            color = (string)e.GetElement("color");

            //update IEncodables
            e.UpdateIEncodable("driver", driver);
        }

        public void Print()
        {
            Console.WriteLine("Max_speed: " + max_speed);
            Console.WriteLine("Min_speed: " + min_speed);
            Console.WriteLine("Color: " + color);
            if(driver != null)
                driver.Print();
        }
    }
}
