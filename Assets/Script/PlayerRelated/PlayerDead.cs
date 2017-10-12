using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters;
using UnityEngine;

public class PlayerDead : MonoBehaviour
{
    [SerializeField] private AudioClip oof;
    public bool Dead { get; private set; }
    private AudioSource _audioSource;

    void Awake()
    {
        _audioSource = gameObject.AddComponent<AudioSource>();
        _audioSource.loop = _audioSource.playOnAwake = false;
        _audioSource.clip = oof;
        _audioSource.pitch = Random.Range(0.01f, 1.2f);
        _audioSource.volume = 100;

    }
    
    void Update()
    {
        if (Die() && !Dead)
        {
            StartCoroutine(GameOver());
            Dead = true;
            _audioSource.Play();
        }
    }

    bool Die()
    {
        float width = Camera.main.orthographicSize * Camera.main.aspect;
        return (transform.position.x < Camera.main.transform.position.x - width - 0.5f) || 
            transform.position.y < Camera.main.transform.position.y - Camera.main.orthographicSize - 1f;
    }

    IEnumerator GameOver()
    {
        yield return new WaitForSeconds(1f);
        PlayerPrefs.SetFloat("CurrentScore", Mathf.Round(ScoreManager.scoreCount));
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameOver");
    }
}