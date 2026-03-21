using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] private float m_speed = 4.0f;

    private Animator m_animator;
    private Rigidbody2D m_body2d;
    private float m_delayToIdle = 0.0f;

    void Start()
    {
        m_animator = GetComponent<Animator>();
        m_body2d = GetComponent<Rigidbody2D>();

        m_body2d.gravityScale = 0f;
        m_animator.SetBool("Grounded", true);

        // --- NOWOŒÆ: Powrót na zapisan¹ pozycjê przed aren¹! ---
        if (GameManager.Instance != null && GameManager.Instance.lastMapPosition != Vector3.zero)
        {
            // Teleportujemy gracza pod drzwi
            transform.position = GameManager.Instance.lastMapPosition;

            // Resetujemy koordynaty, ¿eby przy przechodzeniu miêdzy innymi scenami nie teleportowa³o nas losowo
            GameManager.Instance.lastMapPosition = Vector3.zero;
        }
    }

    void Update()
    {
        float inputX = Input.GetAxis("Horizontal");
        float inputY = Input.GetAxis("Vertical");

        if (inputX > 0)
            GetComponent<SpriteRenderer>().flipX = false;
        else if (inputX < 0)
            GetComponent<SpriteRenderer>().flipX = true;

        m_body2d.linearVelocity = new Vector2(inputX * m_speed, inputY * m_speed);

        if (Mathf.Abs(inputX) > Mathf.Epsilon || Mathf.Abs(inputY) > Mathf.Epsilon)
        {
            m_delayToIdle = 0.05f;
            m_animator.SetInteger("AnimState", 1);
        }
        else
        {
            m_delayToIdle -= Time.deltaTime;
            if (m_delayToIdle < 0)
                m_animator.SetInteger("AnimState", 0);
        }
    }
}