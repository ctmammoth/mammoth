using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mammoth.Engine.Objects
{
    public interface IHoldableItem
    {
        #region Properties
        
        Player Owner
        {
            get;
        }

        #endregion
    }
}
