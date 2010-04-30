using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Mammoth.Engine
{
    class XmlHandler
    {
        public XmlTextReader reader;

        public void ChangeFile(String path)
        {
            reader = new XmlTextReader(path);
        }

        public String GetNextElementName()
        {
            reader.Read();
            if (reader.NodeType == XmlNodeType.Element)
            {
                
            }

            return null;


        }


        public static object CreateFromXml(String type, String parameters)
        {
            return null;
        }

        /* Xml Test
        static void Main(string[] args)
        {

            
            // Create an isntance of XmlTextReader and call Read method to read the file
            XmlHandler x = new XmlHandler();
            x.ChangeFile("rooms.xml");
            XmlTextReader reader = x.reader;
            while (reader.Read())
            {
                Console.WriteLine(reader.Name);

                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (reader.HasAttributes) 
                    { 
						reader.MoveToFirstAttribute();
                        Console.WriteLine(" {0}={1}", reader.Name, reader.Value);
						while (reader.MoveToNextAttribute()) {
	                        Console.WriteLine(" {0}={1}", reader.Name, reader.Value);
                        }
                        // Move the reader back to the element node.
                        reader.MoveToElement();

                    }

                }
            } */

        //}

    }
}
