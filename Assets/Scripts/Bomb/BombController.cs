using UnityEngine;

public class BombController: MonoBehaviour
{
    public float speed = 30f;
    private Rigidbody _rb;
    public ParticleSystem explosion;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        _rb.MovePosition(transform.position + Vector3.down * (speed * Time.deltaTime));
    }

    private void OnTriggerEnter(Collider other)
    {
        Instantiate(explosion, transform.position, Quaternion.identity);
        gameObject.SetActive(false);
    }
}
