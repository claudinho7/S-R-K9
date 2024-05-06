using UnityEngine;

public class BloodTrailScript : MonoBehaviour
{
    public PlayerController player;

    private Vector3 _curPos;
    private Vector3 _tarPos;
    

    private void FixedUpdate()
    {
        if (player.sensesActive)
        {
            gameObject.GetComponent<TrailRenderer>().enabled = true;

            _tarPos = transform.parent.position;
            _tarPos.y = Terrain.activeTerrain.SampleHeight(_tarPos) + 2f;

            _curPos = transform.position;
            _curPos.y = Terrain.activeTerrain.SampleHeight(_curPos) + 2f;
            
            transform.position = Vector3.MoveTowards(_curPos, _tarPos, .5f);
        }
        else
        {
            gameObject.GetComponent<TrailRenderer>().enabled = false;
            
            transform.position = player.transform.position;
        }
    }
}
