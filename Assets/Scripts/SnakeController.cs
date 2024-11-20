using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class SnakeController : MonoBehaviour
{
    public GameObject body;
    public Transform snakeHead;
    public Transform snakeTail;
    public List<Transform> snakeSegments = new List<Transform>();
    public float moveSpeed = 5f;
    public float segmentSpacing;
    public GameObject tail_3, tail_4;
    private Vector2 direction = Vector2.zero;
    public int snake_tail = 0;
    private SnakAiController snakeAi;
    public GameObject foodPrefab; 
    public Joystick joystick;
    private Vector2 lastDirection;
    private bool isSpeedBoosted = false;
    private float space_of_segments;
    public Transform orignal_pos;
    private int foodCollisionCounter = 0;
    private int foodpicker_speed_up = 0;
    public GameObject Speed_button;
    private LeadBoardManager LBM;
    public AudioClip Eating_sfx;
    public AudioSource Gaama_Studio;
    void Start()
    {
        LBM = FindObjectOfType<LeadBoardManager>();
        Speed_button.SetActive(false);
        Transform BodySegment = body.transform;
        snakeSegments.Add(snakeHead);
        snakeSegments.Add(BodySegment);
        snakeSegments.Add(tail_4.transform);
        snakeAi = FindObjectOfType<SnakAiController>();
        space_of_segments = segmentSpacing;
        LBM.AddPlayerInfo(transform.parent.name, snakeSegments.Count);
    }
    void Update()
    {
        direction = new Vector2(joystick.Horizontal, joystick.Vertical).normalized;
        if (direction != Vector2.zero)
        {
            lastDirection = direction;
            MoveSnake(moveSpeed);
        }
        else
        {
            direction = lastDirection;
            MoveSnake(moveSpeed * 0.8f);
        }
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
            if (hit.collider != null && hit.collider.CompareTag("Speed"))
            {
                Debug.Log("Speed object clicked in 2D!");
            }
        }
        if (isSpeedBoosted)
        {
            MoveSnake(moveSpeed * 1.05f);
            segmentSpacing = 0.5f;
        }
        else
        {
            segmentSpacing = space_of_segments;
        }
    }
    void MoveSnake(float speed)
    {
        Vector3 previousPosition = snakeHead.position;
        Quaternion previousRotation = snakeHead.rotation;
        snakeHead.Translate(direction * speed * Time.deltaTime, Space.World);
        if (direction != Vector2.zero)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            snakeHead.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90));
        }
        for (int i = 1; i < snakeSegments.Count; i++)
        {
            Vector3 tempPosition = snakeSegments[i].position;
            snakeSegments[i].position = Vector3.Lerp(snakeSegments[i].position, previousPosition, segmentSpacing * 25 * Time.deltaTime);
            snakeSegments[i].rotation = Quaternion.Lerp(snakeSegments[i].rotation, previousRotation, segmentSpacing * 25 * Time.deltaTime);
            previousPosition = tempPosition;
        }
    }
    public void GrowSnake()
    {
        if (snake_tail == 0)
        {
            Transform beforeTailSegment = snakeSegments[snakeSegments.Count - 1];
            Vector3 newSegmentPosition = beforeTailSegment.position;
            GameObject newSegment = Instantiate(tail_3, newSegmentPosition, Quaternion.identity);
            newSegment.transform.SetParent(snakeHead.transform.parent);
            snakeSegments.Insert(snakeSegments.Count - 1, newSegment.transform);
            tail_3.SetActive(false);
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
            SpriteRenderer newSegmentRenderer = newSegment.GetComponent<SpriteRenderer>();
            SpriteRenderer previousSegmentRenderer = beforeTailSegment.GetComponent<SpriteRenderer>();
            newSegmentRenderer.sortingOrder = previousSegmentRenderer.sortingOrder - 1;
            int Order_layer = newSegmentRenderer.sortingOrder;
            previousSegmentRenderer.sortingOrder = Order_layer;
            //print(previousSegmentRenderer.sortingOrder);
        }
        LBM.UpdatePlayerScore(transform.parent.name, snakeSegments.Count);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    { 
        if (collision.CompareTag("Food"))
        {
            Gaama_Studio.PlayOneShot(Eating_sfx);
            foodCollisionCounter++;
            foodpicker_speed_up++;
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
        if (collision.gameObject.CompareTag("EnemyBody"))
        {
            //print("collision");
            if (snakeAi != null && snakeAi.snakeSegments.Count > snakeSegments.Count)
            {
                //print("You are Dying");
                LBM.UpdateLeaderboardDisplay();
                DestroyPlayer();
            }
        }
        if (foodpicker_speed_up == 30)
        {
            foodpicker_speed_up = 0;
            StartCoroutine(EnableSpecialObjectFor5Seconds());
        }
    }
    public void DestroyPlayer()
    {
        for (int i = snakeSegments.Count - 1; i >= 0; i--)
        {
            if (i == 0 || i == 1 || i == snakeSegments.Count - 1)
            {
                continue;
            }
            Destroy(snakeSegments[i].gameObject);
            snakeSegments.RemoveAt(i);
        }
        tail_3.SetActive(true);
        snakeTail.transform.gameObject.SetActive(true);
        snake_tail = 0;
        transform.position = orignal_pos.position;
        PlayerManager.Instance.PlayerDeath(1);
    }
    public void OnSpeedUpButtonPressed()
    {
        isSpeedBoosted = true;
    }
    public void OnSpeedUpButtonReleased()
    {
        isSpeedBoosted = false; 
    }
    private IEnumerator EnableSpecialObjectFor5Seconds()
    {
        Speed_button.SetActive(true);
        yield return new WaitForSeconds(10);
        isSpeedBoosted = false;
        Speed_button.SetActive(false); 
    }
}
