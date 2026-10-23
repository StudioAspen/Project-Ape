using System.Collections;
using TMPro;
using UnityEngine;

public class NPCSystem : MonoBehaviour, IInteractable
{
    [SerializeField] private SphereCollider interactCollider;
    [Header("Dialogue UI References")]
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI NPCNameText;
    public GameObject dialoguePanel;

    [Space]
    public string dialogueLine;
    public string NPCName;
    public Transform playerTransform;
    public float rotationSpeed = 5f;
    private Coroutine lookAtCoroutine;
    private bool inRange;
    private bool isTalking;
    private bool isFacingPlayer;

    void Start()
    {
        isTalking = false;
        isFacingPlayer = false;
        inRange = true;
        dialoguePanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (inRange && Input.GetKeyDown(KeyCode.E))
        {
            Interact();
        }
    }

    public void StartDialogue()
    {
        if (isTalking && !isFacingPlayer)
        {
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
        }
        dialoguePanel.SetActive(true);
        dialogueText.text = dialogueLine;
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
        isFacingPlayer = true;
    }

    public bool CanInteract()
    {
        return !isTalking;
    }

    public void Interact()
    {
        Debug.Log("interacting");
        StartDialogue();
    }
}
