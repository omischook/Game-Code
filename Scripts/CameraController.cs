using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject player;//this is linked to the player so that we know the position of the player 
    // Start is called before the first frame update
    public float offset;
    public float offSetSmoothing;
    private Vector3 playerPosition;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        playerPosition = new Vector3(player.transform.position.x, transform.position.y, transform.position.z);
        //x matches the player but doesnt change on any other axis this means the camera never goes above our background.
        if (player.transform.localScale.x > 0f)
        {
            playerPosition = new Vector3(playerPosition.x + offset, playerPosition.y, playerPosition.z);
        }
        else
        {
            playerPosition = new Vector3(playerPosition.x - offset, playerPosition.y, playerPosition.z);
        }
            transform.position = Vector3.Lerp(transform.position, playerPosition, offSetSmoothing * Time.deltaTime);
    }
}
