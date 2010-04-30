using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mammoth.Engine
{
    public class ObjectParameters
    {
        private Dictionary<String, String> parameters;

        public ObjectParameters() 
        {
            parameters = new Dictionary<String,String>();
            
        }

        public bool AddAttribute(String attribute, String value)
        {
            // There should only be one value for each attribute value
            if (parameters.ContainsKey(attribute) && parameters.TryGetValue(attribute) != null)
            {
                return false;
            }
            parameters.Add(attribute, value);
            return true;
        }

        public void ReplaceAttribute(String attribute, String value)
        {
            parameters.Add(attribute, value);
        }

        public Dictionary<String,String>.KeyCollection GetAttributes()
        {
            return parameters.Keys;
        }

        public double GetDoubleValue(String attribute)
        {
            return Double.Parse(parameters.TryGetValue(attribute));
        }
        public String GetStringValue(String attribute)
        {
            return parameters.TryGetValue(attribute);
        }


        



    }
}
