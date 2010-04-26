using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace Mammoth.Engine.Interface
{
    public class TScreenManager : DrawableGameComponent
    {
        #region Fields

        List<TScreen> _screens = new List<TScreen>();

        #endregion

        public TScreenManager(Game game)
            : base(game)
        {

        }
    }
}
