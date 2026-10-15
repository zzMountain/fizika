using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(HingeJoint), typeof(SpringJoint))]
public class Catapult : MonoBehaviour
{
    [SerializeField] private ProjectileLoader _loader;
    [SerializeField, Min(0f)] private float _launchSpring = 120f;
    [SerializeField, Min(0f)] private float _launchDamper = 8f;
    [SerializeField, Min(0f)] private float _launchDistance = 0.1f;
    [SerializeField, Min(0f)] private float _relaxedDistance = 20f;
    [SerializeField, Min(0f)] private float _returnSpeed = 80f;
    [SerializeField, Min(0f)] private float _returnForce = 1800f;
    [SerializeField, Min(0f)] private float _restAngleTolerance = 1f;
    [SerializeField, Min(0f)] private float _restSpeedTolerance = 0.15f;

    private Rigidbody _body;
    private HingeJoint _hinge;
    private SpringJoint _spring;
    private Quaternion _restRotation;
    private bool _isLoaded;
    private bool _isReturning;

    private void Awake()
    {
        _body = GetComponent<Rigidbody>();
        _hinge = GetComponent<HingeJoint>();
        _spring = GetComponent<SpringJoint>();
        _restRotation = _body.rotation;
    }

    private void Start()
    {
        _loader.Reload();
        _isLoaded = true;
    }

    private void FixedUpdate()
    {
        if (_isReturning == false)
            return;

        if (Quaternion.Angle(_body.rotation, _restRotation) > _restAngleTolerance)
            return;

        if (_body.angularVelocity.magnitude > _restSpeedTolerance)
            return;

        _hinge.useMotor = false;
        _isReturning = false;
        _loader.Reload();
        _isLoaded = true;
    }

    public void Fire()
    {
        if (_isLoaded == false || _isReturning)
            return;

        _isLoaded = false;
        _spring.maxDistance = _launchDistance;
        _spring.spring = _launchSpring;
        _spring.damper = _launchDamper;
        _body.WakeUp();
    }

    public void Reload()
    {
        if (_isReturning)
            return;

        _isLoaded = false;
        _isReturning = true;
        _spring.spring = 0f;
        _spring.damper = 0f;
        _spring.maxDistance = _relaxedDistance;

        JointMotor motor = _hinge.motor;
        motor.targetVelocity = -_returnSpeed;
        motor.force = _returnForce;
        motor.freeSpin = false;
        _hinge.motor = motor;
        _hinge.useMotor = true;
        _body.WakeUp();
    }
}
