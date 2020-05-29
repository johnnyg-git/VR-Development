using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using VrPhysicsFramework;

namespace VrPhysicsFramework
{
    public class ContinuousMovement : MonoBehaviour
    {
        public float speed = 1;
        public handTypes source;

        public Controller sourceInput;
        CharacterController controller;

        private void Start()
        {
            controller = GetComponent<CharacterController>();
        }

        private void FixedUpdate()
        {
            if (sourceInput == null)
                sourceInput = XRMain.instance.controllers[source];

            Vector3 direction = new Vector3(sourceInput.primary2DAxis.x, 0, sourceInput.primary2DAxis.y);

            controller.Move(direction * Time.fixedDeltaTime * speed);
        }
    }
}