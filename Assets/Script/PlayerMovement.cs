using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private LayerMask collisionMask;
    private Vector2 velocity = Vector2.zero;
    const float gravity = 20;
    const float standardSpeed = 5;

    private void Move()
    {
        var pos = transform.position;
        pos.x += velocity.x * Time.deltaTime;
        pos.y += velocity.y * Time.deltaTime;
        transform.position = pos;
    }

    private void Start()
    {
        velocity.x = standardSpeed;
    }

    private void Update()
    {
        AddGravity();
        Move();
    }

    private void AddGravity()
    {
        velocity.y -= gravity * Time.deltaTime;
        if (CheckGrounded())
            velocity.y = 0;
    }

    private bool CheckGrounded()
    {
        Vector2 bottomRight,
            bottomLeft = transform.position;
        bottomLeft.y -= 0.5f;
        bottomLeft.x -= 0.5f;
        bottomRight = bottomLeft;
        bottomRight.x += 1;
        
        Debug.DrawLine(bottomRight, bottomRight + (Vector2.down * velocity.y * Time.deltaTime * -1));

        for (var i = 0; i < 2; i++)
        {
            var info = Physics2D.Raycast(i == 0 ? bottomRight : bottomLeft, Vector2.down,
                velocity.y * Time.deltaTime * -1,
                collisionMask);
            
            if (!info) continue;
            
            // Move the player down.
            transform.position = new Vector3(transform.position.x, info.point.y + 0.5f, transform.position.z);
            return true;
        }

        return false;
    }
}