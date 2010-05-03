using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Mammoth.Engine
{
    public class XmlHandler
    {
        public XmlTextReader reader;
        private String currentPath; 

        public void ChangeFile(String path)
        {
            currentPath = path;
            reader = new XmlTextReader(path);
        }
        public void GetNextElement()
        {
            do
            {
                reader.Read();
            }
            while (reader.NodeType != XmlNodeType.Element);

        }

        public String GetElementName()
        {
            return reader.Name;
        }



        public String GetNextElementName()
        {
            reader.Read();
            
            if (reader.NodeType == XmlNodeType.Element)
            {
                
            }

            return null;
        }

        public bool GetElement(String elementType, String attribute, String value)
        {
            ChangeFile(currentPath);
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (reader.Name.Equals(elementType))
                    {
                        if (reader.HasAttributes)
                        {
                            while (reader.MoveToNextAttribute())
                            {
                                if (reader.Name.Equals(attribute) && reader.Value.Equals(value))
                                {
                                    reader.MoveToElement();
                                    return true;
                                }
                            } 
                        }
                    }
                }
            }
            return false;


        }

        public bool IsClosingTag(String name)
        {
            if (reader.NodeType == XmlNodeType.EndElement && reader.Name.Equals(name))
            {
                return true;
            }
            return false;
        }



        public static object CreateFromXml(String type, String parameters)
        {
            return null;
        }

        public ObjectParameters getAttributes()
        {         
            ObjectParamters parameters = new ObjectParameters();
            for (int i = 0; i < reader.AttributeCount; i++)
            {
                reader.MoveToAttribute(i);
                parameters.AddAttribute(reader.Name,reader.Value);
                
            }

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
            }

        }*/

    }
}
