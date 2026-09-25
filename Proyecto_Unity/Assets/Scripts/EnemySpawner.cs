using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Asteroid Prefab")]
    public GameObject asteroidPrefab;

    [Header("Spawn Rate Settings")]
    public float spawnRatePerMinute = 30f;
    public float spawnRateIncrement = 1f;

    [Header("Spawn Area Limits")]
    public float xBorderLimit = 8f;
    public float yBorderLimit = 6.2f;

    private float spawnNext = 0f;

    void Start()
    {
        spawnNext = Time.time + (60f / spawnRatePerMinute);
    }

    void Update()
    {
        if (Time.time > spawnNext)
        {
            // Indicamos cuándo podremos volver a instanciar otro enemigo
            spawnNext = Time.time + (60f / spawnRatePerMinute);

            // Con cada spawn hay más asteroides por minuto para incrementar la dificultad
            spawnRatePerMinute += spawnRateIncrement;

            // Guardamos un punto aleatorio entre las esquinas superiores de la pantalla
            float rand = Random.Range(-xBorderLimit, xBorderLimit);
            Vector3 spawnPosition = new Vector3(rand, yBorderLimit, 0f);

            // Instanciamos el asteroide con Object Pool (Iteración 2) o Instantiate (Iteración 1)
            SpawnAsteroid(spawnPosition);
        }
    }

    private void SpawnAsteroid(Vector3 position)
    {
        GameObject asteroid = null;

        if (ObjectPool.Instance != null)
        {
            asteroid = ObjectPool.Instance.GetAsteroid(position, Quaternion.identity);
        }

        if (asteroid == null && asteroidPrefab != null)
        {
            asteroid = Instantiate(asteroidPrefab, position, Quaternion.identity);
        }

        if (asteroid != null)
        {
            Asteroid astComp = asteroid.GetComponent<Asteroid>();
            if (astComp != null)
            {
                astComp.isFragment = false;
                astComp.moveDirection = Vector3.down;
            }
        }
    }
}
