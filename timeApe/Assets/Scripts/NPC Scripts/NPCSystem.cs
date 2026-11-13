using System.Collections;
using TMPro;
using UnityEngine;

public class NPCSystem : MonoBehaviour, IInteractable
{
    public static NPCSystem Instance;
    [SerializeField] private SphereCollider interactCollider;
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI NPCNameText;
    public GameObject dialoguePanel;
    private GameObject player;
    public string dialogueLine;
    public string NPCName;
    public Transform playerTransform;
    public float rotationSpeed = 5f;
    private Coroutine lookAtCoroutine;
    public bool inRange;
    public bool isTalking;


    void Start()
    {
        if (Instance == null) Instance = this;
        isTalking = false;
        inRange = false;
        dialoguePanel.SetActive(false);
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        if (inRange == true)
        {
            if (playerTransform == null) 
            {
                if (player != null) playerTransform = player.transform;
            }
            if (playerTransform != null)
            {
                if (lookAtCoroutine != null) StopCoroutine(lookAtCoroutine);

                lookAtCoroutine = StartCoroutine(LookAtPlayer());
            }
        }
    }
    public void StartDialogue()
    {
        player.GetComponent<PlayerController>().enabled = false; //disables player movement
        dialoguePanel.SetActive(true);
        dialogueText.text = dialogueLine;
        NPCNameText.text = NPCName;
    }

    public void EndDialogue()
    {
        player.GetComponent<PlayerController>().enabled = true; //enables player movement
        StopAllCoroutines();
        dialoguePanel.SetActive(false);
        dialogueText.text = "";
        NPCNameText.text = "";
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            inRange = true;
            CanInteract();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            inRange = false;
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
