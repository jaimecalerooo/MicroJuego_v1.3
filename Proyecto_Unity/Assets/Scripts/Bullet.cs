using UnityEngine;
using UnityEngine.UI;

public class Bullet : MonoBehaviour
{
    public float speed = 12f;
    public float maxLifeTime = 3f;
    public Vector3 targetVector = Vector3.right;

    private float lifeTimer = 0f;

    void Awake()
    {
        // Fuerza la inyección de un Rigidbody cinemático si Unity no detecta físicas,
        // lo que soluciona que OnTriggerEnter falle silenciosamente.
        if (GetComponent<Collider>() != null && GetComponent<Rigidbody>() == null)
        {
            Rigidbody rb = gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.useGravity = false;
        }
        
        if (GetComponent<Collider2D>() != null && GetComponent<Rigidbody2D>() == null)
        {
            Rigidbody2D rb2d = gameObject.AddComponent<Rigidbody2D>();
            rb2d.isKinematic = true;
        }
    }

    void OnEnable()
    {
        lifeTimer = maxLifeTime;
    }

    void Start()
    {
        if (ObjectPool.Instance == null)
        {
            Destroy(gameObject, maxLifeTime);
        }
    }

    void Update()
    {
        transform.Translate(targetVector.normalized * speed * Time.deltaTime, Space.World);

        if (ObjectPool.Instance != null)
        {
            lifeTimer -= Time.deltaTime;
            if (lifeTimer <= 0f)
            {
                DeactivateBullet();
            }
        }
    }

    // Cobertura total para evitar fallos por mala configuración en el Inspector
    private void OnTriggerEnter(Collider other) { CheckHit(other.gameObject); }
    private void OnTriggerEnter2D(Collider2D other) { CheckHit(other.gameObject); }
    private void OnCollisionEnter(Collision collision) { CheckHit(collision.gameObject); }
    private void OnCollisionEnter2D(Collision2D collision) { CheckHit(collision.gameObject); }

    private void CheckHit(GameObject hitObject)
    {
        Asteroid asteroid = hitObject.GetComponent<Asteroid>();
        if (asteroid != null)
        {
            asteroid.TakeHit(targetVector);
            IncreaseScore();
            DeactivateBullet();
        }
        else if (hitObject.CompareTag("Enemy"))
        {
            Destroy(hitObject);
            IncreaseScore();
            DeactivateBullet();
        }
    }

    public void IncreaseScore()
    {
        Player.SCORE += 10;
        UpdateScoreText();

        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateScore(Player.SCORE);
        }
    }

    private void UpdateScoreText()
    {
        GameObject go = GameObject.FindGameObjectWithTag("UI");
        if (go != null)
        {
            Text textComp = go.GetComponent<Text>();
            if (textComp != null)
            {
                textComp.text = "Score: " + Player.SCORE;
            }
        }
    }

    private void DeactivateBullet()
    {
        if (ObjectPool.Instance != null)
        {
            ObjectPool.Instance.ReturnBullet(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}