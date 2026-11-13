using UnityEngine;
using UnityEngine.InputSystem;

public class CombatComponent : MonoBehaviour
{
    
    private CharacterController timeApe;
    [Header("References")]
    public GameObject TempSpear;
    public Transform attackPoint;
    public Transform target;
    public Transform cameraTransform;
    [Header("Throwing Stats")]
    public float throwForce;
    public float throwCooldown;
    public float throwUpwardForce;
    public float spearHeightOffset = 2.5f;
    public bool readyToThrow;
    public string enemyTag = "Enemy";
    [Header("Camera Lock On Settings")]
    public float lockOnSpeed = 10f;
    public float lockOnRange = 20f;
    // input actions
    private InputAction m_throw;
    private InputAction m_aim;

    
    void Awake()
    {
        readyToThrow = true;
        timeApe = gameObject.GetComponent<CharacterController>();
        InvokeRepeating("EnemyCheck", 0, 0.5f);
        m_throw = InputSystem.actions.FindAction("Throw");
        m_aim = InputSystem.actions.FindAction("Aim");
    }

    void Update()
    {
        
        if(m_throw.WasPressedThisFrame() && readyToThrow) // throws spear
        {
            Attack();
        }
        if(m_aim.IsPressed() && target != null) // applies to the camera rotation so fits here more than the update function
        {
            Debug.Log("aiming");    
           CameraLockOn(); // handles the camera lock on
        }
        if (target == null)
        {
            return;
        }
    }
    void Attack() // SHOOTS SPEAR
    {

        readyToThrow = false; //so it cannot be spammed
        GameObject spear = Instantiate(TempSpear, attackPoint.position, cameraTransform.rotation); //this is what creates the spear at the attackpoint
        Rigidbody spearRb = spear.GetComponent<Rigidbody>(); //this grabs the rigidbody component of the spear
        SpearScript spearScript = spear.GetComponent<SpearScript>();

        Vector3 forceDirection = cameraTransform.transform.forward;
        RaycastHit hit;



        
        if (Physics.Raycast(cameraTransform.position,cameraTransform.forward, out hit, 500f, 8)) // using raycast for more accurate shooting outside the lock-on
        {
            forceDirection = (hit.point - attackPoint.position).normalized;
        }
        
        Invoke(nameof(ResetThrow), throwCooldown); // starts resetthrow function in whatever time is set for throwcooldown

        if (spearScript != null)
        {
            spearScript.Initialize(this.transform);
        }
        
        if (timeApe.isGrounded)
        {
            
            spearRb.useGravity = true;
        }
        else
        {
            spearRb.useGravity = false;
        }
        if (target != null)
        {
            Vector3 dir = (target.position - attackPoint.position).normalized * throwForce + (Vector3.up * spearHeightOffset);
            Quaternion lookRotation = Quaternion.LookRotation(dir);
            Vector3 rotation = lookRotation.eulerAngles;
            spearRb.rotation = lookRotation;
            
            spearRb.AddForce(dir,ForceMode.Impulse);

        }
        else
        {
            Vector3 forceToAdd = forceDirection * throwForce + transform.up * throwUpwardForce; //force for the spear's rigidbody
            spearRb.AddForce(forceToAdd, ForceMode.Impulse);
        }
    }

    private void ResetThrow() //resets the throw
    {
        readyToThrow = true;
    }

    void OnDrawGizmosSelected()
    {
        //for testing the range of the lock on
        //Gizmos.color = Color.red;
        //Gizmos.DrawWireSphere(transform.position, lockOnRange);
    }

    void EnemyCheck()
    {   

        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag); // grabs gameobjects with enemy tag
        float shortestDistance = Mathf.Infinity;
        GameObject nearestEnemy = null;

        foreach (GameObject enemy in enemies)
        {
            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
            if (distanceToEnemy < shortestDistance) // calculates the closest enemy
            {
                shortestDistance = distanceToEnemy;
                nearestEnemy = enemy;
            }
        }
        if (nearestEnemy != null && shortestDistance <= lockOnRange)
        {
            target = nearestEnemy.transform;
            Debug.Log("found"); //assigns target from the result of the above if statement and can be used anywhere on this script
        }
        else
        {
            target = null;
        }
    }

    void CameraLockOn()
    {
        Vector3 aimDir = (target.position - transform.position).normalized;
        Quaternion rotation = Quaternion.LookRotation(aimDir);
        cameraTransform.rotation = Quaternion.Lerp(cameraTransform.rotation,rotation,lockOnSpeed);
    }
}
