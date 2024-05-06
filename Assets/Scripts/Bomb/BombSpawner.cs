using UnityEngine;
using Random = UnityEngine.Random;

public class BombSpawner : MonoBehaviour
{
    [SerializeField] private float timeToSpawn = 2f;
    public Vector3 size;
    
    private float _timeSinceSpawn;
    private BombPool _bombPool;

    private void Start()
    {
        _bombPool = FindObjectOfType<BombPool>();
    }

    private void Update()
    {
        _timeSinceSpawn += Time.deltaTime;
        if (_timeSinceSpawn >= timeToSpawn)
        {
            var newBomb = _bombPool.GetBomb();
            newBomb.transform.position = this.transform.position + new Vector3(Random.Range(-size.x / 2, size.x / 2), Random.Range(-size.y / 2, size.y / 2), Random.Range(-size.z / 2, size.z / 2));
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
