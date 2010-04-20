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
        public void AddElement(string key, Encodable theobject)
        {
            table.Add(key, theobject);
        }






















        /// <summary>
        /// Serializes the properties and their names and outputs to a stream. This serialization of Byte can be deserialized later.
        /// </summary>
        public void Serialize(Stream s)
        {
            //The below code was modified from
            //http://msdn.microsoft.com/en-us/library/system.runtime.serialization.formatters.binary.binaryformatter.deserialize(VS.71).aspx
            
            //Copy hashtable to be serialized.
            Hashtable tosend = new Hashtable();
            tosend = table;
            
            // Construct a BinaryFormatter and use it to serialize the data to the stream.
            BinaryFormatter formatter = new BinaryFormatter();
            try
            {
                formatter.Serialize(s, tosend);
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
        public static Hashtable Deserialize(Stream s)
        {
            //The below code was modified from
            //http://msdn.microsoft.com/en-us/library/system.runtime.serialization.formatters.binary.binaryformatter.deserialize(VS.71).aspx

            // Declare the hashtable reference.
            Hashtable recieved = null;
 
            try
            {
                BinaryFormatter formatter = new BinaryFormatter();

                // Deserialize the hashtable from the file and 
                // assign the reference to the local variable.
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
