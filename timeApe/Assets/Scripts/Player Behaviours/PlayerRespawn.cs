using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    private Vector3 checkpointPos; 
    private float maxFallDistance = -120f; 

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("Checkpoint"))
        {
            Vector3 newCheckpointPosition = transform.position; //or the position of the checkpoint object
            checkpointPos = newCheckpointPosition;
            Debug.Log("Checkpoint reached!");
        }

        if (hit.gameObject.CompareTag("water")) //added for dying when touching water
        {
            Respawn();
        }
    }

    public void Respawn()
    {
        transform.position = checkpointPos;
        //velocity = Vector3.zero;
    }
}
