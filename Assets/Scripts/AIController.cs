using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class AIController : MonoBehaviour
{
    public int npcForQuest;
    public NavMeshAgent navMeshAgent;
    public Animator animator;
    [SerializeField] private StoryManager storyManager;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject npc;

    private float thisToPlayer;
    private float thisToNpc;
    private static readonly int IsWalking = Animator.StringToHash("IsWalking");

    private void Start()
    {
        if (npcForQuest > 0)
        {
            gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (!gameObject.activeSelf) return;
        switch (npcForQuest)
        {
            case 1 when storyManager.questLine == 2:
                gameObject.transform.position = Vector3.up * 700f;
                StartCoroutine(DestroyDelay());
                break;
            case 2 when storyManager.questLine == 5:
                gameObject.transform.position = Vector3.up * 700f;
                StartCoroutine(DestroyDelay());
                break;
            case 3 when storyManager.questLine == 7:
                gameObject.transform.position = Vector3.up * 700f;
                StartCoroutine(DestroyDelay());
                break;
            case 4 when storyManager.questLine == 8:
                gameObject.transform.position = Vector3.up * 700f;
                StartCoroutine(DestroyDelay());
                break;
            case 6 when storyManager.questLine == 12:
                gameObject.transform.position = Vector3.up * 700f;
                StartCoroutine(DestroyDelay());
                break;
            case 7 when storyManager.questLine == 13:
                gameObject.transform.position = Vector3.up * 700f;
                StartCoroutine(DestroyDelay());
                break;
            case 8 when storyManager.questLine == 15:
                gameObject.transform.position = Vector3.up * 700f;
                StartCoroutine(DestroyDelay());
                break;
            case 9 when storyManager.questLine == 16:
                gameObject.transform.position = Vector3.up * 700f;
                StartCoroutine(DestroyDelay());
                break;
        }

        if ((npcForQuest != 5 || storyManager.questLine != 10) &&
            (npcForQuest != 10 || storyManager.questLine != 17)) return;
        thisToNpc = Vector3.Distance(transform.position, npc.transform.position);
        thisToPlayer = Vector3.Distance(transform.position, player.transform.position);
        
        navMeshAgent.SetDestination(player.transform.position);
        if (navMeshAgent.velocity.magnitude > 0)
        {
            animator.SetBool(IsWalking, true);
        }
        
        if (thisToNpc < 15f && thisToPlayer < 15f)
        {
            GetComponent<BoxCollider>().enabled = false;
            storyManager.woundedHere = true;
        }
        else
        {
            GetComponent<BoxCollider>().enabled = true;
            storyManager.woundedHere = false;
        }
    }

    private IEnumerator DestroyDelay()
    {
        yield return new WaitForSeconds(1);
        gameObject.SetActive(false);
    }
}
