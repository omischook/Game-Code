using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding; 

public class Enemy : MonoBehaviour
{
    public Animator animator;
    public int maxHealth = 100;
    int currentHealth;
    [Header("Enemy Combat")]
    new public Transform transform;

    public float moveThreshold = 0.01f;

    private Vector3 lastPosition;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();

        InvokeRepeating("UpdatePath", 0f, pathUpdateSeconds);

        lastPosition = transform.position;

        transform = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {


        if ((transform.position - lastPosition).magnitude > moveThreshold)
        {
            animator.SetBool("isWalking", true);
        }
        else
        {
            animator.SetBool("isWalking", false);
        }

        lastPosition = transform.position;

        
    }
    public void TakeDamage(int Damage)
    {
        currentHealth -= Damage;

        animator.SetTrigger("Hurt");
        //Play hurt animation

        if(currentHealth <= 0)
        {
            Die();

            
        }
    }
    void Die()
    {
        Collider2D[] colliders = GetComponents<Collider2D>();
        Debug.Log("Enemy died");
        animator.SetBool("Dead", true);

        foreach (Collider2D collider in colliders)
        {
            if (!collider.isTrigger)
            {
                collider.enabled = false;
            }
        }
        //GetComponent<Collider2D>().enabled = false; 
        //this.enabled = false;
        //Die animation

        //disable the enemy
    }

    [Header("Pathfinding")]
    public Transform target;
    public float activateDistance = 50f;
    public float pathUpdateSeconds;
    [Header("Physics")]
    public float speed = 200f;
    public float nextWaypointDistance = 3f;
    public float jumpNodeHeightRequirement = 0.8f;
    public float jumpModifier = 0.3f;
    public float jumpCheckOffset = 0.1f;

    [Header("Custom Behaviour")]
    public bool followEnabled = true;
    public bool jumpEnabled = true;
    public bool directionLookEnabled = true;

    private Path path;
    private int currentWaypoint = 0;
    bool isGrounded = false;
    Seeker seeker;
    Rigidbody2D rb;

    private void FixedUpdate()
    {
        if(TargetInDistance() && followEnabled)
        {
            PathFollow();
        }
    }

    private void UpdatePath()
    {
        if(followEnabled && TargetInDistance() && seeker.IsDone())
        {
            seeker.StartPath(rb.position, target.position, OnPathComplete);
        }


    }
    private void PathFollow()
    {
        if(path == null)
        {
            return;
        }

        // Reached end of path
        if(currentWaypoint >= path.vectorPath.Count)
        {
            return;
        }

        //check for collision
        Vector3 startOffset = transform.position - new Vector3(0f, GetComponent<Collider2D>().bounds.extents.y + jumpCheckOffset);
        isGrounded = Physics2D.Raycast(startOffset, -Vector3.up, 0.05f);

        isGrounded = Physics2D.Raycast(transform.position, -Vector3.up, GetComponent<Collider2D>().bounds.extents.y + jumpCheckOffset);
        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
        Vector2 force = direction * speed * Time.deltaTime;

        if(jumpEnabled && isGrounded)
        {
            if(direction.y > jumpNodeHeightRequirement)
            {
                rb.AddForce(Vector2.up * speed * jumpModifier);
            }
        }

        rb.AddForce(force);

        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);
        if (distance < nextWaypointDistance)
        {
            currentWaypoint++;
        }

        if (directionLookEnabled)
        {
            if(rb.velocity.x > 0.05f)
            {
                transform.localScale = new Vector3(-1f * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
            else if(rb.velocity.x < -0.05f)
            {
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
        }
    }
    
    private bool TargetInDistance()
    {
        return Vector2.Distance(transform.position, target.transform.position) < activateDistance;
    }

    private void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }

    private bool isPlayerInside = false;
    public LifeAndDeath lifeAndDeath;
    private float attackDelay = 1.0f;
    public PlayerCombat playerCombat;
    public bool parried = false;
    //this is where enemy attacks are controlled from
    private void OnTriggerStay2D(Collider2D collision)
    {
        BoxCollider boxCollider = GetComponent<BoxCollider>();
        if (collision.CompareTag("Player"))
            {
            //Debug.Log("player found");
            isPlayerInside = true;
            Attack();
            }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            {
            //Debug.Log("player gone");
            isPlayerInside = false;
            animator.SetBool("Attack", false);
        }
    }
    private void Attack()
    {
        if ( Time.time % attackDelay == 0 && parried == false)
        {
            animator.SetBool("Attack", true);
            if(isPlayerInside == true && playerCombat.parry == false)
                {
                lifeAndDeath.TakeDamage(1);
                }
            else if(isPlayerInside == true && playerCombat.parry == true)
            {
                Debug.Log("parried nerd");
                StartCoroutine(Stunned());
            }
        }
        
        
    }

    private IEnumerator Stunned()
    {
        Debug.Log("Delay starting");
        animator.SetBool("Attack", false);
        animator.SetBool("Hurt", true);
        parried = true;
        yield return new WaitForSecondsRealtime(3);
        Debug.Log("delay over");
        parried = false;
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, activateDistance);
    }
}
