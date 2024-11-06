using UnityEngine;

public class Tutorial_GrapplingGun : MonoBehaviour
{
    [Header("Scripts Ref:")]
    public Tutorial_GrapplingRope grappleRope;

    [Header("Layers Settings:")]
    [SerializeField] public LayerMask grappleMask;

    [Header("Main Camera:")]
    public Camera m_camera;

    [Header("Transform Ref:")]
    public Transform gunHolder;
    public Transform firePoint;

    [Header("Physics Ref:")]
    public SpringJoint2D m_springJoint2D;
    public Rigidbody2D m_rigidbody;

    [Header("Distance:")]
    [SerializeField] private bool hasMaxDistance = false;
    [SerializeField] private float maxDistnace = 20;

    private enum LaunchType
    {
        Transform_Launch,
        Physics_Launch
    }

    [Header("Launching:")]
    [SerializeField] private bool launchToPoint = true;
    [SerializeField] private LaunchType launchType = LaunchType.Physics_Launch;
    [SerializeField] private float launchSpeed = 1;

    [Header("No Launch To Point")]
    [SerializeField] private bool autoConfigureDistance = false;
    [SerializeField] private float targetDistance = 3;
    [SerializeField] private float targetFrequncy = 1;

    [HideInInspector] public Vector2 grapplePoint;
    [HideInInspector] public Vector2 grappleDistanceVector;

    private void Start()
    {
        grappleRope.enabled = false; //disables both of these until they are needed
        m_springJoint2D.enabled = false;

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            SetGrapplePoint();
        }
        else if (Input.GetKey(KeyCode.Mouse0))
        {

        }
        else if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            grappleRope.enabled = false;
            m_springJoint2D.enabled = false;
            m_rigidbody.gravityScale = 1;
        }
    }



    void SetGrapplePoint()
    {
        Vector2 distanceVector = m_camera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (distanceVector - (Vector2)firePoint.position).normalized;
        
        RaycastHit2D _hit = Physics2D.Raycast(firePoint.position, direction, maxDistnace, grappleMask);
        if (_hit.collider != null)
        {
         grapplePoint = _hit.point;
         grappleDistanceVector = grapplePoint -(Vector2)firePoint.position;
         grappleRope.enabled = true;//this set of 3 lines begins to throw the rope 
                
            
        }
    }

    public void Grapple()
    {
        m_springJoint2D.autoConfigureDistance = false;//since autoconfigure is a rare use case we default it off and can activate it later
        if (!launchToPoint && !autoConfigureDistance)
        {
            m_springJoint2D.distance = targetDistance;
            m_springJoint2D.frequency = targetFrequncy;
        }
        if (!launchToPoint)// this decides whether we want to use autoconfigure distance
        {
            if (autoConfigureDistance)
            {
                m_springJoint2D.autoConfigureDistance = true;
                m_springJoint2D.frequency = 0;
            }

            m_springJoint2D.connectedAnchor = grapplePoint;// sets anchor to grpple point
            m_springJoint2D.enabled = true;
        }
        else
        {
            switch (launchType)
            {
                case LaunchType.Physics_Launch:
                    m_springJoint2D.connectedAnchor = grapplePoint;

                    Vector2 distanceVector = firePoint.position - gunHolder.position;// distance vector of fire point to player pos.

                    m_springJoint2D.distance = distanceVector.magnitude;// the following lines use the spring joint magnitude
                    m_springJoint2D.frequency = launchSpeed;// to create a launchspeed momentum
                    m_springJoint2D.enabled = true;// and enable the springjoint
                    break;
                case LaunchType.Transform_Launch:// because transform launch is a set speed we need to zero everything and use player movement
                    m_rigidbody.gravityScale = 0;
                    m_rigidbody.velocity = Vector2.zero;
                    break;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (firePoint != null && hasMaxDistance)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(firePoint.position, maxDistnace);
            Gizmos.DrawLine(firePoint.position, grapplePoint);
        }
    }

}
