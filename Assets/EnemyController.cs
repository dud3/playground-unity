using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public Transform player;
    public float detectionRange = 8f;
    public float attackRange = 1.5f;

    private NavMeshAgent agent;
    private Animator animator;
    private bool isHit = false;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = 
            Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer < detectionRange && distanceToPlayer > attackRange)
        {
            // Chase the player
            agent.isStopped = false;
            agent.SetDestination(player.position);
        }
        else if (distanceToPlayer <= attackRange)
        {
            // Close enough — stop and face player
            agent.isStopped = true;
            FacePlayer();
        }
        else
        {
            agent.isStopped = true;
        }

        Vector3 localVelocity = transform.InverseTransformDirection(agent.velocity);
        animator.SetFloat("VelocityZ", localVelocity.z, 0.1f, Time.deltaTime);
    }

    void FacePlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0;

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            Quaternion.LookRotation(direction),
            Time.deltaTime * 5f
        );
    }

    public void TakeHit()
    {
        if (!isHit)
            StartCoroutine(HitReaction());
    }

    System.Collections.IEnumerator HitReaction()
    {
        isHit = true;
        agent.isStopped = true;
        animator.SetBool("IsHit", true);

        // Wait for flinch animation to finish
        yield return new WaitForSeconds(1f);

        animator.SetBool("IsHit", false);
        agent.isStopped = false;
        isHit = false;
    }
}
