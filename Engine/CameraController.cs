using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using Mammoth.Core;

namespace Mammoth.Engine
{
    class CameraController : Controller<Camera>
    {
        #region Variables

        #endregion

        public CameraController(Camera camera)
        {
            this.Model = camera;
        }

        private void UpdateFirstPerson()
        {
            LocalPlayer lp = Engine.Instance.LocalPlayer;


        }

        public override void Update(GameTime gameTime)
        {
            switch (this.Model.Type)
            {
                case Camera.CameraType.FIRST_PERSON:
                    UpdateFirstPerson();
                    break;
                default:
                    break;
            }
        }
    }
}
