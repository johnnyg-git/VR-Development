using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Video;

namespace VrPhysicsFramework
{
    /// <summary>
    /// Used for grabbing objects and basic haptics
    /// </summary>
    public class HandGrabber : MonoBehaviour
    {
        [Tooltip("How far the hand grab an object from")]
        public float reach;
        [Tooltip("What layer to look for when trying to grab something")]
        public LayerMask grabbable;
        [Tooltip("The hand is this")]
        public handTypes handSide;

        [Tooltip("All the current grabbable objects, will be set at runtime")]
        public List<Grabbable> grabbables = new List<Grabbable>();
        [Tooltip("Layer to set grabbed objects to")]
        public int grabbedLayer = 11;

        [Tooltip("The joint that connects a grabbed object to the hand, set at runtime")]
        public ConfigurableJoint connectJoint;

        [Tooltip("Input system to get controls from, set at runtime")]
        public Controller input;

        /// <summary>
        /// Gets grabbable objects around the hand
        /// </summary>
        SphereCollider trigger;
        /// <summary>
        /// Layer to return an object to when let go
        /// </summary>
        int oldLayer = 9;

        void Awake()
        {
            // Create reach trigger
            trigger = gameObject.AddComponent<SphereCollider>();
            trigger.isTrigger = true;
            trigger.radius = reach;
        }

        /// <summary>
        /// Handles haptics when the hand collides
        /// </summary>
        private void OnCollisionEnter(Collision collision)
        {
            // If input is valid and if collision velocity is over a threshold
            if (input!=null && collision.impulse.magnitude > 1)
            {
                uint channel = 0;
                float amplitude = Mathf.Clamp(collision.impulse.magnitude/5,0,1);
                float duration = Mathf.Clamp(amplitude /2,0.1f,0.3f);
                // Send haptics to device
                input.device.SendHapticImpulse(channel, amplitude, duration);
            }
        }

        /// <summary>
        /// Will grab and let go of objects if needed
        /// </summary>
        private void Update()
        {
            // Try get input if not already valid
            if (input == null)
            {
                try
                {
                    input = XRMain.instance.controllers[handSide];
                    reach = XRMain.instance.grabReach;
                    grabbable = XRMain.instance.grabbable;
                }
                catch { }
                return;
            }
            // If there are grabbables, grip is pressed and not already grabbing then grab
            if (grabbables.Count > 0 && input.gripPressed && connectJoint == null)
            {
                // Create joint on grabbable
                connectJoint = grabbables[0].rb.gameObject.AddComponent<ConfigurableJoint>();
                // Store current layer for future use
                oldLayer = grabbables[0].gameObject.layer;
                // Set objects layer to grabbed layer
                grabbables[0].SetLayer(grabbedLayer);
                // Make grabbable rotate correctly to fit handle and hand
                grabbables[0].rb.transform.rotation = transform.rotation * Quaternion.Inverse(grabbables[0].transform.rotation) * grabbables[0].rb.transform.rotation;
                // Setup joint
                connectJoint.autoConfigureConnectedAnchor = false;
                connectJoint.xDrive = new JointDrive() { positionSpring = 10000, positionDamper = 50, maximumForce = 3.402823e+38f };
                connectJoint.yDrive = new JointDrive() { positionSpring = 10000, positionDamper = 50, maximumForce = 3.402823e+38f };
                connectJoint.zDrive = new JointDrive() { positionSpring = 10000, positionDamper = 50, maximumForce = 3.402823e+38f };
                connectJoint.angularXDrive = new JointDrive() { positionSpring = 10000, positionDamper = 50, maximumForce = 3.402823e+38f };
                connectJoint.angularYZDrive = new JointDrive() { positionSpring = 10000, positionDamper = 50, maximumForce = 3.402823e+38f };
                connectJoint.connectedBody = GetComponent<Rigidbody>();
                // Make anchor move to handles position so object is moved relative to handle
                connectJoint.anchor = grabbables[0].transform.localPosition;
                connectJoint.connectedAnchor = Vector3.zero;
            }
            // If holding object but no longer pressing down grip then destroy joint
            else if (!input.gripPressed && connectJoint != null)
            {
                // Return object to old layer
                grabbables[0].SetLayer(oldLayer);
                // Destroy the joint
                Destroy(connectJoint);
            }
        }

        /// <summary>
        /// If the trigger touches a grabbable object it will add it to the grabbables list
        /// </summary>
        /// <param name="other"></param>
        private void OnTriggerEnter(Collider other)
        {
            if ((((1 << other.gameObject.layer) & grabbable) != 0) && other.gameObject.GetComponent<Grabbable>() && !grabbables.Contains(other.gameObject.GetComponent<Grabbable>())) {
                print("Found grabbable");
                grabbables.Add(other.gameObject.GetComponent<Grabbable>());
            }
        }

        /// <summary>
        /// Will remove grabbables from the list when no longer needed
        /// </summary>
        private void OnTriggerExit(Collider other)
        {
            // If object is in grabbable layermask and has a grabbale component
            if ((((1 << other.gameObject.layer) & grabbable) != 0) && other.gameObject.GetComponent<Grabbable>())
            {
                // If is inside grabbables remove it
                if (grabbables.Contains(other.gameObject.GetComponent<Grabbable>()))
                    grabbables.Remove(other.gameObject.GetComponent<Grabbable>());
            }
        }

        /// <summary>
        /// Will draw the reach of the hand in the editor
        /// </summary>
        private void OnDrawGizmosSelected()
        {
            // Draw reach in editor
            Gizmos.color = Color.grey;
            Gizmos.DrawWireSphere(transform.position, reach);
        }
    }
}
