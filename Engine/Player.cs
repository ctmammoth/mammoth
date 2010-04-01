using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mammoth.Core;

namespace Mammoth.Engine
{
    abstract class Player<T> : DynamicObject<T> where T : Player<T>
    {
        public Player() : base()
        {
            
        }
    }
}
