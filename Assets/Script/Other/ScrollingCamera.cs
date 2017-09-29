using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollingCamera : MonoBehaviour
{
    [SerializeField] private float _offset;
    private PlayerMovement _movement;
    
    private void Start()
    {
        _movement = GameObject.FindGameObjectWithTag("Player")
            .GetComponent<PlayerMovement>();
        transform.position = new Vector3(_movement.transform.position.x, _movement.transform.position.y, -_offset);
    }

    private void LateUpdate()
    {
        var pos = transform.position;
        pos.x += _movement.WantedSpeed * Time.deltaTime;
        pos.y = Mathf.Lerp(pos.y, _movement.transform.position.y, 1 * Time.deltaTime);
        transform.position = pos;
    }
}