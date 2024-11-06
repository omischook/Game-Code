using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextLevel : MonoBehaviour
{
    public int min = 0;
    public int max = 0;
    public Score Score;

    // Start is called before the first frame update
    void Start()
    {
        max = (SceneManager.sceneCountInBuildSettings);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "NextLevel")
        {
            int randomNumber = Random.Range(min, max);
            SceneManager.LoadScene(randomNumber);
            Score.increaseScore();
        }
    }
}
