using UnityEngine;
using Random = UnityEngine.Random;

namespace Bullet
{
    public class BulletSpawner : MonoBehaviour
    {
        [SerializeField] private float timeToSpawn = 1f;
        public Vector3 size;
    
        private float _timeSinceSpawn;
        private BulletPool _bulletPool;

        private void Start()
        {
            _bulletPool = FindObjectOfType<BulletPool>();
        }

        private void Update()
        {
            _timeSinceSpawn += Time.deltaTime;
            if (_timeSinceSpawn >= timeToSpawn)
            {
                var newBullet = _bulletPool.GetBullet();
                newBullet.transform.position = this.transform.position + new Vector3(Random.Range(-size.x / 2, size.x / 2), Random.Range(-size.y / 2, size.y / 2), Random.Range(-size.z / 2, size.z / 2));
                _timeSinceSpawn = 0f;
            }
        }

        private void OnDrawGizmosSelected()
        {
            Color c = Color.blue;
            c.a = 0.3f;
            Gizmos.color = c;
            Gizmos.DrawCube(transform.position, size);
        }
    }
}
