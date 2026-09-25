using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public static int SCORE = 0;

    [Header("Movement Settings")]
    public float thrustForce = 6f;
    public float rotationSpeed = 150f;
    public float drag = 1f;

    [Header("Shooting Settings")]
    public GameObject bulletPrefab;
    public Transform gunTransform;
    public float fireRate = 0.25f;
    private float nextFire = 0f;

    [Header("Screen Boundaries (Toroidal Wrapping)")]
    public float xBorderLimit = 9.0f;
    public float yBorderLimit = 5.2f;

    private Rigidbody _rigidbody;
    private Vector2 thrustDirection;

    void Start()
    {
        // Rigidbody nos permite aplicar fuerzas físicas a la nave
        _rigidbody = GetComponent<Rigidbody>();
        if (_rigidbody != null)
        {
            _rigidbody.linearDamping = drag; // Unity 6 / linearDamping (o drag)
            _rigidbody.useGravity = false;
            _rigidbody.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;
        }

        if (gunTransform == null)
        {
            // Si no se asigna en inspector, buscar objeto hijo llamado "Gun"
            Transform gunChild = transform.Find("Gun");
            if (gunChild != null)
                gunTransform = gunChild;
            else
                gunTransform = transform;
        }
    }

    private void Update()
    {
        // Control de disparo (Barra espaciadora o botón Fire1)
        if ((Input.GetButtonDown("Fire1") || Input.GetKeyDown(KeyCode.Space)) && Time.time >= nextFire)
        {
            nextFire = Time.time + fireRate;
            Shoot();
        }

        // Envolver la nave alrededor de los bordes de pantalla (Screen Wrapping)
        WrapScreen();
    }

    private void FixedUpdate()
    {
        // Obtenemos las pulsaciones de teclado con fallback a Horizontal/Vertical
        float rawRotation = 0f;
        float rawThrust = 0f;

        try
        {
            rawRotation = Input.GetAxis("Rotate");
            rawThrust = Input.GetAxis("Thrust");
        }
        catch
        {
            rawRotation = Input.GetAxis("Horizontal");
            rawThrust = Input.GetAxis("Vertical");
        }

        if (Mathf.Abs(rawRotation) < 0.01f)
            rawRotation = Input.GetAxis("Horizontal");

        if (Mathf.Abs(rawThrust) < 0.01f && Input.GetAxis("Vertical") > 0.01f)
            rawThrust = Input.GetAxis("Vertical");

        // Sólo empujar hacia adelante en la mecánica tradicional de Asteroids
        if (rawThrust < 0) rawThrust = 0;

        float rotation = rawRotation * rotationSpeed * Time.fixedDeltaTime;
        float thrust = rawThrust * thrustForce;

        // La dirección de empuje por defecto es .right (el eje X positivo de la nave)
        thrustDirection = transform.right;

        // Rotamos con el eje "Rotate" negativo para que la dirección sea correcta
        transform.Rotate(Vector3.forward, -rotation);

        // Añadimos la fuerza capturada a la nave del jugador
        if (_rigidbody != null)
        {
            _rigidbody.AddForce(thrust * (Vector3)thrustDirection);
        }
        else
        {
            transform.Translate(Vector3.right * thrust * Time.fixedDeltaTime, Space.Self);
        }
    }

    private void Shoot()
    {
        Vector3 spawnPos = gunTransform != null ? gunTransform.position : transform.position;
        Quaternion spawnRot = transform.rotation;

        // Intentar usar Object Pooling si está activo (Iteración 2)
        GameObject bullet = null;
        if (ObjectPool.Instance != null)
        {
            bullet = ObjectPool.Instance.GetBullet(spawnPos, spawnRot);
        }

        // Si no hay ObjectPool disponible, instanciar normalmente (Iteración 1)
        if (bullet == null && bulletPrefab != null)
        {
            bullet = Instantiate(bulletPrefab, spawnPos, spawnRot);
        }

        if (bullet != null)
        {
            Bullet bulletScript = bullet.GetComponent<Bullet>();
            if (bulletScript != null)
            {
                bulletScript.targetVector = transform.right;
            }
        }
    }

    private void WrapScreen()
    {
        Vector3 pos = transform.position;

        if (pos.x > xBorderLimit)
            pos.x = -xBorderLimit;
        else if (pos.x < -xBorderLimit)
            pos.x = xBorderLimit;

        if (pos.y > yBorderLimit)
            pos.y = -yBorderLimit;
        else if (pos.y < -yBorderLimit)
            pos.y = yBorderLimit;

        transform.position = pos;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            OnPlayerDestroyed();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            OnPlayerDestroyed();
        }
    }

    public void OnPlayerDestroyed()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.GameOver();
        }
        else
        {
            // Reiniciar la escena directamente como indica la diapositiva 11
            SCORE = 0;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
