using UnityEngine;

public class Tutorial_GrapplingRope : MonoBehaviour
{
    [Header("General Refernces:")]
    public Tutorial_GrapplingGun grapplingGun;
    public LineRenderer m_lineRenderer;

    [Header("General Settings:")]
    [SerializeField] private int percision = 40;
    [Range(0, 20)] [SerializeField] private float straightenLineSpeed = 5;

    [Header("Rope Animation Settings:")]
    public AnimationCurve ropeAnimationCurve;
    [Range(0.01f, 4)] [SerializeField] private float StartWaveSize = 2;
    float waveSize = 0;

    [Header("Rope Progression:")]
    public AnimationCurve ropeProgressionCurve;
    [SerializeField] [Range(1, 50)] private float ropeProgressionSpeed = 1;

    float moveTime = 0;

    [HideInInspector] public bool isGrappling = true;

    bool strightLine = true;

    private void OnEnable()
    {
        moveTime = 0; // Move time counts how long we have been grappling for 
        m_lineRenderer.positionCount = percision; // prescision count is how detailed our rope
        waveSize = StartWaveSize; // wave size is simply being declared in the editor
        strightLine = false; // since this is the start of our wave we 

        LinePointsToFirePoint();

        m_lineRenderer.enabled = true;
    }

    private void OnDisable()
    {
        m_lineRenderer.enabled = false;
        isGrappling = false;
    }

    private void LinePointsToFirePoint() //sets our line points to fire points
    {
        for (int i = 0; i < percision; i++)
        {
            m_lineRenderer.SetPosition(i, grapplingGun.firePoint.position);
        }
    }

    private void Update()
    {
        moveTime += Time.deltaTime;
        DrawRope();
    }

    void DrawRope()
    {
        if (!strightLine)
        {
            // checks if the last point of lr reach grapple point
            if (m_lineRenderer.GetPosition(percision - 1).x == grapplingGun.grapplePoint.x)
            {
                strightLine = true;// makes line straight
            }
            else
            {
                DrawRopeWaves();// draws waves for wavy line
            }
        }
        else
        {
            if (!isGrappling)
            {
                grapplingGun.Grapple();
                isGrappling = true;
            }
            if (waveSize > 0)
            {
                waveSize -= Time.deltaTime * straightenLineSpeed;// gradually decreases the size of the wave
                DrawRopeWaves();
            }
            else
            {
                waveSize = 0;

                if (m_lineRenderer.positionCount != 2) { m_lineRenderer.positionCount = 2; }// removes unnecesary straight lines

                DrawRopeNoWaves();
            }
        }
    }

    void DrawRopeWaves()
    {
        for (int i = 0; i < percision; i++) // iterates through every point made
        {
            float delta = (float)i / ((float)percision - 1f); // delta determines what points between distance of Fp to Gp it should pass
            Vector2 offset = Vector2.Perpendicular(grapplingGun.grappleDistanceVector).normalized * ropeAnimationCurve.Evaluate(delta) * waveSize;
            Vector2 targetPosition = Vector2.Lerp(grapplingGun.firePoint.position, grapplingGun.grapplePoint, delta) + offset; // end position
            Vector2 currentPosition = Vector2.Lerp(grapplingGun.firePoint.position, targetPosition, ropeProgressionCurve.Evaluate(moveTime) * ropeProgressionSpeed);// moves fire point to target point.
            // this finds a perpendicular point and makes it a new point for our line render to pass through making a wave
            m_lineRenderer.SetPosition(i, currentPosition);// sets the position of right point to right spot
        }
    }

    void DrawRopeNoWaves()
    {
        m_lineRenderer.SetPosition(0, grapplingGun.firePoint.position);//sets line render to grapple point
        m_lineRenderer.SetPosition(1, grapplingGun.grapplePoint);//sets other side of line render to fire point
    }
}