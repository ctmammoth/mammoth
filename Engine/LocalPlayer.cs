using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mammoth.Engine
{
    class LocalPlayer : Player<LocalPlayer>
    {
        public LocalPlayer() : base()
        {
            // At end of the method, create the controller for this LocalPlayer.
            CreateController();
        }

        protected override void CreateController()
        {
            base.Controller = new LocalPlayerController(this);
            base.Controller.Register();
        }
    }
}
