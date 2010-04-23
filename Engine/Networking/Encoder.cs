using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace Mammoth.Engine
{
    public class Encoder
    {
        private Hashtable table;

        /// <summary>
        /// The Encoder constructor creates a new Encoder object to store properties in.
        /// Properties are stored in a hashtable in which string versions of their names hash to the properties' values.
        /// </summary>
        public Encoder()
        {
            table = new Hashtable();
        }












        /// <summary>
        /// Adds a property to be encoded. Cannot take anything that is not a primitive or doesn't implement Encodable.
        /// </summary>
        /// <param name="key">A string representing the name of the property being encoded.</param>
        /// <param name="theobject">The property it self. May be any primitive or Encodable.</param>
        public void AddElement(string key, object theobject)
        {
            table.Add(key, theobject);
        }















        /// <summary>
        /// Adds a property to be encoded. Cannot take anything that is not a primitive or doesn't implement Encodable.
        /// </summary>
        /// <param name="key">A string representing the name of the property being encoded.</param>
        /// <param name="theobject">The property it self. May be any primitive or Encodable.</param>
        public void AddElement(string key, IEncodable theobject)
        {
            table.Add(key, theobject);
        }






















        /// <summary>
        /// Serializes the properties and their names and outputs to a stream. This serialization of byte can be deserialized later.
        /// </summary>
        /// <returns>a byte array containing the serialized data</returns>
        public byte[] Serialize()
        {
            //The below code was modified from
            //http://msdn.microsoft.com/en-us/library/system.runtime.serialization.formatters.binary.binaryformatter.deserialize(VS.71).aspx

            //Create memory stream
            MemoryStream s = new MemoryStream();

            //Copy hashtable to be serialized.
            Hashtable tosend = new Hashtable();
            tosend = table;

            // Construct a BinaryFormatter and use it to serialize the data to the stream.
            BinaryFormatter formatter = new BinaryFormatter();
            try
            {
                //serialize hashtable to stream
                formatter.Serialize(s, tosend);

                //declare byte array to store serialization in
                byte[] byteArray = new byte[s.Length];

                //set conditions to start at beginning of stream
                int count = 0;
                s.Seek(0, SeekOrigin.Begin);

                //read from stream to byte array
                while (count < s.Length)
                {
                    byteArray[count] = Convert.ToByte(s.ReadByte());
                    count++;
                }

                return byteArray;
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to serialize. Reason: " + e.Message);
                throw;
            }
        }
























        /// <summary>
        /// Deserializes a stream into a hashtable which can be used to get values sent across a network.
        /// </summary>
        /// <param name="serialized">a byte array of a serialized hashtable to be deserialized</param>
        /// <returns>the hashtable obtained from the serialized data</returns>
        public static Hashtable Deserialize(byte[] serialized)
        {
            //The below code was modified from
            //http://msdn.microsoft.com/en-us/library/system.runtime.serialization.formatters.binary.binaryformatter.deserialize(VS.71).aspx

            // Declare the hashtable reference.
            Hashtable recieved = null;

            try
            {
                //load from byte array to stream
                MemoryStream s = new MemoryStream(serialized);

                // Deserialize the hashtable from the file and 
                // assign the reference to the local variable.
                BinaryFormatter formatter = new BinaryFormatter();
                recieved = (Hashtable)formatter.Deserialize(s);

                return recieved;
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to deserialize. Reason: " + e.Message);
                throw;
            }
        }
    }
}