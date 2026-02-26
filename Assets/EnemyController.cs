using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private Animator animator;
    private bool isHit = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void TakeHit()
    {
        if (!isHit)
            StartCoroutine(HitReaction());
    }

    System.Collections.IEnumerator HitReaction()
    {
        isHit = true;
        animator.SetBool("IsHit", true);

        // Wait for flinch animation to finish
        yield return new WaitForSeconds(1f);

        animator.SetBool("IsHit", false);
        isHit = false;
    }
}
