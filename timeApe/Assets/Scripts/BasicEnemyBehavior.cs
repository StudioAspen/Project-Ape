using UnityEngine;
using UnityEngine.AI;
public class EnemyBehavior : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Transform goal;
    public NavMeshAgent agent;
    private Transform self_pos;

    private float normal_speed;
    private float normal_acceleration;

    public bool isAttacking = false;
    private Vector3 attack_position = Vector3.zero;
    private float attack_range = 3;
    [SerializeField] public int detect_range = 15;
    [SerializeField] public float attack_cooldown = 0.5f;
    [SerializeField] public int lunge_speed = 10;
    public bool isLunging = false;

    public bool isStunned = false; // This variable is used to check if the enemy is stunned so that the other things can't happen. 
    private float stun_cooldown = 2.0f;


    public bool isLoitering = false;
    private Vector3 Loitering_destination;
    [SerializeField] public float loitering_radius = 5;
    [SerializeField] public float loitering_speed = 0.8f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        self_pos = GetComponent<Transform>();
        goal = GameObject.FindGameObjectWithTag("Player").transform;

        normal_speed = agent.speed;
        normal_acceleration = agent.acceleration;
        Loitering_destination = self_pos.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(self_pos.position, goal.position) <= detect_range && !isAttacking && !isStunned)
        {
            Chasing();
        }
        else if (isLunging && !isStunned)
        {
            if (Vector3.Distance(self_pos.position, agent.destination) <= agent.stoppingDistance)
            {
                isLunging = false;
                isAttacking = false;
                Debug.Log("Lunge Ended");
                agent.destination = self_pos.position;
                agent.speed = normal_speed;
                agent.acceleration = normal_acceleration;
            }
        }
        else if (!isAttacking && !isStunned)
        {   
            if (!isLoitering)
            {
                Loitering_destination = Loitering();
            }
            else if (Vector3.Distance(self_pos.position, Loitering_destination) <= agent.stoppingDistance)
            {
                isLoitering = false;
                agent.speed = normal_speed;
                agent.acceleration = normal_acceleration;
            }
        }
        else if (isStunned && !IsInvoking(nameof(Recover)))
        {
            agent.isStopped = true;
            agent.destination = self_pos.position;
            agent.speed = normal_speed;
            agent.acceleration = normal_acceleration;
            Invoke(nameof(Recover), stun_cooldown);
        }
    }
    void Recover()
    {
        Debug.Log("Recovered!");
        isStunned = false;
        agent.isStopped = false;
    }
    
    void Chasing()
    {   
        isLoitering = false;
        Debug.Log("I see you!");
        agent.speed = normal_speed;
        agent.acceleration = normal_acceleration;

        agent.destination = goal.position;
        self_pos.LookAt(goal);
        Debug.Log($"Distance: {Vector3.Distance(self_pos.position, goal.position)}, attack range: {attack_range}");

        if (Vector3.Distance(self_pos.position, goal.position) <= attack_range && !isAttacking && !isStunned && !IsInvoking(nameof(Attack)))
        {
            Debug.Log("Attack!");
            isAttacking = true;
            agent.destination = self_pos.position;
            Invoke(nameof(Attack), attack_cooldown);
        }

    }
    void Attack()
    {
        Debug.Log("Attack!");
        attack_position = self_pos.forward * attack_range;

        if (Physics.Raycast(self_pos.position, self_pos.forward, out RaycastHit rayHit, attack_range))
        {
            attack_position = self_pos.forward * rayHit.distance;
        }

        if (NavMesh.SamplePosition(self_pos.position + attack_position, out NavMeshHit navHit, attack_range, NavMesh.AllAreas) && Vector3.Distance(self_pos.position, goal.position) <= (attack_range * 2f))
        {
            Debug.Log("Enemy Bites");
            agent.acceleration = 100;
            agent.speed = lunge_speed;
            agent.destination = navHit.position;
            isLunging = true;
        }
        else
        {
            isAttacking = false;
            agent.speed = normal_speed;
            agent.acceleration = normal_acceleration;
        }
    }
    Vector3 Loitering()
    {
        Debug.Log("I'm wandering around!");
        agent.speed = loitering_speed;
        agent.acceleration = normal_acceleration;
        Vector3 offset = Random.insideUnitSphere * loitering_radius;
        Vector3 candidate = self_pos.position + offset;
        if (NavMesh.SamplePosition(candidate, out NavMeshHit navHit, loitering_radius, NavMesh.AllAreas))
        {
            agent.destination = navHit.position;
            isLoitering = true;
            return navHit.position;
        }
        else
        {
            return self_pos.position;
        }
    }
}