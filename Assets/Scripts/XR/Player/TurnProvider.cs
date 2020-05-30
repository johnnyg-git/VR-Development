using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

namespace VrPhysicsFramework
{
    public class TurnProvider : LocomotionProvider
    {
        public enum InputAxes
        {
            Primary2DAxis = 0,
            Secondary2DAxis = 1,
        };

        static readonly InputFeatureUsage<Vector2>[] m_Vec2UsageList = new InputFeatureUsage<Vector2>[] {
            CommonUsages.primary2DAxis,
            CommonUsages.secondary2DAxis,
        };

        [Tooltip("Should use snap turn of not")]
        public bool snapTurn;

        [Tooltip("The 2D Input Axis on the primary devices that will be used to trigger a snap turn.")]
        public InputAxes usedAxis = InputAxes.Primary2DAxis;

        [Tooltip("A list of controllers that allow Snap Turn.  If an XRController is not enabled, or does not have input actions enabled.  Snap Turn will not work.")]
        public List<XRController> controllers = new List<XRController>();

        [Header("Snap turn")]
        [Tooltip("The number of degrees clockwise to rotate when snap turning clockwise.")]
        public float turnAmount;

        [Tooltip("The amount of time that the system will wait before starting another snap turn.")]
        public float waitTime = 0.5f;

        [Tooltip("The deadzone that the controller movement will have to be above to trigger a snap turn.")]
        public float deadZone = 0.75f;

        [Header("Smooth turn")]
        [Tooltip("The number of degrees clockwise to rotate a second when smooth turning")]
        public float turnPerSecond;

        [Tooltip("The deadzone that the controller movement will have to be above to trigger smooth turning")]
        public float smoothDeadZone = 0.75f;

        float m_CurrentTurnAmount = 0.0f;
        float m_TimeStarted = 0.0f;

        List<bool> m_ControllersWereActive = new List<bool>();

        private void Update()
        {
            // wait for a certain amount of time before allowing another turn.
            if (snapTurn && m_TimeStarted > 0.0f && (m_TimeStarted + waitTime < Time.time))
            {
                m_TimeStarted = 0.0f;
                return;
            }

            if (controllers.Count > 0)
            {
                EnsureControllerDataListSize();

                InputFeatureUsage<Vector2> feature = m_Vec2UsageList[(int)usedAxis];
                for (int i = 0; i < controllers.Count; i++)
                {
                    XRController controller = controllers[i];
                    if (controller != null)
                    {
                        if (controller.enableInputActions && m_ControllersWereActive[i])
                        {
                            InputDevice device = controller.inputDevice;

                            Vector2 currentState;
                            if (device.TryGetFeatureValue(feature, out currentState))
                            { 
                                float checkingFor = snapTurn ? deadZone : smoothDeadZone;
                                if (currentState.x > checkingFor)
                                {
                                    if (snapTurn)
                                        StartTurn(turnAmount);
                                    else
                                        StartTurn(turnPerSecond * Time.deltaTime * currentState.x);
                                }
                                else if (currentState.x < -checkingFor)
                                {
                                    if (snapTurn)
                                        StartTurn(-turnAmount);
                                    else
                                        StartTurn(-turnPerSecond * Time.deltaTime * -currentState.x);
                                }
                            }
                        }
                        else //This adds a 1 frame delay when enabling input actions, so that the frame it's enabled doesn't trigger a snap turn.
                        {
                            m_ControllersWereActive[i] = controller.enableInputActions;
                        }
                    }
                }
            }

            if (Math.Abs(m_CurrentTurnAmount) > 0.0f && BeginLocomotion())
            {
                var xrRig = system.xrRig;
                if (xrRig != null)
                {
                    xrRig.RotateAroundCameraUsingRigUp(m_CurrentTurnAmount);
                }
                m_CurrentTurnAmount = 0.0f;
                EndLocomotion();
            }
        }

        void EnsureControllerDataListSize()
        {
            if (controllers.Count != m_ControllersWereActive.Count)
            {
                while (m_ControllersWereActive.Count < controllers.Count)
                {
                    m_ControllersWereActive.Add(false);
                }

                while (m_ControllersWereActive.Count < controllers.Count)
                {
                    m_ControllersWereActive.RemoveAt(m_ControllersWereActive.Count - 1);
                }
            }
        }

        internal void FakeStartTurn(bool isLeft)
        {
            StartTurn(isLeft ? -turnAmount : turnAmount);
        }

        private void StartTurn(float amount)
        {
            if (snapTurn && m_TimeStarted != 0.0f)
                return;

            if (!CanBeginLocomotion())
                return;

            m_TimeStarted = Time.time;
            m_CurrentTurnAmount = amount;
        }
    }
}