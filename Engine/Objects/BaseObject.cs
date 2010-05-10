using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace Mammoth.Engine
{
    /// <summary>
    /// Class which all objects in the game inherit from. 
    /// </summary>
    public abstract class BaseObject : IDisposable
    {
        /// <summary>
        /// Sets the BaseObjeect's game field to the current game and 
        /// sets it to be alive
        /// </summary>
        /// <param name="game">The current game</param>
        public BaseObject(Game game)
        {
            this.Game = game;
            this.IsAlive = true;
        }
        
        /// <summary>
        /// Updates for the object for every timestep.
        /// </summary>
        /// <param name="gameTime">The official GameTime.</param>
        public virtual void Update(GameTime gameTime) { }

        /// <summary>
        /// How to draw the object.
        /// </summary>
        /// <param name="gameTime">The official GameTime.</param>
        public virtual void Draw(GameTime gameTime) { }

        /// <summary>
        /// Supplies every object with a unique ID which can be used to access the object in the ModelDB.
        /// </summary>
        public int ID
        {
            get;
            set;
        }
        
        /// <summary>
        /// Returns a string that identifies the object type. Used in sending an IEncodable and reconstructing it to the proper type.
        /// </summary>
        /// <returns></returns>
        public abstract String getObjectType();

        #region Properties

        public Game Game
        {
            get;
            protected set;
        }

        public bool IsAlive
        {
            get;
            set;
        }

        #endregion

        #region IDisposable Members

        public virtual void Dispose() { }

        #endregion
    }
}
