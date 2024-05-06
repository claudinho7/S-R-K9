using Bullet;
using UnityEngine;

public class BulletReturn : MonoBehaviour
{
    private BulletPool _bulletPool;

    private void Start()
    {
        _bulletPool = FindObjectOfType<BulletPool>();
    }

    private void OnDisable()
    {
        if (_bulletPool != null)
        {
            _bulletPool.ReturnBullet(this.gameObject);
        }
    }
}
