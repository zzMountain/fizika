using UnityEngine;

[RequireComponent(typeof(RigidbodyMover))]
public class BotFollower : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField, Min(0f)] private float _stoppingDistance = 2f;

    private RigidbodyMover _mover;

    private void Awake()
    {
        _mover = GetComponent<RigidbodyMover>();
    }

    private void FixedUpdate()
    {
        if (_target == null)
        {
            _mover.Move(Vector3.zero, 0f);
            return;
        }

        Vector3 offset = _target.position - transform.position;
        offset.y = 0f;
        float remainingDistance = Mathf.Max(0f, offset.magnitude - _stoppingDistance);
        _mover.Move(offset.normalized, remainingDistance);
    }

    private void OnDisable()
    {
        _mover.Stop();
    }
}
