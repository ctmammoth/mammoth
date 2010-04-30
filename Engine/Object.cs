using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mammoth.Engine
{
    public abstract class Object
    {
        private static int nextId = 0;
        private int objectId;
        private String objectType;

        public int ObjectId
        {
            get
            {
                return objectId;
            }
            set
            {
                objectId = value;
            }
        }

        public abstract String getObjectType();



        



    }
}
