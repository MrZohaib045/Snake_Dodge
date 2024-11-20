using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoillsionCheck : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            print("hello");
            collision.GetComponent<SnakeController>().DestroyPlayer();
        }
        if (collision.gameObject.CompareTag("Enemy"))
        {
            print("hello");
            collision.GetComponent<SnakeControllerAI>().DestroySnake();
        }
    }
}
