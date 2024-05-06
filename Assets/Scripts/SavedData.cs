using UnityEngine;

[CreateAssetMenu(fileName = "SavedData", menuName = "Variables")]
public class SavedData : ScriptableObject
{
    public int questLine;
    public Vector3 playerPos;
    public bool isNewGame;
}
