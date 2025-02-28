using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreCounter : MonoSingleton<ScoreCounter>
{
    private int _score = 0;
    public int Score 
    {  
        get { return _score; } 
        set { _score = value; }
    }    

    public void AddScore()
    {
        Score++;
    }
    public void RemoveScore()
    {
        Score--;
    }
    public void ResetScore()
    {
        Score = 0;
    }
}
