using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Input;
using System.Configuration;

namespace Mammoth.Engine.Input
{
    public interface IInputService
    {
        #region Properties

        bool IsLocal
        {
            get;
        }

        InputState State
        {
            get;
        }

        #endregion
    }
}
