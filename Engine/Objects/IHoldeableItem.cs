using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mammoth.Engine.Objects
{
    public interface IHoldeableItem
    {
        void SetOwner(Player owner);
    }
}
