using UnityEngine;
using UnityEngine.AI;
public class EnemyBehavior : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Transform goal;
    public NavMeshAgent agent;
    private Transform self_pos;
    private CapsuleCollider self_collision;

    private float normal_speed;
    private float normal_acceleration;

    private bool isAttacking = false;
    private Vector3 attack_position = Vector3.zero;
    private float attack_range;
    [SerializeField] public int detect_range = 15;
    [SerializeField] public float attack_cooldown = 0.5f;
    [SerializeField] public int lunge_speed = 10;
    private bool isLunging = false;

    private bool isStunned = false;
    private float stun_cooldown = 2.0f;


    private bool isLoitering = false;
    private Vector3 Loitering_destination = Vector3.zero;
    [SerializeField] public float loitering_radius = 5;
    [SerializeField] public float loitering_speed = 0.8f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        self_pos = GetComponent<Transform>();
        self_collision = GetComponent<CapsuleCollider>();

        attack_range = 3;
        normal_speed = agent.speed;
        normal_acceleration = agent.acceleration;
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
        else if (self_collision.CompareTag("Player") && !isAttacking)
        {
            Invoke(nameof(OnTriggerEnter), 0.5f);
        }
        else if (isStunned)
        {
            Invoke(nameof(Recover), stun_cooldown);
        }
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Owie!");
            isStunned = true;
            isAttacking = false;
            isLunging = false;
            isLoitering = false;
            agent.acceleration = 100;
            agent.speed = 0;
        }
    }
    void Recover()
    {
        Debug.Log("Recovered!");
        isStunned = false;
        agent.speed = normal_speed;
        agent.acceleration = normal_acceleration;
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

        if (Vector3.Distance(self_pos.position, goal.position) <= attack_range && !isAttacking && !isStunned)
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

        if (NavMesh.SamplePosition(self_pos.position + attack_position, out NavMeshHit navHit, attack_range, NavMesh.AllAreas) && Vector3.Distance(self_pos.position, goal.position) <= (attack_range * 1.5f))
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