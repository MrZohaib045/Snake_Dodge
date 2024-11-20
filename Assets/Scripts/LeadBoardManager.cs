using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeadBoardManager : MonoBehaviour
{
    public List<Snake_Info> snakeInfoList = new List<Snake_Info>();
    public List<Text> playerNameFields;
    public List<Text> playerScoreFields;
    public GameObject King_Crown;
    public GameObject King;

    public void AddPlayerInfo(string name, int score)
    {
        Snake_Info newPlayer = new Snake_Info(name, score);
        snakeInfoList.Add(newPlayer);
        //SortLeaderboard(); // Sort after adding a new player
        UpdateLeaderboardDisplay();
    }
    public void UpdatePlayerScore(string name, int newScore)
    {
        for (int i = 0; i < snakeInfoList.Count; i++)
        {
            if (snakeInfoList[i].name == name)
            {
                snakeInfoList[i] = new Snake_Info(name, newScore);
                //SortLeaderboard(); // Sort after updating a score
                UpdateLeaderboardDisplay();
                return;
            }
        }
    }
    public void RemovePlayerInfo(string name)
    {
        for (int i = 0; i < snakeInfoList.Count; i++)
        {
            if (snakeInfoList[i].name == name)
            {
                snakeInfoList.RemoveAt(i); // Remove the player by index
                return;
            }
        }
    }

    public void UpdateLeaderboardDisplay()
    {
        snakeInfoList.Sort((a, b) => b.score.CompareTo(a.score));
        for (int i = 0; i < playerNameFields.Count; i++)
        {
            if (i < snakeInfoList.Count)
            {
                playerNameFields[i].text = snakeInfoList[i].name;
                playerScoreFields[i].text = snakeInfoList[i].score.ToString();
            }
            else
            {
                playerNameFields[i].text = "";
                playerScoreFields[i].text = "";
            }
        }
        UpdateKing();
    }

    private void UpdateKing()
    {
        if (snakeInfoList.Count == 0) return;
        // Find the top player from the leaderboard
        string topPlayerName = snakeInfoList[0].name;
        GameObject topPlayerObject = GameObject.Find(topPlayerName); // Assuming the GameObject is named after the player
        if (topPlayerObject != null)
        {
            King = topPlayerObject;
            // Position the crown on the top player's head
            Transform snakeHead = King.transform.GetChild(0).transform; 
            if (snakeHead != null && King_Crown != null)
            {
                King_Crown.transform.SetParent(snakeHead); // Attach crown to the head
                King_Crown.transform.localPosition = new Vector3(0.5f, 0, 0); // Adjust position above the head
            }
        }
    }
}

[System.Serializable]
public class Snake_Info
{
    public string name;
    public int score;
    public Snake_Info(string name, int score)
    {
        this.name = name;
        this.score = score;
    }
}
