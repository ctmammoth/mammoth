using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace Mammoth.Engine
{
    class Test
    {
        static void Main()
        {

            //CAR TEST
            Car volvo = new Car(100, 10, "Red");

            Console.WriteLine("Original car: ");
            volvo.Print();

            byte[] carArray = volvo.Encode();

            volvo.max_speed = 500;
            volvo.min_speed = -100;
            volvo.color = "Blue";

            Console.WriteLine("Modified car: ");
            volvo.Print();

            volvo.Decode(carArray);

            Console.WriteLine("Restored car: ");
            volvo.Print();


            //Keep console open for testing.
            Console.Read();
        }
    }
}
