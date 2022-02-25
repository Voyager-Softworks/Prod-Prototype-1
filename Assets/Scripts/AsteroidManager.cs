using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidManager : MonoBehaviour
{

    public int asteroidCount = 10;
    public GameObject[] asteroidPrefabs;

    public float minAsteroidDistance = 5f;
    public float maxAsteroidDistance = 10f;

    public float maxAsteroidSize = 1f;

    public float minAsteroidSize = 0.5f;

    public float asteroidFieldRadius = 100000.0f;

    public float asteroidFieldHeight = 100000.0f;

    List<GameObject> asteroids = new List<GameObject>();

    // Generate a random position within the asteroid field
    Vector3 RandomPosition()
    {
        float x = Random.Range(-asteroidFieldRadius, asteroidFieldRadius);
        float z = Random.Range(-asteroidFieldRadius, asteroidFieldRadius);
        float y = Random.Range(-asteroidFieldHeight/2.0f, asteroidFieldHeight/2.0f);
        return new Vector3(x, y, z) + transform.position;
    }

    // Generate a random asteroid size
    Vector3 RandomSize()
    {
        float size = Random.Range(minAsteroidSize, maxAsteroidSize);

        float sizejitter = size * 0.3f;

        float x = Random.Range(size - sizejitter, size + sizejitter);
        float y = Random.Range(size - sizejitter, size + sizejitter);
        float z = Random.Range(size - sizejitter, size + sizejitter);
        return new Vector3(x, y, z);
    }

    // Generate a random asteroid rotation
    Quaternion RandomRotation()
    {
        return Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
    }

    // Spawn an asteroid
    void SpawnAsteroid()
    {
        // Create an asteroid
        GameObject asteroid = Instantiate(asteroidPrefabs[Random.Range(0, asteroidPrefabs.Length)], RandomPosition(), RandomRotation());

        // Set the asteroid's size
        asteroid.transform.localScale = RandomSize();
        asteroid.transform.parent = transform;
        

        // Add the asteroid to the list
        asteroids.Add(asteroid);
    }

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < asteroidCount; i++)
        {
            SpawnAsteroid();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
