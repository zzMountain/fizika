using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class CommandInput : MonoBehaviour
{
    [SerializeField] private InputAction _action = new InputAction(type: InputActionType.Button);

    public event Action Requested;

    private void OnEnable()
    {
        _action.performed += OnPerformed;
        _action.Enable();
    }

    private void OnDisable()
    {
        _action.Disable();
        _action.performed -= OnPerformed;
    }

    private void OnDestroy()
    {
        _action.Dispose();
    }

    private void OnPerformed(InputAction.CallbackContext context)
    {
        Requested?.Invoke();
    }
}
