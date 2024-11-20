using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class SnakeControllerAI : MonoBehaviour
{
    public GameObject body;
    public Transform snakeHead;
    public Transform snakeTail;
    public List<Transform> snakeSegments = new List<Transform>();
    public GameObject tail_4, tail_3;
    public bool Is_Eating, Is_dodge;
    public Transform currentFood;
    private Vector2 direction = Vector2.up;
    public float rotationSpeed = 5f;
    public float moveSpeed = 5f;
    public float segmentSpacing = 0.5f;
    public int snake_tail = 0;
    public GameObject food;
    private SnakeController snake_player;
    private Vector3 originalPosition;
    private int foodCollisionCounter = 0;

    void Start()
    {
        originalPosition = transform.position;
        Transform BodySegment = body.transform;
        snakeSegments.Add(snakeHead);
        snakeSegments.Add(BodySegment);
        snakeSegments.Add(tail_3.transform);
        snake_player = FindObjectOfType<SnakeController>();
        FindClosestFood();
    }
    void Update()
    {
        MoveTowardsFood();
        MoveSnake();
    }

    void MoveTowardsFood()
    {
        if (currentFood == null)
        {
            FindClosestFood();
            return;
        }
        Vector2 foodDirection = (currentFood.position - snakeHead.position).normalized;
        if (Mathf.Abs(foodDirection.x) > Mathf.Abs(foodDirection.y))
        {
            direction = foodDirection.x > 0 ? Vector2.right : Vector2.left;
        }
        else
        {
            direction = foodDirection.y > 0 ? Vector2.up : Vector2.down;
        }
        Quaternion rotation = Quaternion.LookRotation(Vector3.forward, currentFood.position - snakeHead.position);
        snakeHead.rotation = Quaternion.Lerp(snakeHead.rotation, rotation, Time.deltaTime * rotationSpeed);
    }
    private void FindClosestFood()
    {
        GameObject[] foods = GameObject.FindGameObjectsWithTag("Food");
        float closestDistance = Mathf.Infinity;
        foreach (GameObject food in foods)
        {
            float distance = Vector2.Distance(snakeHead.position, food.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                currentFood = food.transform;
            }
        }
    }
    void MoveSnake()
    {
        Vector3 previousPosition = snakeHead.position;
        Quaternion previousRotation = snakeHead.rotation;
        snakeHead.Translate(direction * moveSpeed * Time.deltaTime);
        for (int i = 1; i < snakeSegments.Count; i++)
        {
            Vector3 tempPosition = snakeSegments[i].position;
            Quaternion tempRotation = snakeSegments[i].rotation;
            snakeSegments[i].position = Vector3.Lerp(snakeSegments[i].position, previousPosition, segmentSpacing * 25f * Time.deltaTime);
            snakeSegments[i].rotation = Quaternion.Lerp(snakeSegments[i].rotation, previousRotation, segmentSpacing * 25f * Time.deltaTime);
            previousPosition = tempPosition;
            previousRotation = tempRotation;
        }
    }
    public void GrowSnake()
    {
        if (snake_tail == 0)
        {
            Transform beforeTailSegment = snakeSegments[snakeSegments.Count - 1];
            Vector3 newSegmentPosition = beforeTailSegment.position;
            GameObject newSegment = Instantiate(tail_4, newSegmentPosition, Quaternion.identity);
            newSegment.transform.SetParent(snakeHead.transform.parent);
            snakeSegments.Insert(snakeSegments.Count - 1, newSegment.transform);
            tail_4.SetActive(false);
        }
        if (snake_tail == 1)
        {
            Transform beforeTailSegment = snakeSegments[snakeSegments.Count - 2];
            Vector3 newSegmentPosition = beforeTailSegment.position;
            GameObject newSegment = Instantiate(snakeTail.transform.gameObject, newSegmentPosition, Quaternion.identity);
            newSegment.transform.SetParent(snakeHead.transform.parent);
            snakeSegments.Insert(snakeSegments.Count - 2, newSegment.transform);
            snakeTail.transform.gameObject.SetActive(false);
        }
        if (snake_tail == 2)
        {
            Transform beforeTailSegment = snakeSegments[snakeSegments.Count - 3];
            Vector3 newSegmentPosition = beforeTailSegment.position;
            GameObject newSegment = Instantiate(body, newSegmentPosition, Quaternion.identity);
            newSegment.transform.SetParent(snakeHead.transform.parent);
            snakeSegments.Insert(snakeSegments.Count - 3, newSegment.transform);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Food"))
        {
            if (collision.CompareTag("Food"))
            {
                foodCollisionCounter++; 
                if (foodCollisionCounter == 3)
                {
                    GrowSnake();
                    snake_tail++;
                    foodCollisionCounter = 0;
                }
                Destroy(collision.gameObject);
                if (snake_tail >= 2)
                {
                    snake_tail = 2; 
                }
            }
        }
        if (collision.gameObject.CompareTag("PlayerBody"))
        {
            if (snake_player != null && snake_player.snakeSegments.Count > snakeSegments.Count)
            {
                Kill_Snake();
            }
        }
    }
    public void DestroySnake()
    {
        if (snakeSegments.Count < 40)
        {
            transform.position = originalPosition;
        }
        else
        {
            for (int i = snakeSegments.Count - 1; i >= 0; i--)
            {
                if (i == 0 || i == 1 || i == snakeSegments.Count - 1)
                {
                    continue;

                }
                //Instantiate(food, snakeSegments[i].position, Quaternion.identity);
                Destroy(snakeSegments[i].gameObject);
                snakeSegments.RemoveAt(i);
            }
            tail_4.SetActive(true);
            snakeTail.transform.gameObject.SetActive(true);
            snake_tail = 0;
            transform.position = originalPosition;
        }
    }

    private void Kill_Snake()
    {
        foreach (Transform segment in snakeSegments)
        {
            Instantiate(food, segment.position, Quaternion.identity);
            Destroy(segment.gameObject);
        }
        snakeSegments.Clear();
        Destroy(gameObject);
        if (PlayerManager.Instance != null)
        {
            PlayerManager.Instance.RemovePlayer(gameObject);
        }
    }
}
