using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class HealthBar : MonoBehaviour
{
    public Slider slider;

    public void SetMaxHelath(int health)
    {
        slider.maxValue = health;
        slider.value = health;
    }

    public void setHealth(int health)
    {
        slider.value = health;
    }
    void Update()
    {
        if (slider.value <=0)
        {
            SceneManager.LoadScene(0);
            PlayerPrefs.SetInt("CurrentScore", 0);
        }
    }
}
