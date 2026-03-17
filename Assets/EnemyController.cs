using Unity.VisualScripting.FullSerializer;
using UnityEditor.Rendering.Universal.ShaderGUI;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public Transform player;
    public float detectionRange = 8f;
    public float attackRange = 1.5f;

    private Vector3 originalPosition;
    private Vector3 patrolPosition;
    private bool patrolFlag = true;
    private NavMeshAgent agent;
    private Animator animator;
    private bool isHit = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        originalPosition = transform.position;
        patrolPosition = originalPosition + transform.forward * 5.0f;

        Debug.Log(originalPosition);
        Debug.Log(patrolPosition);
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null) return;

        // Debug.Log(agent.remainingDistance);

        float distanceToPlayer =
            Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer < detectionRange && distanceToPlayer > attackRange)
        {
            // Chase the player
            if (Vector3.Angle(transform.forward, player.position - transform.position) <= 60f)
            {
                agent.isStopped = false;
                agent.SetDestination(player.position);
            }
        }
        else if (distanceToPlayer <= attackRange)
        {
            // Close enough — stop and face player
            FacePlayer();
        }
        else
        {
           // agent.isStopped = false;

            if (agent.remainingDistance <= 1)
            {
                // agent.SetDestination(patrolFlag ? patrolPosition : originalPosition);
                // patrolFlag = !patrolFlag;
            }

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
