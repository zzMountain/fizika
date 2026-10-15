using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(HingeJoint))]
public class Swing : MonoBehaviour
{
    [SerializeField, Min(0f)] private float _pushImpulse = 24f;
    [SerializeField, Min(0f)] private float _maximumAngularSpeed = 2.2f;

    private Rigidbody _body;

    private void Awake()
    {
        _body = GetComponent<Rigidbody>();
    }

    public void Push()
    {
        Vector3 axis = transform.right;
        float speed = Vector3.Dot(_body.angularVelocity, axis);

        if (Mathf.Abs(speed) >= _maximumAngularSpeed)
            return;

        float direction = speed < 0f ? -1f : 1f;
        _body.AddTorque(axis * (_pushImpulse * direction), ForceMode.Impulse);
    }
}
