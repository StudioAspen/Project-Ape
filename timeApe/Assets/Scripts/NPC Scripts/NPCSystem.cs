using System.Collections;
using TMPro;
using UnityEngine;

public class NPCSystem : MonoBehaviour, IInteractable
{
    [SerializeField] private SphereCollider interactCollider;
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI NPCNameText;
    public GameObject dialoguePanel;
    public string dialogueLine;
    public string NPCName;
    public Transform playerTransform;
    public float rotationSpeed = 5f;
    private Coroutine lookAtCoroutine;
    private bool inRange;
    private bool isTalking;

    void Start()
    {
        isTalking = false;
        inRange = false;
        dialoguePanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (inRange && Input.GetKeyDown(KeyCode.E)) //input will change when input action is created for interaction
        {
            Interact();
        }
    }

    public void StartDialogue()
    {
        dialoguePanel.SetActive(true);
        dialogueText.text = dialogueLine;
        NPCNameText.text = NPCName;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            inRange = true;
            CanInteract();
            
            //when within trigger collider, turn and face player
            if (playerTransform == null) 
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null) playerTransform = player.transform;
            }
            if (playerTransform != null)
            {
                if (lookAtCoroutine != null) StopCoroutine(lookAtCoroutine);

                lookAtCoroutine = StartCoroutine(LookAtPlayer());
            }

            StartDialogue(); //delete when input system is set up for interaction
        }
    }

    private IEnumerator LookAtPlayer()
    {
        //calculates target direction
        Vector3 targetDirection = playerTransform.position - transform.position;
        targetDirection.y = 0;

        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

        while (Quaternion.Angle(transform.rotation, targetRotation) > 0.1f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            yield return null;
        }

        transform.rotation = targetRotation;
    }

    public bool CanInteract()
    {
        return !isTalking;
    }

    public void Interact()
    {
        StartDialogue();
    }
}
