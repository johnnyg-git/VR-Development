using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VrPhysicsFramework
{
    public class Grabbable : MonoBehaviour
    {
        public float grabDist;

        private SphereCollider trigger;

        void Awake()
        {
            gameObject.layer = Layers.grabbable;
            trigger = gameObject.AddComponent<SphereCollider>();
            trigger.isTrigger = true;
            trigger.radius = grabDist;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, grabDist);
        }
    }
}