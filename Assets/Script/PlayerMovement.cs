using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Vector2 velocity = Vector2.zero;
    const float gravity = 20;
    const float standardSpeed = 5;

    void Move()
    {

        var pos = transform.position;
        pos.x += velocity.x * Time.deltaTime;
        pos.y += velocity.y * Time.deltaTime;
        transform.position = pos;
    }

    void Start()
    {
        velocity.x = standardSpeed;

    }

    void Update()
    {
        Move(); 
    }
}
