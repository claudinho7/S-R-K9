using System.Collections.Generic;
using UnityEngine;

public class BombPool : MonoBehaviour
{
    [SerializeField]private GameObject bombPrefab;

    private Queue<GameObject> _bombsPool = new Queue<GameObject>();
    private const int PoolStartSize = 20;

    private void Start()
    {
        for (int i = 0; i < PoolStartSize; i++)
        {
            GameObject bomb = Instantiate(bombPrefab);
            _bombsPool.Enqueue(bomb);
            bomb.SetActive(false);
        }
    }

    public GameObject GetBomb()
    {
        if (_bombsPool.Count > 0)
        {
            GameObject bomb = _bombsPool.Dequeue();
            bomb.SetActive(true);
            return bomb;
        }
        else
        {
            GameObject bomb = Instantiate(bombPrefab);
            return bomb;
        }
    }

    public void ReturnBomb(GameObject bomb)
    {
        _bombsPool.Enqueue(bomb);
        bomb.SetActive(false);
    }
}
