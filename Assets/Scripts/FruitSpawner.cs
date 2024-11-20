using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FruitSpawner : MonoBehaviour
{
    public GameObject[] fruits; 
    public int fruitsToSpawnAtOnce = 4; 
    private int totalFruitsSpawned = 0; 
    private float xMin = -45f;
    private float xMax = 45f;
    private float yMin = -40f;
    private float yMax = 40f;

    void Start()
    {
        StartCoroutine(SpawnFruitsContinuously());
    }
    IEnumerator SpawnFruitsContinuously()
    {
        while (totalFruitsSpawned < 2000) 
        {
            for (int i = 0; i < fruitsToSpawnAtOnce; i++)
            {
                GameObject fruit = fruits[Random.Range(0, fruits.Length)];
                float randomX = Random.Range(xMin, xMax);
                float randomY = Random.Range(yMin, yMax);
                Vector2 spawnPosition = new Vector2(randomX, randomY);
                Instantiate(fruit, spawnPosition, Quaternion.identity);

                totalFruitsSpawned++;
                if (totalFruitsSpawned >= 2000)
                {
                    yield break;
                }
            }
            yield return new WaitForSeconds(0.5f);
        }
    }
}
