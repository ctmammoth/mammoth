﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace Mammoth.Engine.Networking
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
        /// Loads serialized data, and makes the objects accessible to Decode() methods in IEncodable objects.
        /// Throws an error if the byte[] cannot be deserialized.
        /// </summary>
        /// <param name="serialized">a byte array representing a serialized hashtable</param>
        public Encoder(byte[] serialized)
        {
            //The below code was modified from
            //http://msdn.microsoft.com/en-us/library/system.runtime.serialization.formatters.binary.binaryformatter.deserialize(VS.71).aspx
            try
            {
                //load from byte array to stream
                MemoryStream s = new MemoryStream(serialized);

                // Deserialize the hashtable from the file and 
                // assign the reference to the local variable.
                BinaryFormatter formatter = new BinaryFormatter();

                //set table to deserialized table
                table = (Hashtable)formatter.Deserialize(s);
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to deserialize. Reason: " + e.Message);
                table = new Hashtable();
            }
        }


        /// <summary>
        /// Adds a property to be encoded. Cannot take anything that is not a primitive or doesn't implement Encodable. Will not add null.
        /// </summary>
        /// <param name="key">A string representing the name of the property being encoded.</param>
        /// <param name="theobject">The property it self. May be any primitive or Encodable. Will be ignored if property is null.</param>
        public void AddElement(string key, object theobject)
        {
            if (theobject != null)
            {
                table.Add(key, theobject);
            }
        }


        /// <summary>
        /// Adds a property to be encoded. Cannot take anything that is not a primitive or doesn't implement Encodable. Will not add null.
        /// </summary>
        /// <param name="key">A string representing the name of the property being encoded.</param>
        /// <param name="theobject">The property it self. May be any primitive or Encodable. Will be ignored if property is null.</param>
        public void AddElement(string key, IEncodable theobject)
        {
            if (theobject != null)
            {
                table.Add(key, theobject.Encode());
            }
        }


        /// <summary>
        /// Once a byte array has been deserialized, then elements can be accesed through their String key. Returns null if the key could not be hashed.
        /// </summary>
        /// <param name="key">a string that hashes to the parameter to access</param>
        /// <returns>an object which must be type cast</returns>
        public object GetElement(string key, object orig)
        {
            object hashed = table[key];

            if (hashed != null)
                return hashed;
            else
            {
                return (object)orig;
            }
        }

        /// <summary>
        /// Once a byte array has been deserialized, then IEncodable objects can be updated. Note that if the IEncodable is null, it cannot be updated.
        /// </summary>
        /// <param name="key">a string that hashes to the parameter to access </param>
        /// <param name="toupdate">the actual object to update</param>
        public void UpdateIEncodable(string key, IEncodable toupdate)
        {
            try
            {
                if (toupdate != null)
                {
                    byte[] hashed = (byte[])table[key];
                    toupdate.Decode(hashed);
                }
            }
            catch (Exception e)
            {

            }
        }


        /// <summary>
        /// Checks if there is an update for the given key.
        /// </summary>
        /// <param name="key">The string key that wants to be checked.</param>
        /// <returns>True if there is indeed an update.</returns>
        public bool UpdatesFor(string key)
        {
            return table.ContainsKey(key);
        }


        /// <summary>
        /// Serializes the properties and their names and outputs to a byte array. This serialization can be deserialized later by an Encoder.
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
    }
}