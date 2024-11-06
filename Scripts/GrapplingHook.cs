using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingHook : MonoBehaviour
{
    public Camera cam;
    public LineRenderer lr;

    public LayerMask grappleMask;//What you can grapple to
    public float moveSpeed = 2;//Speed when grappling
    public float grappleLength = 10;//Unit length of grapple
    public int maxPoints = 1;//Number of hooks

    public Transform groundCheck;
    public float groundCheckRadius;
    public LayerMask groundLayer;
    public bool isTouchingGround;

    private Rigidbody2D rig;//Needed for physics
    private List<Vector2> points = new List<Vector2>();//Needed for physics
    // Start is called before the first frame update
    void Start()
    {
        rig = GetComponent<Rigidbody2D>();
        lr.positionCount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        isTouchingGround = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        if (Input.GetMouseButtonDown(0) && isTouchingGround != true)//detects for mouse input & performs ground check
        {
            Vector2 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
            //Above makes mouse position on screen a worldpoint detectable by game
            Vector2 direction = (mousePos - (Vector2)transform.position).normalized;

            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, grappleLength, grappleMask);
            if (hit.collider != null)
            {
                Vector2 hitPoint = hit.point;
                points.Add(hitPoint);

                if (points.Count > maxPoints)
                {
                    points.RemoveAt(0);
                }
            }
        }

        if (points.Count > 0)
        {
            Vector2 moveTo = centroid(points.ToArray());

            rig.MovePosition(Vector2.MoveTowards(transform.position, moveTo, Time.deltaTime * moveSpeed));

            lr.positionCount = 0;
            lr.positionCount = points.Count * 2;
            for (int n = 0, j = 0; n < points.Count * 2; n += 2, j++)
            {
                lr.SetPosition(n, transform.position);
                lr.SetPosition(n + 1, points[j]);
            }
        }
        //this causes our player to detach from the grappling hook and lets them move again
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Detach();
        }
    }
    //this is the detach statements that clears all of the points that we are attached to
    void Detach()
    {
        lr.positionCount = 0;
        points.Clear();
    }
    // this allows us to find the center of multiple points if we choose to allow more than one point 
    Vector2 centroid(Vector2[] points)
    {
        Vector2 center = Vector2.zero;
        foreach (Vector2 point in points)
        {
            center += point;
        }
        center /= points.Length;
        return center;
    }
}
