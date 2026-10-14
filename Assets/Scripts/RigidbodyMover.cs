using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
public class RigidbodyMover : MonoBehaviour
{
    [SerializeField, Min(0f)] private float _speed = 3f;
    [SerializeField, Min(0f)] private float _stepHeight = 0.3f;
    [SerializeField, Range(0f, 89f)] private float _slopeLimit = 45f;
    [SerializeField] private LayerMask _groundMask = 1;
    [SerializeField, Min(0.01f)] private float _groundProbeHeight = 0.3f;
    [SerializeField, Min(0.01f)] private float _groundProbeDistance = 0.5f;
    [SerializeField, Min(0.01f)] private float _stepProbeHeight = 0.06f;
    [SerializeField, Min(0.01f)] private float _stepProbePadding = 0.15f;
    [SerializeField, Min(0.001f)] private float _surfacePadding = 0.02f;

    private Rigidbody _body;
    private CapsuleCollider _capsule;

    private void Awake()
    {
        _body = GetComponent<Rigidbody>();
        _capsule = GetComponent<CapsuleCollider>();
    }

    private void OnDisable()
    {
        Stop();
    }

    public void Move(Vector3 direction, float remainingDistance)
    {
        if (isActiveAndEnabled == false)
        {
            return;
        }

        float speed = Mathf.Min(_speed, remainingDistance / Time.fixedDeltaTime);
        Vector3 feet = _body.position + _capsule.center - Vector3.up * (_capsule.height * 0.5f);
        Vector3 velocity = direction * speed;
        bool grounded = Physics.Raycast(feet + Vector3.up * _groundProbeHeight, Vector3.down,
            out RaycastHit groundHit, _groundProbeDistance, _groundMask, QueryTriggerInteraction.Ignore);

        if (grounded && Vector3.Angle(groundHit.normal, Vector3.up) <= _slopeLimit)
        {
            velocity = Vector3.ProjectOnPlane(velocity, groundHit.normal).normalized * speed;

            if (groundHit.distance > _groundProbeHeight + _surfacePadding)
            {
                velocity.y = Mathf.Min(velocity.y, _body.linearVelocity.y);
            }

            if (speed > 0f)
            {
                TryClimbStep(feet, direction, speed);
            }
        }
        else
        {
            velocity.y = _body.linearVelocity.y;
        }

        _body.linearVelocity = velocity;
    }

    public void Stop()
    {
        _body.linearVelocity = Vector3.zero;
    }

    private void TryClimbStep(Vector3 feet, Vector3 direction, float speed)
    {
        float probeDistance = _capsule.radius + speed * Time.fixedDeltaTime + _stepProbePadding;
        Vector3 lowerOrigin = feet + Vector3.up * _stepProbeHeight;
        Vector3 upperOrigin = feet + Vector3.up * (_stepHeight + _surfacePadding);

        if (Physics.Raycast(lowerOrigin, direction, probeDistance, _groundMask,
            QueryTriggerInteraction.Ignore) == false)
        {
            return;
        }

        if (Physics.Raycast(upperOrigin, direction, probeDistance, _groundMask, QueryTriggerInteraction.Ignore))
        {
            return;
        }

        Vector3 topOrigin = upperOrigin + direction * probeDistance;

        if (Physics.Raycast(topOrigin, Vector3.down, out RaycastHit topHit, _stepHeight,
            _groundMask, QueryTriggerInteraction.Ignore) == false)
        {
            return;
        }

        float rise = topHit.point.y - feet.y;

        if (rise > _surfacePadding && rise <= _stepHeight &&
            Vector3.Angle(topHit.normal, Vector3.up) <= _slopeLimit)
        {
            _body.MovePosition(_body.position + Vector3.up * (rise + _surfacePadding));
        }
    }
}
