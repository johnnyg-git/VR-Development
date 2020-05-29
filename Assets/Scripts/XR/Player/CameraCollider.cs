using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CameraCollider : MonoBehaviour
{
    [Tooltip("Radius of the capsule collider")]
    public float radius;

    CapsuleCollider col;

    void Start()
    {
        col = new GameObject("Cam Collider").AddComponent<CapsuleCollider>();
        col.gameObject.layer = gameObject.layer;
        col.transform.parent = transform.parent;
        col.radius = radius;
    }

    void FixedUpdate()
    {
        col.height = transform.localPosition.y;
        col.transform.position = transform.position;
        col.center = new Vector3(0, -col.height / 2, 0);
    }

    private void OnDrawGizmosSelected()
    {
        float _height = transform.localPosition.y;
        Vector3 _pos = new Vector3(transform.position.x, transform.position.y - (_height / 2), transform.position.z);
        Matrix4x4 angleMatrix = Matrix4x4.TRS(_pos, transform.rotation, Handles.matrix.lossyScale);
        using (new Handles.DrawingScope(angleMatrix))
        {
            Handles.color = Color.red;
            var pointOffset = (_height - (radius * 2)) / 2;
            Handles.DrawWireArc(Vector3.up * pointOffset, Vector3.left, Vector3.back, -180, radius);
            Handles.DrawLine(new Vector3(0, pointOffset, -radius), new Vector3(0, -pointOffset, -radius));
            Handles.DrawLine(new Vector3(0, pointOffset, radius), new Vector3(0, -pointOffset, radius));
            Handles.DrawWireArc(Vector3.down * pointOffset, Vector3.left, Vector3.back, 180, radius);
            Handles.DrawWireArc(Vector3.up * pointOffset, Vector3.back, Vector3.left, 180, radius);
            Handles.DrawLine(new Vector3(-radius, pointOffset, 0), new Vector3(-radius, -pointOffset, 0));
            Handles.DrawLine(new Vector3(radius, pointOffset, 0), new Vector3(radius, -pointOffset, 0));
            Handles.DrawWireArc(Vector3.down * pointOffset, Vector3.back, Vector3.left, -180, radius);
            Handles.DrawWireDisc(Vector3.up * pointOffset, Vector3.up, radius);
            Handles.DrawWireDisc(Vector3.down * pointOffset, Vector3.up, radius);

        }
    }
}
