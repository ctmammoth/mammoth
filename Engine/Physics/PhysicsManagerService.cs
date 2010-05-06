using System;
using System.Collections.Generic;
using System.Diagnostics;
using StillDesign.PhysX;
using Microsoft.Xna.Framework;

namespace Mammoth.Engine.Physics
{
    /// <summary>
    /// This interface contains the methods required by the physics manager.  It allows the user
    /// to pass in descriptions of actors to be added/removed from the scene.
    /// 
    /// See the comments for the implementation below for more information.
    /// </summary>
    public interface IPhysicsManagerService
    {
        void CollidePair(Actor ActorA, Actor ActorB);

        Actor CreateActor(ActorDescription aDesc);
        Actor CreateActor(ActorDescription aDesc, PhysicalObject owner);
        void RemoveActor(Actor toRemove);

        RaycastHit RaycastClosestShape(Vector3 position, Vector3 direction);
    
        Controller CreateController(ControllerDescription cDesc, PhysicalObject owner);

        void RemoveController(Controller toRemove);

        void CreateScene();
        void RemoveScene();
        
        void Simulate(float time); 
        bool FetchResults();

        void Dispose();

        #region Properties

        Core Core
        {
            get;
        }

        Scene Scene
        {
            get;
        }

        #endregion
    }

    /// <summary>
    /// Implementation of the physics manager service.
    /// </summary>
    public class PhysicsManagerService : GameComponent, IPhysicsManagerService
    {
        #region Variables
        /// <summary>
        /// The core.  Responsible for managing PhysX at the highest level.
        /// </summary>
        private Core core;

        /// <summary>
        /// A queue of Actors to be deleted.  This is needed because Actors cannot be removed during the
        /// simulation, according to the simulate() documentation.
        /// </summary>
        private Queue<Actor> actorDisposal;

        /// <summary>
        /// The current scene.
        /// </summary>
        private Scene curScene;

        /// <summary>
        /// Used to add/remove controllers.  There will probably be only one controller on the client
        /// (the local player).
        /// </summary>
        private ControllerManager controllerManager;

        /// <summary>
        /// A queue of Controllers to be deleted.  This is needed because Actors cannot be removed during 
        /// the simulation, according to the simulate() documentation.
        /// </summary>
        private Queue<Controller> controllerDisposal;
        #endregion

        /// <summary>
        /// The UserContactReport class for this PhysicsManagerService.
        /// </summary>
        private class ContactReporter : UserContactReport
        {
            private PhysicsManagerService owner;

            public ContactReporter(PhysicsManagerService owner)
            {
                this.owner = owner;
            }

            /// <summary>
            /// Collides two objects involved in a collision with each other.
            /// </summary>
            /// <param name="contactInformation">The pair of Actors colliding.</param>
            /// <param name="events">This argument is not used.</param>
            public override void OnContactNotify(ContactPair contactInformation, ContactPairFlag events)
            {
                // Test
                Debug.Assert(contactInformation != null);

                // Collide the pair in the ContactPair
                owner.CollidePair(contactInformation.ActorA, contactInformation.ActorB);
            }
        }

        /// <summary>
        /// Collides a pair of objects with each other
        /// </summary>
        /// <param name="ActorA"></param>
        /// <param name="ActorB"></param>
        public void CollidePair(Actor ActorA, Actor ActorB)
        {
            // Collide the objects with each other: make sure ActorA is not null and has userdata
            if (ActorA != null && ActorA.UserData != null)
            {
                // Make sure ActorB is not null and has userdata
                if (ActorB != null && ActorB.UserData != null)
                {
                    ((PhysicalObject)ActorA.UserData).CollideWith(
                        (PhysicalObject)ActorB.UserData);
                    Console.WriteLine("A = " + ((PhysicalObject)ActorA.UserData).getObjectType());
                }
            }
            else
                Console.WriteLine("ActorA has no data or is null");

            // Make sure ActorB is not null and has userdata
            if (ActorB != null && ActorB.UserData != null)
            {
                // Make sure ActorA is not null and has userdata
                if (ActorA != null && ActorA.UserData != null)
                {
                    ((PhysicalObject)ActorB.UserData).CollideWith(
                    (PhysicalObject)ActorA.UserData);
                    Console.WriteLine("B = " + ((PhysicalObject)ActorB.UserData).getObjectType());
                }
            }
            else
                Console.WriteLine("ActorB has no data or is null");
        }

        /// <summary>
        /// Constructor.  Creates the PhysX core and sets the default scene description.
        /// </summary>
        public PhysicsManagerService(Game game) : base(game)
        {
            // Add this object as the service provider
            game.Services.AddService(typeof(IPhysicsManagerService), this);

            // Make disposal queues
            actorDisposal = new Queue<Actor>();
            controllerDisposal = new Queue<Controller>();

            // Construct the core
            core = new Core(new CoreDescription(), new ConsoleOutputStream());
            // Turn on CCD (continuous collision detection)
            core.SetParameter(PhysicsParameter.ContinuousCollisionDetection, true);

            // Set debug parameters
            #if PHYSX_DEBUG
            core.SetParameter(PhysicsParameter.VisualizationScale, 2.0f);
            core.SetParameter(PhysicsParameter.VisualizeCollisionShapes, true);
            core.Foundation.RemoteDebugger.Connect("localhost");
            #endif

            // No scene or controller manager yet
            curScene = null;
            controllerManager = null;
        }

        /// <summary>
        /// Overrides GameComponent.Initialize().
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
        }

        /// <summary>
        /// Overrides GameComponent.Initialize().  Currently does nothing.
        /// 
        /// TODO: decide whether to call simulate() here.
        /// </summary>
        /// <param name="gametime"></param>
        public override void Update(GameTime gametime)
        {
        }

        #region IPhysicsManagerService Members

        /// <summary>
        /// Shoots a ray that stops after hitting the nearest shape.
        /// </summary>
        /// <param name="position">The position from which to cast the ray.</param>
        /// <param name="direction">A unit vector in the direction in which to cast the ray.</param>
        /// <returns>The PhysicalObject that owns the Actor that owns the Shape hit by the ray, or null if nothing
        /// was hit or the Actor hit had no userdata.</returns>
        public RaycastHit RaycastClosestShape(Vector3 position, Vector3 direction)
        {
            // Return the result of the raycast
            return curScene.RaycastClosestShape(new StillDesign.PhysX.Ray(position, direction), ShapesType.Dynamic);
            /*
            // Make sure the shape that was hit exists and that its actor has userdata
            if (rayHit.Shape != null && rayHit.Shape.Actor.UserData != null)
            {
                // Get the PhysicalObject that owns the Shape hit by the raycast
                PhysicalObject objHit = ((PhysicalObject)rayHit.Shape.Actor.UserData);
                Console.WriteLine("Raycast hit something of type " + objHit.getObjectType());
                return objHit;
            }
            else
                return null;
            */
        }

        /// <summary>
        /// Creates a new Actor in the current scene.
        /// </summary>
        /// <param name="aDesc">The Actor description with which to create the new Actor.</param>
        /// <param name="owner">The object with which to associate the new Actor.  The new Actor's userdata 
        /// is set to this value.</param>
        /// <returns>The new Actor, or null if there is currently no scene.</returns>
        public Actor CreateActor(ActorDescription aDesc, PhysicalObject owner)
        {
            // Make sure the scene exists
            if (curScene != null)
            {
                // Test
                Debug.Assert(aDesc != null);
                Debug.Assert(owner != null);

                // Create the actor in the current scene
                Actor actor = curScene.CreateActor(aDesc);
                // Associate it with the specified owner
                actor.UserData = owner;

                return actor;
            }
            else
                return null;
        }

        /// <summary>
        /// Creates a new Actor in the current scene.
        /// </summary>
        /// <param name="aDesc">The Actor description with which to create the new Actor.</param>
        /// <param name="owner">The object with which to associate the new Actor.  The new Actor's userdata 
        /// is set to this value.</param>
        public Actor CreateActor(ActorDescription aDesc)
        {
            // Make sure the scene exists
            if (curScene != null)
            {
                // Test
                Debug.Assert(aDesc != null);

                // Create the actor in the current scene
                Actor actor = curScene.CreateActor(aDesc);

                return actor;
            }
            else
                return null;
        }

        /// <summary>
        /// Removes an Actor from the current scene.  The Actor is not removed immediately: it is added to 
        /// the disposal queue and destroyed after the next called to fetchResults().  
        /// </summary>
        /// <param name="toRemove">The Actor to remove from the current scene.  Must be non-null.  If the
        /// Actor is not in the current scene, it will not be removed.</param>
        public void RemoveActor(Actor toRemove)
        {
            // Test
            Debug.Assert(toRemove != null);

            // Make sure the scene exists
            if (curScene != null)
            {
                // Is the Actor in the scene?
                if (curScene.Actors.Contains(toRemove))
                {
                    // Add to the queue
                    actorDisposal.Enqueue(toRemove);
                }
            }
        }

        /// <summary>
        /// Creates a character controller if a scene exists.
        /// </summary>
        /// <param name="cDesc">The description with which to create the new controller.  Must be non-null.</param>
        /// <param name="owner">The parent object of this controller.  Must be non-null.</param>
        /// <returns>The new controller (with the UserData field set to owner), or null if no scene currently
        /// exists.</returns>
        public Controller CreateController(ControllerDescription cDesc, PhysicalObject owner)
        {
            // Make sure the scene and controller manager exist
            if (curScene != null)
            {
                // Test
                Debug.Assert(controllerManager != null);
                Debug.Assert(cDesc != null);
                Debug.Assert(owner != null);

                // Create the controller
                Controller ctrler = controllerManager.CreateController(cDesc);
                // Set the userdata for the controller and its actor
                ctrler.UserData = owner;
                ctrler.Actor.UserData = owner;

                return ctrler;
            }
            else
                return null;
        }

        /// <summary>
        /// Removes a character controller from the current scene.  The Controller is not removed immediately:
        /// it's added to a disposal queue and destroyed after the next fetchResults() call.
        /// </summary>
        /// <param name="toRemove">The controller to be removed.  Must be non-null.  If the Controller is
        /// not in the current scene, it will not be removed.</param>
        public void RemoveController(Controller toRemove)
        {
            // Test
            Debug.Assert(toRemove != null);

            // Make sure the scene exists
            if (curScene != null)
            {
                // Make sure the controller is in the current manager
                if (controllerManager.Controllers.Contains(toRemove))
                {
                    // Add to the queue
                    controllerDisposal.Enqueue(toRemove);
                }
            }
        }

        /// <summary>
        /// Creates a new scene with this PhysicsManagerService's default scene description.  Also creates 
        /// the ground plane.  If a scene already exists, this method has no effect.
        /// 
        /// TODO: figure out where to create the terrain using a heightfield.
        /// </summary>
        public void CreateScene()
        {
            // Make sure no scene exists
            if (curScene == null)
            {
                // Figure out whether or not to simulate with hardware
                SimulationType hworsw = (core.HardwareVersion == HardwareVersion.None ? SimulationType.Software :
                    SimulationType.Hardware);

                // Create the scene
                curScene = core.CreateScene(new SceneDescription()
                {
                    // We'll make our own plane
                    GroundPlaneEnabled = false,
                    // Set this to our custom class
                    UserContactReport = new ContactReporter(this),
                    // Set gravity to earth's default
                    Gravity = new Vector3(0.0f, -9.81f, 0.0f) / 9.81f,
                    // Use variable timesteps for the simulation to make sure that it's syncing with the refresh 
                    // rate; may change later.
                    TimestepMethod = TimestepMethod.Variable,
                    // Hardware or software simulation
                    SimulationType = hworsw
                });

                // Enable collisions for all objects
                curScene.SetActorGroupPairFlags(0, 0, ContactPairFlag.All);

                // Create the controller manager
                controllerManager = curScene.CreateControllerManager();
            }
        }

        /// <summary>
        /// Deletes the current scene immediately (if it exists).  If no scene exists, this method has no 
        /// effect.
        /// 
        /// TODO: should this be void and just add the actor to a disposal queue (just to be safe)?
        /// </summary>
        public void RemoveScene()
        {
            if (curScene != null)
            {
                // Clear disposal queue: don't want strange references to nonexistant Actors
                actorDisposal.Clear();

                // Delete the scene if it exists
                curScene.Dispose();
                curScene = null;
            }
        }

        /// <summary>
        /// Runs the simulation for a timestep.  Must be followed by a call to fetchResults().
        /// </summary>
        /// <param name="time">The length of the timestep for which to run the simulation.</param>
        public void Simulate(float time)
        {
            curScene.Simulate(time);
            curScene.FlushStream();
        }

        /// <summary>
        /// Blocks until results have been fetched.  Must be preceded by a call to simulate().
        /// </summary>
        /// <returns>True when results have been fetched.</returns>
        public bool FetchResults()
        {
            // Block until rigid body simulations have finished
            bool resultsFetched = curScene.FetchResults(SimulationStatus.RigidBodyFinished, true);

            // Dispose of all deleted Actors
            while (actorDisposal.Count > 0)
            {
                actorDisposal.Dequeue().Dispose();
            }

            // Dispose of all deleted Controllers
            while (controllerDisposal.Count > 0)
            {
                controllerDisposal.Dequeue().Dispose();
            }

            return resultsFetched;
        }

        /// <summary>
        /// Disposes of this physics manager.  The scene (and therefore all actors in it) are 
        /// dispose()'d.
        /// 
        /// TODO: what if this is called during simulate()?
        /// </summary>
        public new void Dispose()
        {
            if (curScene != null)
            {
                curScene.Dispose();
            }

            actorDisposal.Clear();
            core.Dispose();
        }
        #endregion

        #region Properties

        public Core Core
        {
            get { return core; }
        }

        public Scene Scene
        {
            get { return curScene; }
        }

        #endregion
    }
}
