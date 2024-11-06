using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Score : MonoBehaviour
{
    public TextMeshProUGUI currentScore;
    public TextMeshProUGUI highScore;
    public int number = 0;


    void Start()
    {
        highScore.text = ("High score " + PlayerPrefs.GetInt("HighScore"));
        currentScore.text = ("Curent Score " + PlayerPrefs.GetInt("CurrentScore"));
    }

    public void increaseScore()
    {
        number = PlayerPrefs.GetInt("CurrentScore");
        number++;


        if (PlayerPrefs.GetInt("CurrentScore") >= PlayerPrefs.GetInt("HighScore", 0))
        {
            PlayerPrefs.SetInt("HighScore", number);
        }
        
        PlayerPrefs.SetInt("CurrentScore", number);

    }
}
