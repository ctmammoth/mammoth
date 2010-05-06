using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using StillDesign.PhysX;

namespace Mammoth.Engine.Objects
{
    /// <summary>
    /// Used to give things like the ground plane and that random box some userdata
    /// </summary>
    public class MinimalPhysicalObject : PhysicalObject
    {
        public MinimalPhysicalObject(Actor a)
        {
            Actor = a;
            a.UserData = this;
        }
            
        public override string getObjectType()
        {
            return "Minimal physical object";
        }

        public void InitializeDefault(int id)
        {
            ID = id;
        }

        public override void CollideWith(PhysicalObject obj)
        {
        }
    }
}
