using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class MovementProvider : MonoBehaviour
{
    private Rigidbody rb;

    public enum InputAxes
    {
        Primary2DAxis = 0,
        Secondary2DAxis = 1,
    };

    static readonly InputFeatureUsage<Vector2>[] m_Vec2UsageList = new InputFeatureUsage<Vector2>[] {
            CommonUsages.primary2DAxis,
            CommonUsages.secondary2DAxis,
        };

    public InputAxes usedAxis = InputAxes.Primary2DAxis;

    public XRRig rig;

    [Tooltip("A list of controllers that allow Snap Turn.  If an XRController is not enabled, or does not have input actions enabled.  Snap Turn will not work.")]
    public List<XRController> controllers = new List<XRController>();

    [Tooltip("The deadzone that the controller movement will have to be above to trigger movement")]
    public float deadZone = 0.1f;

    public float speed;

    List<bool> m_ControllersWereActive = new List<bool>();

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
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

    void Update()
    {
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
                            Vector3 velocity = new Vector3(0, rb.velocity.y, 0);
                            Quaternion dir = Quaternion.Euler(0, rig.cameraGameObject.transform.eulerAngles.y, 0);
                            if(currentState.x>deadZone)
                                velocity.x = speed * Time.fixedDeltaTime * currentState.x;
                            else if (currentState.x < -deadZone)
                                velocity.x = -speed * Time.fixedDeltaTime * -currentState.x;
                            if (currentState.y > deadZone)
                                velocity.z = -speed * Time.fixedDeltaTime * -currentState.y;
                            else if (currentState.y < -deadZone)
                                velocity.z = speed * Time.fixedDeltaTime * currentState.y;

                            velocity = dir * velocity;
                            velocity.y = rb.velocity.y;

                            rb.velocity = velocity;
                        }
                    }
                    else
                    {
                        m_ControllersWereActive[i] = controller.enableInputActions;
                    }
                }
            }
        }
    }
}
