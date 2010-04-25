using System;
using StillDesign.PhysX;

namespace Mammoth.Engine.Physics
{
    interface IPhysicsManagerService
    {
        Actor createActor(ActorDescription aDesc, PhysicalObject owner);
        bool removeActor(Actor toRemove);
    
        Controller createController(ControllerDescription cDesc);
        bool removeController(ControllerDescription toRemove);

        void createScene();
        void removeScene();
        
        void simulate(float time); 
        bool fetchResults();

        void dispose();
    }
}
