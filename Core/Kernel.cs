using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Mammoth.Core
{
    class Kernel
    {
        #region Variables

        private static Kernel _instance = null;
        private LinkedList<IProcess> _processList;

        #endregion

        private Kernel()
        {
            _processList = new LinkedList<IProcess>();
        }

        public void UpdateAll(GameTime gameTime)
        {
            LinkedListNode<IProcess> node = _processList.First;
            while(node != null)
            {
                // Make sure that the process is still alive.
                if (node.Value.IsAlive)
                    // Tell the process to update itself.
                    node.Value.Update(gameTime);
                // If it's not still alive, remove it from the process list.
                else
                    _processList.Remove(node);

                node = node.Next;
            }
        }

        public void RegisterProcess(IProcess proc)
        {
            _processList.AddLast(proc);
        }

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
