using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private LayerMask collisionMask;
    private Vector2 velocity = Vector2.zero;
    private const float Gravity = 50;
    public const float StandardSpeed = 5;
    private const float JumpHeight = 10;

    bool facingRight = true;

    private bool grounded;
    
    private void Move()
    {
        var pos = transform.position;
        pos.x += velocity.x * Time.deltaTime;
        pos.y += velocity.y * Time.deltaTime;
        transform.position = pos;
    }

    private void Start()
    {
        velocity.x = StandardSpeed;
    }

    private void Update()
    {
        AddGravity();
        SideCollision();
        CheckGrounded();

        if (grounded)
            velocity.y = 0;
        
        if (Input.GetKeyDown(KeyCode.Space))
            Jump();    
        
        Move();
    }

    private void Jump()
    {
        if (grounded)
            velocity.y = JumpHeight;
        else
        {
            Vector3 direction = Vector2.right;
            if (velocity.x < 0) direction *= -1;
            var info = Physics2D.Raycast(transform.position, direction, velocity.x * Time.deltaTime, collisionMask);

            if (info)
            {
                Flip();
                velocity.y = JumpHeight;
            }
            
        }
    }

    private void AddGravity()
    {
        velocity.y -= Gravity * Time.deltaTime;
    }

    private void SideCollision()
    {
        Vector2 bottomRight = transform.position;
        bottomRight.x += 0.5f;
        bottomRight.y -= 0.4f;
        var topRight = bottomRight;
        topRight.y += 0.8f;

        for (var i = 0; i < 2; i++)
        {
            var info = Physics2D.Raycast(i == 0 ? bottomRight : topRight, Vector2.right,
                velocity.x * Time.deltaTime, collisionMask);

            if (!info) continue;
            
            transform.position = new Vector3(info.point.x - 0.5f, transform.position.y, transform.position.z);
            velocity.x = 0;
            return;
        }
    }

    private void CheckGrounded()
    {
        

        Vector2 bottomRight,
            bottomLeft = transform.position;
        bottomLeft.y -= 0.5f;
        bottomLeft.x -= 0.4f;
        bottomRight = bottomLeft;
        bottomRight.x += 0.8f;
        
        Debug.DrawLine(bottomRight, bottomRight + (Vector2.down * velocity.y * Time.deltaTime    ));

        for (var i = 0; i < 2; i++)
        {
            var info = Physics2D.Raycast(i == 0 ? bottomRight : bottomLeft, Vector2.down,
                velocity.y * Time.deltaTime * -1,
                collisionMask);
            
            if (!info) continue;
            
            // Move the player down.
            transform.position = new Vector3(transform.position.x, info.point.y + 0.5f, transform.position.z);
            grounded = true;
            return;
        }

        grounded = false;
    }

    void Flip()
    {

        // Switch the way the player is labelled as facing
        facingRight = !facingRight;

        //Multiply the player's x local cale by -1
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
}