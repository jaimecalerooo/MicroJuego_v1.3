using UnityEngine;
using UnityEngine.SceneManagement;

public class Asteroid : MonoBehaviour
{
    [Header("Movement Properties")]
    public float speed = 2.5f;
    public float rotationSpeed = 45f;
    public Vector3 moveDirection = Vector3.down;
    public float maxLifeTime = 8f;

    [Header("Fragment Properties (Iteration 2)")]
    public bool isFragment = false;
    public GameObject miniAsteroidPrefab;
    [Tooltip("Ángulo de separación respecto a la bisectriz (vector de la bala) en grados")]
    public float separationAngle = 35f;

    [Header("Screen Boundaries")]
    public float lowerBound = -6.5f;

    private float lifeTimer = 0f;

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
        transform.Translate(moveDirection.normalized * speed * Time.deltaTime, Space.World);
        transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);

        if (transform.position.y < lowerBound)
        {
            DeactivateAsteroid();
            return;
        }

        if (ObjectPool.Instance != null)
        {
            lifeTimer -= Time.deltaTime;
            if (lifeTimer <= 0f)
            {
                DeactivateAsteroid();
            }
        }
    }

    public void TakeHit(Vector3 bulletDirection)
    {
        if (!isFragment)
        {
            SpawnFragments(bulletDirection);
        }

        DeactivateAsteroid();
    }

    private void SpawnFragments(Vector3 bulletDirection)
    {
        if (bulletDirection == Vector3.zero)
        {
            bulletDirection = Vector3.up;
        }

        float bulletAngle = Mathf.Atan2(bulletDirection.y, bulletDirection.x) * Mathf.Rad2Deg;

        float angle1 = bulletAngle + separationAngle;
        float angle2 = bulletAngle - separationAngle;

        Vector3 dir1 = new Vector3(Mathf.Cos(angle1 * Mathf.Deg2Rad), Mathf.Sin(angle1 * Mathf.Deg2Rad), 0f).normalized;
        Vector3 dir2 = new Vector3(Mathf.Cos(angle2 * Mathf.Deg2Rad), Mathf.Sin(angle2 * Mathf.Deg2Rad), 0f).normalized;

        SpawnSingleFragment(transform.position, dir1);
        SpawnSingleFragment(transform.position, dir2);
    }

    private void SpawnSingleFragment(Vector3 position, Vector3 direction)
    {
        GameObject fragment = null;

        if (ObjectPool.Instance != null)
        {
            fragment = ObjectPool.Instance.GetMiniAsteroid(position, Quaternion.identity);
        }

        if (fragment == null)
        {
            if (miniAsteroidPrefab != null)
            {
                fragment = Instantiate(miniAsteroidPrefab, position, Quaternion.identity);
            }
            else
            {
                // Fallback de seguridad: si no hay un prefab en el pool o en el inspector, reduce este objeto a la mitad.
                fragment = Instantiate(gameObject, position, Quaternion.identity);
                fragment.transform.localScale = transform.localScale * 0.5f;
            }
        }

        if (fragment != null)
        {
            Asteroid astComponent = fragment.GetComponent<Asteroid>();
            if (astComponent != null)
            {
                astComponent.isFragment = true;
                astComponent.moveDirection = direction;
                astComponent.speed = speed * 1.3f; 
            }
        }
    }

    private void OnTriggerEnter(Collider other) { CheckPlayerCollision(other.gameObject); }
    private void OnTriggerEnter2D(Collider2D other) { CheckPlayerCollision(other.gameObject); }
    private void OnCollisionEnter(Collision collision) { CheckPlayerCollision(collision.gameObject); }
    private void OnCollisionEnter2D(Collision2D collision) { CheckPlayerCollision(collision.gameObject); }

    private void CheckPlayerCollision(GameObject hitObject)
    {
        if (hitObject.CompareTag("Player"))
        {
            Player player = hitObject.GetComponent<Player>();
            if (player != null)
            {
                player.OnPlayerDestroyed();
            }
            else
            {
                Player.SCORE = 0;
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    }

    private void DeactivateAsteroid()
    {
        if (ObjectPool.Instance != null)
        {
            if (isFragment)
                ObjectPool.Instance.ReturnMiniAsteroid(gameObject);
            else
                ObjectPool.Instance.ReturnAsteroid(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}