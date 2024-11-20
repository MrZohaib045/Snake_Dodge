using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }
    public List<GameObject> players = new List<GameObject>();
    public Text playerCountText;
    public Text DeathCounter;
    public int death_value;
    public GameObject GameOverPanel;
    public GameObject GameWinPanel;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        UpdatePlayerCountText();
        updatePlayerDeath();
    }
    void UpdatePlayerCountText()
    {
        playerCountText.text = players.Count.ToString();

        if(players.Count == 0)
        {
            print("You Wins");
            GameWinPanel.SetActive(true);
            Invoke("loadSceneAgain", 4f);
        }
    }
    public void RemovePlayer(GameObject player)
    {
        if (players.Contains(player))
        {
            players.Remove(player);
            Destroy(player);
            UpdatePlayerCountText();
        }
    }
    void updatePlayerDeath()
    {
        DeathCounter.text = death_value.ToString();
    }
    public void PlayerDeath(int number)
    {
        death_value -= number;
        updatePlayerDeath();
        if(death_value < 0)
        {
            print("gameover");
            GameOverPanel.SetActive(true);
            Invoke("loadSceneAgain", 4f);
        }
    }
    void loadSceneAgain()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
