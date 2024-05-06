using UnityEngine;

public class BombReturn : MonoBehaviour
{
    private BombPool _bombPool;

    private void Start()
    {
        _bombPool = FindObjectOfType<BombPool>();
    }

    private void OnDisable()
    {
        if (_bombPool != null)
        {
            _bombPool.ReturnBomb(this.gameObject);
        }
    }
}
