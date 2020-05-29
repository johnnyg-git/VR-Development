using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace VrPhysicsFramework
{
    public class Controller : MonoBehaviour
    {
        internal InputDevice device;

        public Vector2 primary2DAxis;
        public float trigger;
        public float grip;
        public Vector2 secondary2DAxis;
        public bool secondary2DAxisClick;
        public bool primaryButton;
        public bool primaryPressed;
        public bool primaryTouch;
        public bool secondaryButton;
        public bool secondaryTouch;
        public bool gripButton;
        public bool gripPressed;
        public bool triggerButton;
        public bool triggerPressed;
        public bool menuButton;
        public bool primary2DAxisClick;
        public bool primary2DAxisTouch;
        public float batteryLevel;
        public HapticCapabilities hapticCapabilities;
        public bool supportsHaptics;

        private void Awake()
        {
            if (device.TryGetHapticCapabilities(out hapticCapabilities))
                supportsHaptics = true;
        }

        private void Update()
        {
            if(device!=null)
            {
                device.TryGetFeatureValue(CommonUsages.primary2DAxis, out primary2DAxis);
                device.TryGetFeatureValue(CommonUsages.trigger, out trigger);
                device.TryGetFeatureValue(CommonUsages.grip, out grip);
                device.TryGetFeatureValue(CommonUsages.primaryButton, out primaryButton);
                device.TryGetFeatureValue(CommonUsages.primaryTouch, out primaryTouch);
                device.TryGetFeatureValue(CommonUsages.primary2DAxis, out primary2DAxis);
                device.TryGetFeatureValue(CommonUsages.secondaryButton, out secondaryButton);
                device.TryGetFeatureValue(CommonUsages.gripButton, out gripButton);
                device.TryGetFeatureValue(CommonUsages.triggerButton, out triggerButton);
                device.TryGetFeatureValue(CommonUsages.menuButton, out menuButton);
                device.TryGetFeatureValue(CommonUsages.primary2DAxisClick, out primary2DAxisClick);
                device.TryGetFeatureValue(CommonUsages.primary2DAxisTouch, out primary2DAxisTouch);
                device.TryGetFeatureValue(CommonUsages.batteryLevel, out batteryLevel);

                if (grip > .25f) gripPressed = true;
                else gripPressed = false;

                if (trigger > .25f) triggerPressed = true;
                else triggerPressed = false;
            }
        }
    }
}
