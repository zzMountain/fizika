using UnityEngine;

[RequireComponent(typeof(CharacterController), typeof(MovementInput))]
public class PlayerMover : MonoBehaviour
{
    [SerializeField, Min(0f)] private float _speed = 5f;
    [SerializeField] private float _gravity = -20f;
    [SerializeField] private float _groundedVelocity = -2f;

    private CharacterController _controller;
    private MovementInput _input;
    private float _verticalVelocity;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _input = GetComponent<MovementInput>();
    }

    private void Update()
    {
        if (_controller.isGrounded && _verticalVelocity < 0f)
        {
            _verticalVelocity = _groundedVelocity;
        }

        _verticalVelocity += _gravity * Time.deltaTime;
        Vector2 inputDirection = _input.Direction;
        Vector3 velocity = new Vector3(inputDirection.x * _speed, _verticalVelocity, inputDirection.y * _speed);
        _controller.Move(velocity * Time.deltaTime);
    }

    private void OnDisable()
    {
        _verticalVelocity = 0f;
    }
}
