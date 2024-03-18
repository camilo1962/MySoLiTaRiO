using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOver : MonoBehaviour
{
    [SerializeField]
    Text scoreText;
    // Start is called before the first frame update
    void Start()
    {
        scoreText.text = "0";
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setScore(int score)
    {
        scoreText.text = score.ToString();
    }

    public void activate(bool turn)
    {
        gameObject.SetActive(turn);
    }
}
