using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters;
using UnityEngine;

public class PlayerDead : MonoBehaviour
{

    public bool Dead { get; private set; }

    
    void Update()
    {
        if (Die() && !Dead)
        {
            StartCoroutine(GameOver());
            Dead = true;
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