using System.Collections;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal.Internal;
using Vector3 = UnityEngine.Vector3;


public class SpearScript : MonoBehaviour
{
    
    private Rigidbody targetEnemyRb;
    private Transform playerTransform;
    private Transform targetEnemyTransform;
    private Vector3 targetDestination;
    private Vector3 enemyStartPosition;
    private Quaternion enemyStartRotation;
    private float offset = 5f;
    private float heightOffset = 2.0f;
    private bool isPulling;
    private float pullTimer = 0f;
    private float duration = 0.7f;

    AnimationCurve curve = new AnimationCurve(
        new Keyframe(0f, 0f, 0f, 0f),
        new Keyframe(0.15f, 0.031f, 0.5f, -0.5f),
        new Keyframe(0.32f, 0.125f, 1f, -1f),
        new Keyframe(0.5f, 0.5f, 5.5f, 5.5f),
        new Keyframe(0.68f, 0.875f, 1f, -1f),
        new Keyframe(0.85f, 0.969f, 0.5f, -0.5f),
        new Keyframe(1f, 1f, 0f, 0f)
    );
    
    public void Initialize(Transform shootTransform) //this is used in the player controller to grab the players transform
    {
        playerTransform = shootTransform;
    }


    void OnCollisionEnter(Collision collision)
    {   
        Rigidbody spearRb = GetComponent<Rigidbody>();
        spearRb.isKinematic = true; 

        Enemy enemyComponent = collision.gameObject.GetComponent<Enemy>(); // gets information of any enemy with the enemy script
        StartCoroutine(Waiter());

        if (enemyComponent != null && !isPulling)
        {   
            targetEnemyTransform = enemyComponent.transform; //gets the start position of the specific enemy we hit as the collision starts
            targetEnemyRb = targetEnemyTransform.GetComponent<Rigidbody>();
            enemyStartPosition = targetEnemyTransform.position; //this will be the location of exactly where they were when hit

            Vector3 euler = targetEnemyTransform.eulerAngles;// forces rotation to be upright * doesnt work that well
            enemyStartRotation = Quaternion.Euler(0f,euler.y,0f);

            Vector3 directionToPlayer = (playerTransform.position - enemyStartPosition).normalized; // gets the direction of the enemy in relation to the player
            


            targetDestination = playerTransform.position - (directionToPlayer * offset) + (Vector3.up * heightOffset); //apply offsets


            isPulling = true;
            
            
            if (spearRb != null)
            {
                spearRb.isKinematic = true;
                spearRb.useGravity = false;
                spearRb.linearVelocity = Vector3.zero;

            }

            if (targetEnemyRb != null)
            {
                targetEnemyRb.isKinematic = true;
                targetEnemyRb.freezeRotation = true;
            }

            
            transform.SetParent(targetEnemyTransform);
        }

    }
    
    IEnumerator Waiter()
    {
        yield return new WaitForSeconds(3);
        Object.Destroy(this.gameObject);
    }
    
     void FixedUpdate()
    {
        if (isPulling && targetEnemyTransform != null && playerTransform != null)
        {
            pullTimer += Time.deltaTime;

            float t = pullTimer / duration;
            
            t = Mathf.Clamp01(t);

            float value = curve.Evaluate(t);

            Vector3 newPosition = Vector3.Lerp(enemyStartPosition, targetDestination, value);

                if (targetEnemyRb != null)
            {
                targetEnemyRb.MovePosition(newPosition);
                targetEnemyRb.MoveRotation(enemyStartRotation);
            }
            else
            {
                targetEnemyTransform.position = newPosition;
                targetEnemyTransform.rotation = enemyStartRotation;
            }
                if (t >= 1f)
            {
                Rigidbody enemyRb = targetEnemyTransform.GetComponent<Rigidbody>();
                if (enemyRb != null)
                {
                    enemyRb.isKinematic = false;
                    enemyRb.useGravity = true;
                    enemyRb.freezeRotation = false;
                    Vector3 euler = targetEnemyTransform.eulerAngles;// forces rotation to be upright * doesnt work that well
                    enemyRb.rotation = Quaternion.Euler(0f,euler.y,0f);
                    enemyRb.linearVelocity = Vector3.zero;
                }

               

                Destroy(gameObject);
            }
        }

        
    }
}

