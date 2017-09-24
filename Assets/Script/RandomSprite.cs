using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Selects a random sprite to use when the object is created, or started.
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class RandomSprite : MonoBehaviour
{
    [SerializeField] private Sprite[] _possibleSprites;
    private SpriteRenderer _spriteRenderer;

    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();

        if (_possibleSprites.Length <= 1) return;

        _spriteRenderer.sprite = _possibleSprites[Random.Range(0, _possibleSprites.Length)];
    }
}