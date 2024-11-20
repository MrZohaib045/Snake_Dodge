using System.Collections.Generic;
using UnityEngine;

public class SnakAiController : MonoBehaviour
{
    public Transform snakeHead, body, Tail;
    public GameObject Body, Tail1, Tail2;
    public List<Transform> snakeSegments = new List<Transform>();
    public Vector2 areaSize = new Vector2(10, 10);
    public float moveSpeed = 2f;
    public float rotationSpeed = 100f;
    public float maxTargetRadius = 10f;
    public float searchRadius;
    public float Detection_Radius;
    public float segmentSpacing = 0.5f;
    public Vector2 targetPosition;
    public List<Transform> foodTargets = new List<Transform>();
    public int snake_tail = 0;
    private int foodCollisionCounter = 0;
    private float targetTimer = 0f; 
    private float targetChangeTime = 5f;
    private SnakeController snake_player;
    public GameObject food;
    public string Snake_Name;
    private LeadBoardManager LBM;
    public ParticleSystem destroyEffect;

    void Start()
    {
        LBM = FindObjectOfType<LeadBoardManager>();
        Detection_Radius = 3f;
        searchRadius = Random.Range(12, 18);
        snakeSegments.Add(snakeHead);
        snakeSegments.Add(body);
        snakeSegments.Add(Tail);
        SetRandomTargetPosition();
        snake_player = FindObjectOfType<SnakeController>();
        Snake_Name = transform.parent.name;
        LBM.AddPlayerInfo(Snake_Name, snakeSegments.Count);
    }

    void Update()
    {
        MoveSnake();
        CheckBoundary();
        targetTimer += Time.deltaTime;
        if (targetTimer >= targetChangeTime)
        {
            SetRandomTargetPosition();
            targetTimer = 0f;
        }
    }
    void MoveSnake()
    {
        Vector3 targetDir = (targetPosition - (Vector2)snakeHead.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward, targetDir);
        snakeHead.rotation = Quaternion.RotateTowards(snakeHead.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        snakeHead.Translate(Vector3.up * moveSpeed * Time.deltaTime);
        if (Vector2.Distance(snakeHead.position, targetPosition) < 0.1f)
        {
            UpdateFoodTargets();
            SetRandomTargetPosition();
            targetTimer = 0f; 
        }
        Vector3 previousPosition = snakeHead.position;
        Quaternion previousRotation = snakeHead.rotation;
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
    void SetRandomTargetPosition()
    {
        UpdateFoodTargets();

        if (foodTargets.Count > 0)
        {
            int randomIndex = Random.Range(0, foodTargets.Count);
            targetPosition = foodTargets[randomIndex].position;
        }
        else
        {
            float randomAngle = Random.Range(0f, 2 * Mathf.PI);
            float randomDistance = Random.Range(0f, maxTargetRadius);

            float offsetX = Mathf.Cos(randomAngle) * randomDistance;
            float offsetY = Mathf.Sin(randomAngle) * randomDistance;

            targetPosition = new Vector2(
                Mathf.Clamp(snakeHead.position.x + offsetX, -areaSize.x / 2, areaSize.x / 2),
                Mathf.Clamp(snakeHead.position.y + offsetY, -areaSize.y / 2, areaSize.y / 2));
        }
    }
    void UpdateFoodTargets()
    {
        foodTargets.Clear();
        Collider2D[] colliders = Physics2D.OverlapCircleAll(snakeHead.position, searchRadius);
        foreach (var collider in colliders)
        {
            if (collider.CompareTag("Food"))
            {
                foodTargets.Add(collider.transform);
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Food"))
        {
            foodCollisionCounter++;
            if (foodCollisionCounter == 2)
            {
                GrowSnake();
                snake_tail++;
                foodCollisionCounter = 0;
            }
            Destroy(collision.gameObject);
            UpdateFoodTargets();
            if (snake_tail >= 2)
            {
                snake_tail = 2;
            }
        }
        if (collision.gameObject.CompareTag("PlayerBody"))
        {
            if (snake_player != null && snake_player.snakeSegments.Count > snakeSegments.Count)
            {
                Kill_Snake();
                LBM.RemovePlayerInfo(Snake_Name);
                LBM.UpdateLeaderboardDisplay();
            }
        }
        if (collision.gameObject.CompareTag("EnemyBody"))
        {
            SnakAiController otherSnake = collision.transform.parent.GetChild(0).GetComponent<SnakAiController>();
            if (otherSnake != null && otherSnake != this)
            {
                if (otherSnake.snakeSegments.Count > this.snakeSegments.Count)
                {
                    print("Other snake is larger. Killing this snake.");
                    Kill_Snake();
                    LBM.RemovePlayerInfo(Snake_Name);
                    LBM.UpdateLeaderboardDisplay();
                }
            }
        }
    }
    public void GrowSnake()
    {
        if (snake_tail == 0)
        {
            Transform beforeTailSegment = snakeSegments[snakeSegments.Count - 1];
            Vector3 newSegmentPosition = beforeTailSegment.position;
            GameObject newSegment = Instantiate(Tail1, newSegmentPosition, Quaternion.identity);
            newSegment.transform.SetParent(snakeHead.transform.parent);
            snakeSegments.Insert(snakeSegments.Count - 1, newSegment.transform);
            Tail1.SetActive(false);
        }
        if (snake_tail == 1)
        {
            Transform beforeTailSegment = snakeSegments[snakeSegments.Count - 2];
            Vector3 newSegmentPosition = beforeTailSegment.position;
            GameObject newSegment = Instantiate(Tail2.transform.gameObject, newSegmentPosition, Quaternion.identity);
            newSegment.transform.SetParent(snakeHead.transform.parent);
            snakeSegments.Insert(snakeSegments.Count - 2, newSegment.transform);
            Tail2.transform.gameObject.SetActive(false);
        }
        if (snake_tail == 2)
        {
            Transform beforeTailSegment = snakeSegments[snakeSegments.Count - 3];
            Vector3 newSegmentPosition = beforeTailSegment.position;
            GameObject newSegment = Instantiate(Body, newSegmentPosition, Quaternion.identity);
            newSegment.transform.SetParent(snakeHead.transform.parent);
            snakeSegments.Insert(snakeSegments.Count - 3, newSegment.transform);
        }

        LBM.UpdatePlayerScore(Snake_Name, snakeSegments.Count);
    }
    void CheckBoundary()
    {
        Vector2 pos = snakeHead.position;
        pos.x = Mathf.Clamp(pos.x, -areaSize.x / 2, areaSize.x / 2);
        pos.y = Mathf.Clamp(pos.y, -areaSize.y / 2, areaSize.y / 2);
        snakeHead.position = pos;
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(areaSize.x, areaSize.y, 0));
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(snakeHead.position, searchRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(snakeHead.position, Detection_Radius);
    }
    private void Kill_Snake()
    {
        foreach (Transform segment in snakeSegments)
        {
            if (destroyEffect != null)
            {
                ParticleSystem effect = Instantiate(destroyEffect, segment.position, Quaternion.identity);
                Destroy(effect.gameObject, effect.main.duration + 0.35f);
            }
            Instantiate(food, segment.position, Quaternion.identity);
            Destroy(segment.gameObject);
        }
        snakeSegments.Clear();
        if (PlayerManager.Instance != null)
        {
            PlayerManager.Instance.RemovePlayer(gameObject);
        }
        Destroy(transform.parent.gameObject);
    }
}
