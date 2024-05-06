using UnityEngine;

public class BulletController : MonoBehaviour
{
    public float speed = 55f;

    private Rigidbody _rb;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        _rb.MovePosition(transform.position + Vector3.back * (speed * Time.deltaTime));
    }

    private void OnTriggerEnter(Collider other)
    {
        gameObject.SetActive(false);
    }
}