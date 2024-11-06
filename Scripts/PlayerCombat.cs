using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public Transform groundCheck;
    public float groundCheckRadius;
    public LayerMask groundLayer;
    public bool isTouchingGround;

    public Animator animator;

    public Transform attackPoint;
    public int attackdamage = 40;
    public float attackRange = 0.5f;
    public LayerMask enemyLayers;

    public float attackRate = 2f;
    float nextAttackTime = 0f;
    public bool blocking = false;
    private float parryWindow = 10f;
    private float startTime;
    public bool parry = true;
    public int parryCount = 0;
    // Start is called before the first frame update
    void Start()
    {
        startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time >= nextAttackTime)
        {

            if (Input.GetMouseButtonDown(0) && isTouchingGround == true && blocking == false)
            {
                Attack();
                nextAttackTime = Time.time + 1f / attackRate;
            }
        }
        if (Input.GetMouseButtonDown(1) && isTouchingGround == true)
        {
            if (Time.time - startTime < parryWindow && parryCount < 1)
            {
                blocking = true;
                parry = true;
                Debug.Log("Successful parry well done");
                animator.SetBool("Blocking", true);
                parryCount++;
            }
            else
            {
                parry = false;
                blocking = true;
                animator.SetBool("Blocking", true);
            }
        }
        if (Input.GetMouseButtonUp(1) == true)
        {
            blocking = false;
            parry = false;
            animator.SetBool("Blocking", false);
            parryCount--;
        }
    }
    void Attack()
    {
        // Play an attack animation
        animator.SetTrigger("Attack");
        // detect all enemies in range 
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);
        // damage and knock back enemies
        foreach(Collider2D enemy in hitEnemies)
        {
            enemy.GetComponent<Enemy>().TakeDamage(attackdamage);
        }
    }
void OnDrawGizmosSelected()
{
    if (attackPoint == null)
        return;

    Gizmos.DrawWireSphere(attackPoint.position, attackRange);
}
}

