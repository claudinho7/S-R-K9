using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoryManager : MonoBehaviour
{
    public SavedData savedData;
    
    public bool interacted;
    public bool woundedInteracted;
    
    public GameObject[] woundedSoldiers;
    private readonly Dictionary<int, List<GameObject>> _objectsByPublicInt = new Dictionary<int, List<GameObject>>();

    public int questLine;
    private bool _questActive;
    [SerializeField] private Text questLog;

    public bool woundedHere;

    private void Awake()
    {
        if (savedData.isNewGame)
        {
            questLine = 0;
        }
        else
        {
            questLine = savedData.questLine;
        }
    }

    private void Start()
    {
        // group game objects by public int value
        foreach (GameObject obj in woundedSoldiers)
        {
            AIController aiController = obj.GetComponent<AIController>();
            int publicIntValue = aiController.npcForQuest;
            if (!_objectsByPublicInt.ContainsKey(publicIntValue))
            {
                _objectsByPublicInt.Add(publicIntValue, new List<GameObject>());
            }
            _objectsByPublicInt[publicIntValue].Add(obj);
        }
    }

    private void Update()
    {
        //QuestLine progression
        switch (questLine)
        {
            case 0:
                if (interacted)
                {
                    Debug.Log("starting quest 1");
                    questLine = 1;
                    _questActive = true;
                }
                break;
            case 1:
                SetActiveObjectsWithPublicInt(1, true);
                if (woundedInteracted)
                {
                    Debug.Log("Dead soldier found");
                    questLine = 2;
                }
                break;
            case 2:
                //SetActiveObjectsWithPublicInt(1, false);
                if (interacted)
                {
                    Debug.Log("starting quest 2.1");
                    questLine = 3;
                }
                break;
            case 3:
                SetActiveObjectsWithPublicInt(2, true);
                if (woundedInteracted)
                {
                    Debug.Log("First 2 members found");
                    questLine = 4;
                }
                break;
            case 4:
                SetActiveObjectsWithPublicInt(2, true);
                if (woundedInteracted)
                {
                    Debug.Log("starting quest 2.2");
                    questLine = 5;
                }
                break;
            case 5:
                //SetActiveObjectsWithPublicInt(2, false);
                SetActiveObjectsWithPublicInt(3, true);
                if (woundedInteracted)
                {
                    Debug.Log("2 more members found");
                    questLine = 6;
                }
                break;
            case 6:
                SetActiveObjectsWithPublicInt(3, true);
                if (woundedInteracted)
                {
                    Debug.Log("starting quest 2.3");
                    questLine = 7;
                }
                break;
            case 7:
                //SetActiveObjectsWithPublicInt(3, false);
                SetActiveObjectsWithPublicInt(4, true);
                if (woundedInteracted)
                {
                    Debug.Log("Last member found. Return");
                    questLine = 8;
                }
                break;
            case 8:
                //SetActiveObjectsWithPublicInt(4, false);
                if (interacted)
                {
                    Debug.Log("starting quest 3");
                    questLine = 9;
                }
                break;
            case 9:
                SetActiveObjectsWithPublicInt(5, true);
                if (woundedInteracted)
                {
                    Debug.Log("Soldier found. Guiding back");
                    questLine = 10;
                }
                break;
            case 10:
                SetActiveObjectsWithPublicInt(5, true);
                if (woundedHere && interacted) //must add a check to see if soldier is with dog here
                {
                    Debug.Log("starting quest 4.1");
                    questLine = 11;
                }
                break;
            case 11:
                SetActiveObjectsWithPublicInt(5, false);
                SetActiveObjectsWithPublicInt(6, true);
                if (woundedInteracted)
                {
                    Debug.Log("starting quest 4.2");
                    questLine = 12;
                }
                break;
            case 12:
                //SetActiveObjectsWithPublicInt(6, false);
                SetActiveObjectsWithPublicInt(7, true);
                if (woundedInteracted)
                {
                    Debug.Log("squad found, return");
                    questLine = 13;
                }
                break;
            case 13:
                //SetActiveObjectsWithPublicInt(7, false);
                if (interacted)
                {
                    Debug.Log("starting quest 5.1");
                    questLine = 14;
                }
                break;
            case 14:
                SetActiveObjectsWithPublicInt(8, true);
                if (woundedInteracted)
                {
                    Debug.Log("starting quest 5.2");
                    questLine = 15;
                }
                break;
            case 15:
                //SetActiveObjectsWithPublicInt(8, false);
                SetActiveObjectsWithPublicInt(9, true);
                if (woundedInteracted)
                {
                    Debug.Log("starting quest 5.3");
                    questLine = 16;
                }
                break;
            case 16:
                //SetActiveObjectsWithPublicInt(9, false);
                SetActiveObjectsWithPublicInt(10, true);
                if (woundedInteracted)
                {
                    Debug.Log("soldier found, guide back");
                    questLine = 17;
                }
                break;
            case 17:
                SetActiveObjectsWithPublicInt(10, true);
                if (woundedHere && interacted) //add check to see if soldier is with dog
                {
                    Debug.Log("quest line finished");
                    questLine = 18;
                    SetActiveObjectsWithPublicInt(10, false);
                }
                break;
            default:
                if (_questActive && (interacted || woundedInteracted))
                {
                    Debug.Log("quest already started");
                }
                break;
        }
        
        
        //QuestLog update
        questLog.GetComponent<Text>().text = questLine switch
        {
            0 => "Find your officer and pickup your first task. He is the one in front of the medical tent. If you forgot your training, press Esc and read the manual.",
            1 => "Your first mission as a rescue Dog is to find a wounded Soldier. He is not far, i just heard him screaming for help.",
            2 => "That's is him alright, but you're too late, he is already dead. Report back to your officer.",
            3 => "There has been a massive bomb explosion where Delta Squad is supposed to be. Track them down and see if they require medical help.",
            4 => "Looks like they explosion got them scattered all over the place, these 2 are injured but they will be ok. Interact with them again to signal the rescue team to pick them up.",
            5 => "Find the rest of the Delta Squad. 2/5 Rescued.",
            6 => "That's 4 so far. Signal the rescue team to pick these up.",
            7 => "Search for the last Delta Squad member. Hurry, his teammates are worried. 4/5 Rescued.",
            8 => "You're too late, he is dead. Return to your officer.",
            9 => "The commander of Foxtrot squad told us that one of his soldiers got lost somewhere on the front line. Find him and use your abilities to guide him back to safety.",
            10 => "He is now fallowing you closely. Be careful at the enemy fire, you must keep him safe, guide him back to camp.",
            11 => "Good job bringing this guy back, we will take care of him now. Unfortunately, his job was to warn Charlie Squad about a massive enemy push. You must take this letter to them immediately so they can retreat before it's too late.",
            12 => "This dead soldier is part of Charlie Squad, you must be close. Keep searching.",
            13 => "Charlie Squad is in full retreat. Return to your officer quickly for your next mission.",
            14 => "While you were on your last mission, one of our planes crashed behind enemy lines. You are the only one who can sneak there and bring the pilots back, that if they are still alive. Go quick, try to find them and guide them back home.",
            15 => "This is the crash site, and one of the pilots is dead. The other one must have catapulted somewhere in the area, try to track him down before the enemy soldiers find him.",
            16 => "This must be where he landed, and he killed this guy while at it, surely he is close. Keep searching.",
            17 => "The soldier is wounded and moving slow, but he is fallowing you. Guide him past the enemy far back to camp",
            18 => "Great job, surely you will receive a medal after this, and many many treats. You can close the game, this is where the story ends... for now.",
            _ => questLog.GetComponent<Text>().text
        };
        
    }

    private void SetActiveObjectsWithPublicInt(int publicIntValue, bool active)
    {
        // activate game objects with a specific public int value
        if (_objectsByPublicInt.ContainsKey(publicIntValue))
        {
            foreach (GameObject obj in _objectsByPublicInt[publicIntValue])
            {
                obj.SetActive(active);
            }
        }
    }
}
