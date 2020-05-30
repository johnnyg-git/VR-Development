using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

namespace VrPhysicsFramework
{
    [RequireComponent(typeof(XRRig))]
    [RequireComponent(typeof(TurnProvider))]
    public class XRMain : MonoBehaviour
    {
        public static XRMain instance;
        [NonSerialized]
        public Controller rightControllerInput, leftControllerInput;
        public Dictionary<handTypes, Controller> controllers = new Dictionary<handTypes, Controller>();

        [Header("Main setup")]
        [Tooltip("XR Controllers for ")]
        public XRController rightHand;
        public XRController leftHand;
        [SerializeField]
        [Tooltip("What to try find for controllers")]
        InputDeviceCharacteristics rightControllerCharacteristics, leftControllerCharacteristics;

        [Header("Grabbing setup")]
        [Tooltip("Max grabbing distance for hands")]
        public float grabReach = 0.15f;
        [Tooltip("What's grabbable by the hands")]
        public LayerMask grabbable;

        List<InputDevice> devices = new List<InputDevice>();
        InputDevice rightControllerDevice, leftControllerDevice;

        void Start()
        {
            if(instance!=null)
                throw new Exception("XRMain already exists");

            instance = this;
            Time.fixedDeltaTime = Time.timeScale / XRDevice.refreshRate;
            InputDevices.GetDevices(devices);
            List<InputDevice> hands = new List<InputDevice>();

            try
            {
                InputDevices.GetDevicesWithCharacteristics(rightControllerCharacteristics, hands);
                if (hands.Count > 0)
                {
                    rightControllerDevice = hands[0];
                    rightControllerInput = rightHand.gameObject.AddComponent<Controller>();
                    controllers[handTypes.right] = rightControllerInput;
                    rightControllerInput.device = rightControllerDevice;
                }
            }
            catch { }

            try
            {
                InputDevices.GetDevicesWithCharacteristics(leftControllerCharacteristics, hands);
                if (hands.Count > 0)
                {
                    leftControllerDevice = hands[0];
                    leftControllerInput = leftHand.gameObject.AddComponent<Controller>();
                    controllers[handTypes.left] = leftControllerInput;
                    leftControllerInput.device = leftControllerDevice;
                }
            }
            catch { }
        }

        private void Update()
        {
            if(leftControllerDevice==null)
            {
                List<InputDevice> hands = new List<InputDevice>();
                InputDevices.GetDevicesWithCharacteristics(leftControllerCharacteristics, hands);
                if (hands.Count > 0)
                {
                    leftControllerDevice = hands[0];
                    leftControllerInput = leftHand.gameObject.AddComponent<Controller>();
                    controllers[handTypes.left] = leftControllerInput;
                    leftControllerInput.device = leftControllerDevice;
                }
            }
            if(rightControllerDevice==null)
            {
                List<InputDevice> hands = new List<InputDevice>();
                InputDevices.GetDevicesWithCharacteristics(rightControllerCharacteristics, hands);
                if (hands.Count > 0)
                {
                    rightControllerDevice = hands[0];
                    rightControllerInput = rightHand.gameObject.AddComponent<Controller>();
                    controllers[handTypes.right] = rightControllerInput;
                    rightControllerInput.device = rightControllerDevice;
                }
            }
        }

        private void OnDestroy()
        {
            instance = null;
        }
    }
}
