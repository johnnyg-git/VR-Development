using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Video;

namespace VrPhysicsFramework
{
    public class HandGrabber : MonoBehaviour
    {
        public float reach;
        public LayerMask grabbable;
        public handTypes handSide;

        public List<Grabbable> grabbables = new List<Grabbable>();

        public Controller input;

        private SphereCollider trigger;
        public ConfigurableJoint connectJoint;

        void Awake()
        {
            trigger = gameObject.AddComponent<SphereCollider>();
            trigger.isTrigger = true;
            trigger.radius = reach;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if(input==null && XRMain.instance.controllers.ContainsKey(handSide))
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
            if (input!=null && collision.impulse.magnitude > 1)
            {
                uint channel = 0;
                float amplitude = Mathf.Clamp(collision.impulse.magnitude/5,0,1);
                float duration = Mathf.Clamp(amplitude /2,0.1f,0.3f);
                input.device.SendHapticImpulse(channel, amplitude, duration);
            }
        }

        private void Update()
        {
            if (input == null && XRMain.instance.controllers.ContainsKey(handSide))
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
            if (grabbables.Count > 0 && input.gripPressed && connectJoint == null)
            {
                connectJoint = grabbables[0].gameObject.AddComponent<ConfigurableJoint>();
                connectJoint.autoConfigureConnectedAnchor = false;
                connectJoint.xDrive = new JointDrive() { positionSpring = 10000, positionDamper = 50, maximumForce = 3.402823e+38f };
                connectJoint.yDrive = new JointDrive() { positionSpring = 10000, positionDamper = 50, maximumForce = 3.402823e+38f };
                connectJoint.zDrive = new JointDrive() { positionSpring = 10000, positionDamper = 50, maximumForce = 3.402823e+38f };
                connectJoint.angularXDrive = new JointDrive() { positionSpring = 10000, positionDamper = 50, maximumForce = 3.402823e+38f };
                connectJoint.angularYZDrive = new JointDrive() { positionSpring = 10000, positionDamper = 50, maximumForce = 3.402823e+38f };
                connectJoint.connectedBody = GetComponent<Rigidbody>();
                connectJoint.connectedAnchor = Vector3.zero;
                connectJoint.anchor = Vector3.zero;
            }
            else if (!input.gripPressed && connectJoint != null)
            {
                Destroy(connectJoint);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (input == null && XRMain.instance.controllers.ContainsKey(handSide))
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
            if ((((1 << other.gameObject.layer) & grabbable) != 0) && other.gameObject.GetComponent<Grabbable>()) {
                print("Found grabbable");
                grabbables.Add(other.gameObject.GetComponent<Grabbable>());
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if ((((1 << other.gameObject.layer) & grabbable) != 0) && other.gameObject.GetComponent<Grabbable>())
            {
                if (grabbables.Contains(other.gameObject.GetComponent<Grabbable>()))
                    grabbables.Remove(other.gameObject.GetComponent<Grabbable>());
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.grey;
            Gizmos.DrawWireSphere(transform.position, reach);
        }
    }
}
