using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{
    Transform player;

    void Start()
    {
        player = GameObject.Find("Player").transform;   
    }


    void LateUpdate()
    {

        transform.position = new Vector3(player.position.x, player.position.y, -10f);
    }
}
