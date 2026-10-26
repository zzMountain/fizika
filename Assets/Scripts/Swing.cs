using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(HingeJoint))]
public class Swing : MonoBehaviour
{
    [SerializeField] private CommandInput _input;
    [SerializeField, Min(0f)] private float _pushImpulse = 24f;
    [SerializeField, Min(0f)] private float _maximumAngularSpeed = 2.2f;

    private Rigidbody _body;
    private bool _pushRequested;

    private void Awake()
    {
        _body = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        _input.Requested += OnPushRequested;
    }

    private void OnDisable()
    {
        _input.Requested -= OnPushRequested;
        _pushRequested = false;
    }

    private void FixedUpdate()
    {
        if (_pushRequested == false)
            return;

        _pushRequested = false;
        Push();
    }

    private void OnPushRequested()
    {
        _pushRequested = true;
    }

    private void Push()
    {
        Vector3 axis = transform.right;
        float angularSpeedAlongAxis = Vector3.Dot(_body.angularVelocity, axis);

        if (Mathf.Abs(angularSpeedAlongAxis) >= _maximumAngularSpeed)
            return;

        float direction = angularSpeedAlongAxis < 0f ? -1f : 1f;
        _body.AddTorque(axis * (_pushImpulse * direction), ForceMode.Impulse);
    }
}
