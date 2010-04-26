using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

using Microsoft.Xna.Framework;

namespace Mammoth.Engine.Networking
{
    class Test
    {
        static void Main()
        {

            /*////CAR TEST
            //Driver ben = new Driver("Ben", 20, false);
            //Driver dave = new Driver("Dave", 100, true);
            //Car volvo = new Car(100, 10, "Red", ben);

            //Console.WriteLine("Original car: ");
            //volvo.Print();

            //byte[] carArray = volvo.Encode();

            //volvo.max_speed = 500;
            //volvo.min_speed = -100;
            //volvo.color = "Blue";
            //volvo.driver = dave;

            //Console.WriteLine("Modified car: ");
            //volvo.Print();

            //volvo.Decode(carArray);

            //Console.WriteLine("Restored car: ");
            volvo.Print();*


            //////Keep console open for testing.
            //Console.Read();
            TestServer();
             */
            TestClient();
        }

        static void TestServer()
        {
            IServerNetworking server = new LidgrenServerNetworking(Mammoth.Engine.Engine.Instance);
            while (true)
            {
                server.Update(null);
            }
        }

        static void TestClient()
        {
            IClientNetworking client = new LidgrenClientNetworking(Mammoth.Engine.Engine.Instance);
            Driver ben = new Driver("Ben", 20, false);
            Car volvo = new Car(100, 10, "Red");
            client.joinGame();
            client.sendThing(volvo);
            client.Update(null);
        }
    }
}
