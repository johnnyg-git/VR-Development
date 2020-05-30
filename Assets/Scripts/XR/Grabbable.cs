using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace VrPhysicsFramework
{
    public class Grabbable : MonoBehaviour
    {
        public float grabDist;
        public float axis;

        [NonSerialized]
        public Rigidbody rb;
        private Collider trigger;

        void Awake()
        {
            rb = GetComponentInParent<Rigidbody>();
            if(rb==null)
                rb = GetComponent<Rigidbody>();
            if (rb == null)
            {
                Debug.LogError("Failed to find rigidbody for grabbable on " + gameObject.name);
                Destroy(gameObject);
                return;
            }
            gameObject.layer = Layers.grabbable;
            if (axis < 0)
            {
                SphereCollider t = gameObject.AddComponent<SphereCollider>();
                t.radius = grabDist;
                trigger = t;
            }
            else
            {
                CapsuleCollider t = gameObject.AddComponent<CapsuleCollider>();
                t.radius = grabDist;
                t.height = axis;
                trigger = t;
            }
            trigger.isTrigger = true;
        }

        public void SetLayer(int layer)
        {
            foreach(Collider t in rb.gameObject.GetComponentsInChildren<Collider>())
            {
                t.gameObject.layer = layer;
            }
        }

        public virtual void OnGrabbed()
        {

        }

        private void OnDrawGizmosSelected()
        {
            if (axis > 0)
            {
                float _height = axis;
                Vector3 _pos = transform.position;
                Matrix4x4 angleMatrix = Matrix4x4.TRS(_pos, transform.rotation, Handles.matrix.lossyScale);
                using (new Handles.DrawingScope(angleMatrix))
                {
                    Handles.color = Color.green;
                    var pointOffset = (_height - (grabDist * 2)) / 2;
                    Handles.DrawWireArc(Vector3.up * pointOffset, Vector3.left, Vector3.back, -180, grabDist);
                    Handles.DrawLine(new Vector3(0, pointOffset, -grabDist), new Vector3(0, -pointOffset, -grabDist));
                    Handles.DrawLine(new Vector3(0, pointOffset, grabDist), new Vector3(0, -pointOffset, grabDist));
                    Handles.DrawWireArc(Vector3.down * pointOffset, Vector3.left, Vector3.back, 180, grabDist);
                    Handles.DrawWireArc(Vector3.up * pointOffset, Vector3.back, Vector3.left, 180, grabDist);
                    Handles.DrawLine(new Vector3(-grabDist, pointOffset, 0), new Vector3(-grabDist, -pointOffset, 0));
                    Handles.DrawLine(new Vector3(grabDist, pointOffset, 0), new Vector3(grabDist, -pointOffset, 0));
                    Handles.DrawWireArc(Vector3.down * pointOffset, Vector3.back, Vector3.left, -180, grabDist);
                    Handles.DrawWireDisc(Vector3.up * pointOffset, Vector3.up, grabDist);
                    Handles.DrawWireDisc(Vector3.down * pointOffset, Vector3.up, grabDist);

                }
            }
            else
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(transform.position,grabDist);
            }
        }
    }
}