using UnityEngine;
using UnityEngine.InputSystem;

public class InteractController : MonoBehaviour
{
    private NPCSystem npcScript;
    public bool isInteracting = false;
    private PlayerInput inputActions;

    void Awake()
    {
        inputActions = new PlayerInput();
    }
    void OnEnable()
    {
        inputActions.Enable();

        inputActions.Player.Interact.performed += ctx => isInteracting = !isInteracting;
    }
    void OnDisable()
    {
        inputActions.Disable();
    }
    void Start()
    {
        npcScript = NPCSystem.Instance;
    }
    void Update()
    {
        if (npcScript.inRange == true)
        {
            if (isInteracting == true)
            {
                npcScript.Interact();
            }
            if (isInteracting == false)
            {
                npcScript.EndDialogue();
            }
        }
    }
}
