using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace Encoder
{
    class Test
    {
        static void Main()
        {
            Encoder encode = new Encoder();

            //Populate encoder.
            int a = 222; float b = 0.5F; double c = 100000000; string d = "hello";
            encode.AddElement("int", a);
            encode.AddElement("float", b);
            encode.AddElement("double", c);
            encode.AddElement("string", d);

            // To serialize the hashtable and its key/value pairs,  
            // you must first open a stream for writing. 
            // In this case, use a file stream.
            FileStream fs = new FileStream("TestFile.dat", FileMode.Create);
            encode.Serialize(fs);
            fs.Close();

            // Open the file containing the data that you want to deserialize.
            FileStream fs2 = new FileStream("TestFile.dat", FileMode.Open);
            Hashtable recieved = Encoder.Deserialize(fs2);
            fs2.Close();


            // To prove that the table deserialized correctly, 
            // display the key/value pairs.
            foreach (DictionaryEntry de in recieved)
            {
                Console.WriteLine("{0} hashes to  {1}.", de.Key, de.Value);
            }



            //CAR TEST
            Car volvo = new Car(100, 10, "Red");

            Console.WriteLine("Original car: ");
            volvo.Print();

            FileStream carstream1 = new FileStream("Car.dat", FileMode.Create);
            volvo.Encode(carstream1);
            carstream1.Close();

            volvo.max_speed = 500;
            volvo.min_speed = -100;
            volvo.color = "Blue";

            Console.WriteLine("Modified car: ");
            volvo.Print();

            // Open the file containing the data that you want to deserialize.
            FileStream carstream2 = new FileStream("Car.dat", FileMode.Open);
            volvo.Decode(carstream2);
            carstream2.Close();

            Console.WriteLine("Restored car: ");
            volvo.Print();


            //Keep console open for testing.
            //Console.Read();
        }
    }
}
