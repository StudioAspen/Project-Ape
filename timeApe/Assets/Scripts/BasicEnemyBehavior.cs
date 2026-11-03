using System.Numerics;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AI;
using Vector3 = UnityEngine.Vector3;
public class EnemyBehavior : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Transform goal;
    public NavMeshAgent agent;
    private Transform self;
    private float normal_speed;
    private float normal_acceleration;

    private bool isAttacking = false;
    private Vector3 attack_position = Vector3.zero;
    private Collider Attack_Range_Box;
    private float attack_range;
    [SerializeField] public int detect_range = 15;
    [SerializeField] public int cooldown = 2;
    [SerializeField] public int lunge_speed = 20;

    private bool isLunging = false;

    private bool isLoitering = false;
    private Vector3 Loitering_destination = Vector3.zero;
    [SerializeField] public float loitering_radius = 5;
    [SerializeField] public float loitering_speed = 0.8f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        self = GetComponent<Transform>();
        Attack_Range_Box = GetComponent<Collider>();
        attack_range = 3;
        normal_speed = agent.speed;
        normal_acceleration = agent.acceleration;
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(self.position, goal.position) <= detect_range && !isAttacking)
        {
            Chasing();
        }
        else if (isLunging)
        {
            if (Vector3.Distance(self.position, agent.destination) <= agent.stoppingDistance)
            {
                isLunging = false;
                isAttacking = false;
                Debug.Log("Lunge Ended");
                agent.destination = self.position;
                agent.speed = normal_speed;
                agent.acceleration = normal_acceleration;
            }
        }
        else if (!isAttacking)
        {   
            if (!isLoitering)
            {
                Loitering_destination = Loitering();
            }
            else if (Vector3.Distance(self.position, Loitering_destination) <= agent.stoppingDistance)
            {
                isLoitering = false;
                agent.speed = normal_speed;
                agent.acceleration = normal_acceleration;
            }
        }
    }

    void Chasing()
    {   
        isLoitering = false;
        Debug.Log("I see you!");
        agent.speed = normal_speed;
        agent.acceleration = normal_acceleration;

        agent.destination = goal.position;
        self.LookAt(goal);
        Debug.Log($"Distance: {Vector3.Distance(self.position, goal.position)}, attack range: {attack_range}");

        if (Vector3.Distance(self.position, goal.position) <= attack_range && !isAttacking)
        {
            Debug.Log("Attack!");
            isAttacking = true;
            agent.destination = self.position;
            Invoke(nameof(Attack), cooldown);
        }

    }
    void Attack()
    {
        Debug.Log("Attack!");
        attack_position = self.forward * attack_range;

        if (Physics.BoxCast(Attack_Range_Box.bounds.center, Attack_Range_Box.bounds.extents, self.forward, out RaycastHit rayHit, UnityEngine.Quaternion.identity, attack_range))
        {
            attack_position = self.forward * rayHit.distance;
        }

        if (NavMesh.SamplePosition(self.position + attack_position, out NavMeshHit navHit, attack_range, NavMesh.AllAreas) && Vector3.Distance(self.position, goal.position) <= (attack_range * 1.5f))
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
        Vector3 candidate = self.position + offset;
        if (NavMesh.SamplePosition(candidate, out NavMeshHit navHit, loitering_radius, NavMesh.AllAreas))
        {
            agent.destination = navHit.position;
            isLoitering = true;
            return navHit.position;
        }
        else
        {
            return self.position;
        }
    }
}