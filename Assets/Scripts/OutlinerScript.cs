using UnityEngine;

public class OutlinerScript : MonoBehaviour
{
    public GameObject playerObject;
    private PlayerController _player;

    private void Start()
    {
        playerObject = GameObject.Find("Player");
        _player = playerObject.GetComponent<PlayerController>();
    }

    private void FixedUpdate()
    {
        if (_player.sensesActive)
        {
            gameObject.layer = 6;
        }
        else
        {
            gameObject.layer = 3;
        }
    }
}
