using UnityEngine;
using UnityEngine.InputSystem;

public class JointDemoInput : MonoBehaviour
{
    [SerializeField] private Swing _swing;
    [SerializeField] private Catapult _catapult;
    [SerializeField] private InputAction _push = new InputAction("Push", InputActionType.Button, "<Keyboard>/space");
    [SerializeField] private InputAction _fire = new InputAction("Fire", InputActionType.Button, "<Keyboard>/f");
    [SerializeField] private InputAction _reload = new InputAction("Reload", InputActionType.Button, "<Keyboard>/r");

    private bool _pushRequested;
    private bool _fireRequested;
    private bool _reloadRequested;

    private void OnEnable()
    {
        _push.performed += OnPush;
        _fire.performed += OnFire;
        _reload.performed += OnReload;
        _push.Enable();
        _fire.Enable();
        _reload.Enable();
    }

    private void OnDisable()
    {
        _push.Disable();
        _fire.Disable();
        _reload.Disable();
        _push.performed -= OnPush;
        _fire.performed -= OnFire;
        _reload.performed -= OnReload;
        _pushRequested = false;
        _fireRequested = false;
        _reloadRequested = false;
    }

    private void FixedUpdate()
    {
        if (_pushRequested)
            _swing.Push();

        if (_reloadRequested)
            _catapult.Reload();
        else if (_fireRequested)
            _catapult.Fire();

        _pushRequested = false;
        _fireRequested = false;
        _reloadRequested = false;
    }

    private void OnDestroy()
    {
        _push.Dispose();
        _fire.Dispose();
        _reload.Dispose();
    }

    private void OnPush(InputAction.CallbackContext context)
    {
        _pushRequested = true;
    }

    private void OnFire(InputAction.CallbackContext context)
    {
        _fireRequested = true;
    }

    private void OnReload(InputAction.CallbackContext context)
    {
        _reloadRequested = true;
    }
}
