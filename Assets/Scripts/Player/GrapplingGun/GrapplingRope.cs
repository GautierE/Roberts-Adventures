using UnityEngine;

public class GrapplingRope : MonoBehaviour
{
    [Header("General Refernces:")]
    public GrapplingGun grapplingGun;
    public LineRenderer m_lineRenderer;

    [Header("General Settings:")]
    [SerializeField] private int precision = 40;
    [Range(0, 20)] [SerializeField] private float straightenLineSpeed = 5;

    [Header("Rope Animation Settings:")]
    public AnimationCurve ropeAnimationCurve;
    [Range(0.01f, 4)] [SerializeField] private float StartWaveSize = 2;
    float waveSize = 0;

    [Header("Rope Progression:")]
    public AnimationCurve ropeProgressionCurve;
    [SerializeField] [Range(1, 50)] private float ropeProgressionSpeed = 1;

    [Header("Impact particules:")]
    public GameObject impactParticles;


    float moveTime = 0;

    [HideInInspector] public bool isGrappling = false;

    bool straightLine = true;

    /// <summary>
    /// Se lance une fois au début du script
    /// </summary>
    private void Start()
    {
        OnDisable();
    }

    /// <summary>
    /// Permet d'enable la corde
    /// </summary>
    private void OnEnable()
    {
        //On remet toutes las variables dans les valeurs par défaut
        moveTime = 0;
        m_lineRenderer.positionCount = precision;
        waveSize = StartWaveSize;
        straightLine = false;

        LinePointsToFirePoint();

        m_lineRenderer.enabled = true;
    }

    /// <summary>
    /// Permet de disable la corde
    /// </summary>
    public void OnDisable()
    {
        m_lineRenderer.enabled = false;
        isGrappling = false;
    }
    
    /// <summary>
    /// Set les points de la corde jusquà la cible
    /// </summary>
    private void LinePointsToFirePoint()
    {
        for (int i = 0; i < precision; i++)
            m_lineRenderer.SetPosition(i, grapplingGun.firePoint.position);
  
    }

    /// <summary>
    /// Est appelée à toutes les frames
    /// </summary>
    private void Update()
    {
        moveTime += Time.deltaTime;
        DrawRope();
    }

    /// <summary>
    /// Joue le son de la corde et crée un impact sur la cible
    /// </summary>
    private void playRopeSound(){
        this.gameObject.GetComponent<AudioSource>().Play();
    
        impactParticles.GetComponent<Transform>().position = grapplingGun.grapplePoint;
        impactParticles.GetComponent<ParticleSystem>().Play();
        
    }

    /// <summary>
    /// Dessine la corde
    /// </summary>
    void DrawRope()
    {   
        //Si la corde n'est pas droite
        if (!straightLine)
        {
            //Si la position en X de la corde est le même que la position en X du point à accrocher
            if (m_lineRenderer.GetPosition(precision - 1).x == grapplingGun.grapplePoint.x)
                straightLine = true;
            else{
                //Joue les sons
                playRopeSound();
                DrawRopeWaves();
            }
                
        }
        else
        {
            //Si il n'est pas en train de grapple
            if (!isGrappling)
            {
                //On appelle la méthode grapplingGun.Grapple();
                grapplingGun.Grapple();
                isGrappling = true;
            }

            //Permet de set la wave size en focntion du temps
            waveSize = (waveSize > 0) ? waveSize -= Time.deltaTime * straightenLineSpeed : 0;
            m_lineRenderer.positionCount = precision ;
            DrawRopeWaves();
        }
    }

    /// <summary>
    /// Dessine le "Bruit" sur la corde
    /// <remark>
    /// Il faut que la line Renderer soit visible
    /// </remark>
    /// </summary>
    void DrawRopeWaves()
    {
        //On crée 3 vector de dimension 2.
        Vector2 offset;
        Vector2 targetPosition;
        Vector2 currentPosition;

        for (int i = 0; i < m_lineRenderer.positionCount; i++)
        {
            //On calcule le delta 
            float delta = (float)i / ((float)precision - 1f);
            //On prends le vecteur perpendiculaire et normalisé 
            offset = Vector2.Perpendicular(grapplingGun.grappleDistanceVector).normalized * ropeAnimationCurve.Evaluate(delta) * waveSize;
            //Assigne le lerp pour set la position de la cible
            targetPosition = Vector2.Lerp(grapplingGun.firePoint.position, grapplingGun.grapplePoint, delta) + offset;
            //Assigne le lerp pour la position courante dans la corde
            currentPosition = Vector2.Lerp(grapplingGun.firePoint.position, targetPosition, ropeProgressionCurve.Evaluate(moveTime) * ropeProgressionSpeed);

            //Set la position du point sur la corde en focntion de nos calculs précédents
            m_lineRenderer.SetPosition(i, currentPosition);
        }

       
    }

    /// <summary>
    /// Dessine une corde sans bruit
    /// <remark>
    /// Il faut que la line Renderer soit visible
    /// </remark>
    /// </summary>
    void DrawRopeNoWaves()
    {
        m_lineRenderer.SetPosition(0, grapplingGun.firePoint.position);
        m_lineRenderer.SetPosition(1, grapplingGun.grapplePoint);
    }
}
