using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour {

    public GameObject Player;

    private Vector2 velocity = Vector2.zero;
    public const float speed = 5;


    private Vector3 offset;

    private void Follow ()
    {
        var pos = transform.position;
        pos.x += velocity.x * Time.deltaTime;
        pos.y = Player.transform.position.y;
        transform.position = pos;
    }

    private void Start ()
    {
        velocity.x = speed;
        offset = transform.position - Player.transform.position;
	}
	
	void LateUpdate ()
    {
        Follow();
	}
}
