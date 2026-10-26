using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(SpringJoint))]
public class Catapult : MonoBehaviour
{
    private readonly WaitForFixedUpdate _physicsStep = new WaitForFixedUpdate();

    [SerializeField] private CommandInput _fireInput;
    [SerializeField] private CommandInput _reloadInput;
    [SerializeField] private ProjectileLoader _loader;
    [SerializeField, Range(0f, 90f)] private float _launchAngle = 65f;
    [SerializeField, Min(0f)] private float _restAngleTolerance = 1f;
    [SerializeField, Min(0f)] private float _restAngularSpeedTolerance = 0.15f;

    private Rigidbody _body;
    private SpringJoint _spring;
    private Quaternion _restRotation;
    private Vector3 _restAnchor;
    private Vector3 _launchAnchor;
    private Coroutine _reloadRoutine;
    private bool _isLoaded;

    private void Awake()
    {
        _body = GetComponent<Rigidbody>();
        _spring = GetComponent<SpringJoint>();
        _restRotation = _body.rotation;
        _restAnchor = _spring.connectedAnchor;

        Quaternion launchRotation = _restRotation * Quaternion.AngleAxis(_launchAngle, Vector3.right);
        _launchAnchor = _body.position + launchRotation * _spring.anchor;
    }

    private void OnEnable()
    {
        _fireInput.Requested += OnFireRequested;
        _reloadInput.Requested += OnReloadRequested;
        OnReloadRequested();
    }

    private void OnDisable()
    {
        _fireInput.Requested -= OnFireRequested;
        _reloadInput.Requested -= OnReloadRequested;

        if (_reloadRoutine != null)
        {
            StopCoroutine(_reloadRoutine);
            _reloadRoutine = null;
        }

        _isLoaded = false;
    }

    private void OnFireRequested()
    {
        if (_isLoaded == false)
            return;

        _isLoaded = false;
        _spring.connectedAnchor = _launchAnchor;
        _body.WakeUp();
    }

    private void OnReloadRequested()
    {
        if (_reloadRoutine != null)
            return;

        _isLoaded = false;
        _spring.connectedAnchor = _restAnchor;
        _body.WakeUp();
        _reloadRoutine = StartCoroutine(ReloadWhenResting());
    }

    private IEnumerator ReloadWhenResting()
    {
        do
        {
            yield return _physicsStep;
        }
        while (Quaternion.Angle(_body.rotation, _restRotation) > _restAngleTolerance
            || _body.angularVelocity.magnitude > _restAngularSpeedTolerance);

        _loader.Reload();
        _isLoaded = true;
        _reloadRoutine = null;
    }
}
