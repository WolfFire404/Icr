using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreMaganer : MonoBehaviour
{
    public Text scoreText;
    public Text HighScoreText;

    void Start()
    {
        scoreText.text = "Score: " + Mathf.Round(PlayerPrefs.GetFloat("CurrentScore")).ToString();
        HighScoreText.text = "HighScore: " + Mathf.Round(PlayerPrefs.GetFloat("HighScore")).ToString();
    }
}
