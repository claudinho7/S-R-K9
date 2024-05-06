using System.Collections.Generic;
using UnityEngine;

namespace Bullet
{
    public class BulletPool : MonoBehaviour
    {
        [SerializeField]private GameObject bulletPrefab;
        private readonly Queue<GameObject> _bulletsPool = new Queue<GameObject>();
        private const int PoolStartSize = 40;

        private void Start()
        {
            for (int i = 0; i < PoolStartSize; i++)
            {
                GameObject bullet = Instantiate(bulletPrefab);
                _bulletsPool.Enqueue(bullet);
                bullet.SetActive(false);
            }
        }

        public GameObject GetBullet()
        {
            if (_bulletsPool.Count > 0)
            {
                GameObject bullet = _bulletsPool.Dequeue();
                bullet.SetActive(true);
                return bullet;
            }
            else
            {
                GameObject bullet = Instantiate(bulletPrefab);
                return bullet;
            }
        }

        public void ReturnBullet(GameObject bullet)
        {
            _bulletsPool.Enqueue(bullet);
            bullet.SetActive(false);
        }
    }
}
