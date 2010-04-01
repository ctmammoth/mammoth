using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Mammoth.Engine
{
    class MamContentManager
    {
        #region Variables

        private MamContentManager _instance;

        private Dictionary<string, Model> _modelDict;
        private Dictionary<string, Texture2D> _textureDict;

        #endregion

        private MamContentManager()
        {
            
        }

        public Model loadModel(string path)
        {
            if (_modelDict.ContainsKey(path))
                return _modelDict[path];
            Model toLoad;
        }

        public Texture2D loadTexture(string path)
        {

        }

        #region Properties

        private Microsoft.Xna.Framework.Content.ContentManager Content
        {
            get;
            set;
        }

        #endregion
    }
}
