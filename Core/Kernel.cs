using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mammoth.Core
{
    class Kernel
    {
        #region Variables

        private static Kernel _instance = null;
        private LinkedList<IProcess> _controllerList;

        #endregion



        #region Properties

        public static Kernel Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Kernel();
                return _instance;
            }
        }

        #endregion
    }
}
