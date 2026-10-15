using UnityEngine;

public class ProjectileLoader : MonoBehaviour
{
    [SerializeField] private Rigidbody _prefab;
    [SerializeField] private Transform _socket;

    private Rigidbody _projectile;

    private void OnDestroy()
    {
        if (_projectile != null)
            Destroy(_projectile.gameObject);
    }

    public void Reload()
    {
        if (_projectile != null)
        {
            _projectile.gameObject.SetActive(false);
            Destroy(_projectile.gameObject);
        }

        _projectile = Instantiate(_prefab, _socket.position, _socket.rotation);
    }
}
