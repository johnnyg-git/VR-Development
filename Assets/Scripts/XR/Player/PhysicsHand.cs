using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace VrPhysicsFramework
{
    public class PhysicsHand : MonoBehaviour
    {
        public Rigidbody handRb;

        void Awake()
        {
            handRb.transform.parent = null;
        }
    }
}
