using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    private CharacterController _controller;
    private Animator _animator;
    [SerializeField]private StoryManager storyManager;
    [SerializeField]private SavedData savedData;
    [SerializeField] private UIManager uiManager;

    private Vector3 _playerVelocity;
    private bool _groundedPlayer;
    private Transform _cameraMainTransform;
    private Transform _tReference;
    
    //input action references
    [Header("Input Actions")]
    [SerializeField]private InputActionReference movementControl;
    [SerializeField]private InputActionReference jumpControl;
    [SerializeField]private InputActionReference sprintControl;
    [SerializeField]private InputActionReference crouchControl;
    [SerializeField]private InputActionReference sensesControl;
    [SerializeField]private InputActionReference interactControl;
    
    //variables
    [Header("Variables")]
    public float senses = 100f;
    public bool sensesActive;
    public float health = 5f;
    [SerializeField]private float stamina = 100f;
    [SerializeField]private float playerSpeed = 3.0f;
    [SerializeField]private float jumpHeight = 1.0f;
    [SerializeField]private float rotationSpeed = 5f;
    private const float GravityValue = -9.81f;
    private float _sprintSpeed = 1f;
    private float _crouchSpeed = 1f;
    private float _regenCooldown = 3f;
    private const float MaxRegenCooldown = 3f;
    private bool _canRegenHealth;
    private bool _startRegenCooldown;
    private int _canInteract;
    
    //animations references
    private static readonly int IsWalking = Animator.StringToHash("IsWalking");
    private static readonly int IsRunning = Animator.StringToHash("IsRunning");
    private static readonly int IsCrouching = Animator.StringToHash("IsCrouching");
    private static readonly int Jump = Animator.StringToHash("Jump");
    
    //player UI
    [Header("UI")]
    [SerializeField]private Slider staminaBar;
    [SerializeField]private Slider sensesBar;
    [SerializeField]private Image senseFilter;
    [SerializeField]private Image blood1;
    [SerializeField]private Image blood2;
    [SerializeField]private Text interactPopUp;

    [Header("Sound")] 
    [SerializeField] private AudioSource footSteps;

    private void Awake()
    {
        Time.timeScale = 1f;
        
        //if not new game then set the player position and quest line value to the saved one
        if (!savedData.isNewGame && savedData.playerPos != new Vector3(0,0,0))
        {
            transform.position = savedData.playerPos;
        }
        else
        {
            transform.position = new Vector3(47, 51, 29);
        }
        
        _tReference = new GameObject().transform; //crate new object to use as reference
        senseFilter.GetComponent<Image>().enabled = false; //set the grey filter invisible
        interactPopUp.GetComponent<Text>().enabled = false; //set the interact text invisible
    }

    private void Start()
    {
        _controller = gameObject.GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
        if (Camera.main != null) _cameraMainTransform = Camera.main.transform;
        
        //UI
        staminaBar.maxValue = 100f;
        sensesBar.maxValue = 100f;
    }

    private void Update()
    {
        //set player on ground
        _groundedPlayer = _controller.isGrounded;
        if (_groundedPlayer && _playerVelocity.y < 0)
        {
            _playerVelocity.y = 0f;
        }

        //set camera reference to combat slow when looking down
        _tReference.eulerAngles = new Vector3(0, _cameraMainTransform.eulerAngles.y, 0);
        
        //stamina and senses drain
        if (stamina < 100f && !sprintControl.action.IsPressed() && !crouchControl.action.IsPressed())
        {
            stamina += 20f * Time.deltaTime;
        }
        if (senses < 100f && !sensesActive)
        {
            senses += 10f * Time.deltaTime;
        }
        staminaBar.value = stamina;
        sensesBar.value = senses;

        //Movement
        #region Movement Code
        Vector2 movement = movementControl.action.ReadValue<Vector2>();
        Vector3 move = new Vector3(movement.x, 0, movement.y);
        move = _tReference.forward * move.z + _cameraMainTransform.right.normalized * move.x;
        move.y = 0;
        
        
        if (movement != Vector2.zero)
        {
            float targetAngle = Mathf.Atan2(movement.x, movement.y) * Mathf.Rad2Deg + _cameraMainTransform.eulerAngles.y;
            Quaternion rotation = Quaternion.Euler(0f, targetAngle, 0f);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * rotationSpeed);
            
            _animator.SetBool(IsWalking, true); //perform animation
            footSteps.enabled = true; //play sound
        }
        else
        {
            _animator.SetBool(IsWalking, false); //stop animation
            footSteps.enabled = false; //stop sound
        }
        
        //sprinting
        if (sprintControl.action.IsPressed() && movement != Vector2.zero && stamina > 0f)
        {
            _sprintSpeed = 3f;
            _animator.SetBool(IsRunning, true); //perform animation
            stamina -= 5f * Time.deltaTime; //drain stamina when sprinting
            
            footSteps.pitch = 1.7f; //speed up the footstep sound
        }
        else
        {
            _sprintSpeed = 1f;
            _animator.SetBool(IsRunning, false); //stop animation
            
            footSteps.pitch = 1f; //reset footstep sound
        }

        //crouching
        if (crouchControl.action.IsPressed() && _groundedPlayer && stamina > 0f)
        {
            _animator.SetBool(IsCrouching, true);
            _crouchSpeed = 0.4f;
            stamina -= 3f * Time.deltaTime; //drain stamina when crouching
            
            footSteps.pitch = 0.6f; //lower the footstep sound
        }
        else
        {
            _animator.SetBool(IsCrouching, false);
            _crouchSpeed = 1f;
            
            footSteps.pitch = 1f; //reset footstep sound
        }

        _controller.Move(move * (Time.deltaTime * playerSpeed * _sprintSpeed * _crouchSpeed));
        
        // jumping
        if (jumpControl.action.triggered && _groundedPlayer && !crouchControl.action.IsPressed() && stamina > 29f)
        {
            _playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * GravityValue);
            _animator.SetTrigger(Jump);
            stamina -= 15f; //drain stamina when jumping
        }
        
        _playerVelocity.y += GravityValue * Time.deltaTime;
        _controller.Move(_playerVelocity * Time.deltaTime);
        #endregion

        //senses toggle
        #region Dog Senses
        //active senses when X pressed, sense bar is full and senses not already active
        if (sensesControl.action.triggered && !sensesActive && senses >= 100f)
        {
            sensesActive = true;
        }
        //while senses active and sense bar not empty drain bar and set grey filter active
        if (sensesActive && senses > 0f)
        {
            senseFilter.GetComponent<Image>().enabled = true;
            senses -= 20f * Time.deltaTime;
        } 
        //set senses and grey filter as false and start regaining the bar when bar is drained
        else if (sensesActive && senses <= 0f)
        {
            sensesActive = false;
            senseFilter.GetComponent<Image>().enabled = false;
        }
        #endregion
        
        //health regen
        #region Health Regen
        if (_startRegenCooldown)
        {
            _regenCooldown -= Time.deltaTime;

            if (_regenCooldown <= 0)
            {
                _canRegenHealth = true;
                _startRegenCooldown = false;
            }
        }
        if (_canRegenHealth)
        {
            if (health < 5f)
            {
                health += 1;
                UpdateHealthSplatter();
                _canRegenHealth = false;
                _regenCooldown = MaxRegenCooldown;
                _startRegenCooldown = true;
            }
            else
            {
                health = 5f;
                UpdateHealthSplatter();
                _regenCooldown = MaxRegenCooldown;
                _canRegenHealth = false;
            }
        }

        //back to checkpoint if dead
        if (health <= 0f)
        {
            LoadPlayer();
            health = 5f;
            UpdateHealthSplatter();
        }
        #endregion

        //Interaction
        switch (interactControl.action.triggered)
        {
            case true when _canInteract == 1:
                storyManager.interacted = true;
                SavePlayer();
                break;
            case true when _canInteract == 2:
                storyManager.woundedInteracted = true;
                SavePlayer();
                break;
            default:
                storyManager.interacted = false;
                storyManager.woundedInteracted = false;
                break;
        }
    }

    private void OnEnable()
    {
        movementControl.action.Enable();
        jumpControl.action.Enable();
        sprintControl.action.Enable();
        crouchControl.action.Enable();
        sensesControl.action.Enable();
        interactControl.action.Enable();
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void OnDisable()
    {
        movementControl.action.Disable();
        jumpControl.action.Disable();
        sprintControl.action.Disable();
        crouchControl.action.Disable();
        sensesControl.action.Disable();
        interactControl.action.Disable();
        Cursor.lockState = CursorLockMode.Confined;
    }

    //Collision
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Bullet"))
        {
            health -= 1;
            //UI blood splatter
            UpdateHealthSplatter();
            StartCoroutine(HurtFlash());
            //set health regen cooldown
            _canRegenHealth = false;
            _regenCooldown = MaxRegenCooldown;
            _startRegenCooldown = true;
        }
        else if (other.gameObject.CompareTag("Bomb"))
        {
            health -= 3;
            //UI blood splatter
            StartCoroutine(HurtFlash());
            UpdateHealthSplatter();
            //set health regen cooldown
            _canRegenHealth = false;
            _regenCooldown = MaxRegenCooldown;
            _startRegenCooldown = true;
        }
        else if (other.gameObject.CompareTag("Wire"))
        {
            health -= 1;
            //UI blood splatter
            StartCoroutine(HurtFlash());
            UpdateHealthSplatter();
            //set health regen cooldown
            _canRegenHealth = false;
            _regenCooldown = MaxRegenCooldown;
            _startRegenCooldown = true;

            other.gameObject.GetComponent<AudioSource>().Play();
        }
        else if (other.gameObject.CompareTag("NPC")) //set canInteract true
        {
            interactPopUp.GetComponent<Text>().enabled = true; //show text
            _canInteract = 1;
        }
        else if (other.gameObject.CompareTag("WoundedNPC")) //set canInteract true
        {
            interactPopUp.GetComponent<Text>().enabled = true; //show text
            _canInteract = 2;
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        Debug.Log("trigger exit");
        interactPopUp.GetComponent<Text>().enabled = false;
        _canInteract = 0;
    }

    private void UpdateHealthSplatter()
    {
        Color blood1Alpha = Color.red;
        Color blood2Alpha = Color.red;

        //Health splatter colors
        #region Health to Blood Alpha Mapping
        if (health < 1f)
        {
            blood1Alpha = Color.black;
            blood2Alpha = Color.black;
            blood1Alpha.a = 1f;
            blood2Alpha.a = 1f;
        }
        else if (health < 2f)
        {
            blood1Alpha = Color.black;
            blood2Alpha = new Color(1f, 0f, 0f, 0.8f);
        }
        else if (health < 3f)
        {
            blood1Alpha = Color.red;
            blood2Alpha = new Color(1f, 0f, 0f, 0.3f);
        }
        else if (health < 4f)
        {
            blood1Alpha = new Color(1f, 0f, 0f, 0.8f);
            blood2Alpha.a = 0f;
        }
        else if (health < 5f)
        {
            blood1Alpha = new Color(1f, 0f, 0f, 0.3f);
            blood2Alpha.a = 0f;
        }
        else
        {
            blood1Alpha.a = 0f;
            blood2Alpha.a = 0f;
        }
        #endregion Health to Blood Alpha Mapping

        blood1.color = blood1Alpha;
        blood2.color = blood2Alpha;
    }

    private IEnumerator HurtFlash()
    {
        for (int i = 0; i < 4; i++)
        {
            blood1.enabled = !blood1.enabled;
            blood2.enabled = !blood2.enabled;
            yield return new WaitForSeconds(0.05f);
        }
        blood1.enabled = true;
        blood2.enabled = true;
    }

    private void SavePlayer()
    {
        savedData.playerPos = transform.position;
        savedData.questLine = storyManager.questLine;

        savedData.isNewGame = false;
    }

    public void LoadPlayer()
    {
        transform.position = savedData.playerPos;
        storyManager.questLine = savedData.questLine;
        
        uiManager.Resume();
    }
}