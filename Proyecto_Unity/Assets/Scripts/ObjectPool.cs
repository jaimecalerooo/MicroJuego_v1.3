using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Patrón de diseño Object Pooling (Iteración 2 - Slides 15 y 16)
/// Evita llamadas recurrentes a Instantiate y Destroy durante el bucle de juego,
/// optimizando el uso de memoria y eliminando pausas por Garbage Collection.
/// </summary>
public class ObjectPool : MonoBehaviour
{
    public static ObjectPool Instance { get; private set; }

    [Header("Prefabs")]
    public GameObject bulletPrefab;
    public GameObject asteroidPrefab;
    public GameObject miniAsteroidPrefab;

    [Header("Pool Initial Sizes")]
    public int bulletPoolSize = 25;
    public int asteroidPoolSize = 15;
    public int miniAsteroidPoolSize = 30;

    private readonly Queue<GameObject> bulletPool = new Queue<GameObject>();
    private readonly Queue<GameObject> asteroidPool = new Queue<GameObject>();
    private readonly Queue<GameObject> miniAsteroidPool = new Queue<GameObject>();

    private Transform poolContainer;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Creamos un contenedor fuera de la vista de la cámara para almacenar los objetos inactivos
        GameObject containerGo = new GameObject("Pool_Container");
        containerGo.transform.SetParent(transform);
        containerGo.transform.position = new Vector3(-1000f, -1000f, 0f);
        poolContainer = containerGo.transform;

        InitializePools();
    }

    private void InitializePools()
    {
        if (bulletPrefab != null)
        {
            for (int i = 0; i < bulletPoolSize; i++)
            {
                GameObject obj = CreateNewObject(bulletPrefab, poolContainer);
                bulletPool.Enqueue(obj);
            }
        }

        if (asteroidPrefab != null)
        {
            for (int i = 0; i < asteroidPoolSize; i++)
            {
                GameObject obj = CreateNewObject(asteroidPrefab, poolContainer);
                asteroidPool.Enqueue(obj);
            }
        }

        if (miniAsteroidPrefab != null)
        {
            for (int i = 0; i < miniAsteroidPoolSize; i++)
            {
                GameObject obj = CreateNewObject(miniAsteroidPrefab, poolContainer);
                miniAsteroidPool.Enqueue(obj);
            }
        }
    }

    private GameObject CreateNewObject(GameObject prefab, Transform parent)
    {
        GameObject obj = Instantiate(prefab, parent);
        obj.SetActive(false);
        return obj;
    }

    // ==========================================
    // Métodos para Bullets
    // ==========================================
    public GameObject GetBullet(Vector3 position, Quaternion rotation)
    {
        GameObject obj = GetFromQueue(bulletPool, bulletPrefab);
        if (obj != null)
        {
            obj.transform.position = position;
            obj.transform.rotation = rotation;
            obj.transform.SetParent(null);
            obj.SetActive(true);
        }
        return obj;
    }

    public void ReturnBullet(GameObject bullet)
    {
        bullet.SetActive(false);
        bullet.transform.SetParent(poolContainer);
        bulletPool.Enqueue(bullet);
    }

    // ==========================================
    // Métodos para Asteroids Grandes
    // ==========================================
    public GameObject GetAsteroid(Vector3 position, Quaternion rotation)
    {
        GameObject obj = GetFromQueue(asteroidPool, asteroidPrefab);
        if (obj != null)
        {
            obj.transform.position = position;
            obj.transform.rotation = rotation;
            obj.transform.SetParent(null);
            obj.SetActive(true);
        }
        return obj;
    }

    public void ReturnAsteroid(GameObject asteroid)
    {
        asteroid.SetActive(false);
        asteroid.transform.SetParent(poolContainer);
        asteroidPool.Enqueue(asteroid);
    }

    // ==========================================
    // Métodos para Mini-Asteroids (Fragmentos)
    // ==========================================
    public GameObject GetMiniAsteroid(Vector3 position, Quaternion rotation)
    {
        GameObject obj = GetFromQueue(miniAsteroidPool, miniAsteroidPrefab);
        if (obj != null)
        {
            obj.transform.position = position;
            obj.transform.rotation = rotation;
            obj.transform.SetParent(null);
            obj.SetActive(true);
        }
        return obj;
    }

    public void ReturnMiniAsteroid(GameObject miniAsteroid)
    {
        miniAsteroid.SetActive(false);
        miniAsteroid.transform.SetParent(poolContainer);
        miniAsteroidPool.Enqueue(miniAsteroid);
    }

    private GameObject GetFromQueue(Queue<GameObject> pool, GameObject prefab)
    {
        if (pool.Count > 0)
        {
            return pool.Dequeue();
        }

        // Si la piscina se agota bajo demanda, instanciamos uno nuevo para no bloquear el juego
        if (prefab != null)
        {
            return CreateNewObject(prefab, poolContainer);
        }

        return null;
    }
}
