using System;
using System.Collections.Generic;
using System.Diagnostics;

using StillDesign.PhysX;
using Microsoft.Xna.Framework;

namespace Mammoth.Engine.Physics
{
    public class PhysicsManagerService : IPhysicsManagerService
    {
        #region Variables
        /**
         * Core.
         */
        private Core core;

        /**
         * Stores the default scene description.  Every new scene built with createScene() will have this description.
         */
        private SceneDescription defaultSceneDesc;

        /**
         * This notifies events involved in a collision.  It will be given to scenes when they're created.
         */
        UserContactReport contactReport;

        /**
         * The current scene.
         */
        private Scene curScene;
        #endregion

        /**
         * The UserContactReport class for this PhysicsManagerService.  It only exists to override OnContactNotify.
         */
        private class ContactReporter : UserContactReport
        {
            /**
             * Collides two objects involved in a collision with each other.
             * 
             * @param contactInformation - the pair of Actors colliding.
             * @param events - not used by this method.
             */
            override public void OnContactNotify(ContactPair contactInformation, ContactPairFlag events)
            {
                // Test
                Debug.Assert(contactInformation != null);

                // Collide the objects with each other
                ((PhysicalObject)contactInformation.ActorA.UserData).collideWith(
                    (PhysicalObject)contactInformation.ActorB.UserData);
                ((PhysicalObject)contactInformation.ActorB.UserData).collideWith(
                    (PhysicalObject)contactInformation.ActorA.UserData);
            }
        }

        /**
         * Constructor.  Creates the PhysX core and sets the default scene description.
         */
        public PhysicsManagerService()
        {
            // Construct the core
            core = new Core(new CoreDescription(), new ConsoleOutputStream());

            // Create the contact reporter
            contactReport = new ContactReporter();

            // Initialize the scene description with reasonable values
            defaultSceneDesc = new SceneDescription()
            {
                // We'll make our own plane
                GroundPlaneEnabled = false,
                // Set this to our custom class
                UserContactReport = contactReport,
                // Set gravity to earth's default
                Gravity = new Vector3(0.0f, -9.81f, 0.0f) / 9.81f,
                // Use variable timesteps for the simulation to make sure that it's syncing with the refresh rate; may change later.
                TimestepMethod = TimestepMethod.Variable
                // TODO: choose whether to accelerate using hardware or software
            };

            // No scene yet
            curScene = null;
        }

        #region IPhysicsManagerService Members

        /**
         * Creates a new Actor in the current scene.
         * 
         * @param aDesc - the Actor description with which to create the new Actor.
         * @param owner - the object with which to associate the new Actor.  The new Actor's userdata is set to this value.
         * @return the new Actor, or null if there is currently no scene.
         */
        Actor IPhysicsManagerService.createActor(ActorDescription aDesc, PhysicalObject owner)
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

        /**
         * Removes an Actor from the current scene.
         * 
         * @param toRemove - the Actor to remove from the current scene.  Must be non-null.
         * @return true if the Actor was removed from the current scene, false otherwise.
         */
        bool IPhysicsManagerService.removeActor(Actor toRemove)
        {
            // Test
            Debug.Assert(toRemove != null);

            // Make sure the scene exists
            if (curScene != null)
            {
                // Is the Actor in the scene?
                if (curScene.Actors.Contains(toRemove))
                {
                    // Remove it
                    toRemove.Dispose();
                    // Test
                    Debug.Assert(!curScene.Actors.Contains(toRemove));

                    return true;
                }
                else
                    return false;
            }
            else
                return false;
        }

        Controller IPhysicsManagerService.createController(ControllerDescription cDesc)
        {
            throw new NotImplementedException();
        }

        bool IPhysicsManagerService.removeController(ControllerDescription toRemove)
        {
            throw new NotImplementedException();
        }

        /**
         * Creates a new scene with this PhysicsManagerService's default scene description.  Also creates the
         * ground plane.  If a scene already exists, this method has no effect.
         * 
         * TODO: figure out where to create the terrain using a heightfield.
         */
        void IPhysicsManagerService.createScene()
        {
            if (curScene == null)
                // Create the scene if it doesn't exist
                curScene = core.CreateScene(defaultSceneDesc);
        }

        /**
         * Deletes the current scene if it exists.  If no scene exists, this method has no effect.
         */
        void IPhysicsManagerService.removeScene()
        {
            if (curScene != null)
            {
                // Delete the scene if it exists
                curScene.Dispose();
                curScene = null;
            }
        }

        /**
         * Runs the simulation for a timestep.
         * 
         * @param time - the length of the timestep for which to run the simulation.
         */
        void IPhysicsManagerService.simulate(float time)
        {
            // Test
            Debug.Assert(time > 0);

            curScene.Simulate(time);
            curScene.FlushStream();
        }

        /**
         * Blocks until results have been fetched.
         * 
         * @return true when results have been fetched.
         */
        bool IPhysicsManagerService.fetchResults()
        {
            // Block until rigid body simulations have finished
            return curScene.FetchResults(SimulationStatus.RigidBodyFinished, true);
        }

        void IPhysicsManagerService.dispose()
        {
            if (curScene != null)
                curScene.Dispose();

            core.Dispose();
        }

        #endregion
    }
}
