using JetBrains.Annotations;
using ScoreSpace;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class UIControl : MonoSingleton<UIControl>
{
    public GameObject scoreGo;
    public GameObject scorePlusGo;
    private Text scoreText;
    private Text scorePlusText;
    private Animator scorePlusAnimator;
    private void Start()
    {
        scorePlusAnimator = scorePlusGo.GetComponent<Animator>();
        scoreText = scoreGo.GetComponent<Text>();
        scorePlusText = scorePlusGo.GetComponent<Text>();
        ScoreControl.Instance.PlusScore(3,Vector3.zero,0.5f);
    }
    private IEnumerator PrintScore(float singleWordLastTime)
    {
        string old = scoreText.text;
        string newScore = ScoreControl.Instance.Scores.ToString();
        for (int i = 0; i < old.Length; i++)
        {
            scoreText.text = null;
            for (int j = 0; j < old.Length-1 - i; j++)
            {
                scoreText.text += old[j];
            }
            yield return new WaitForSeconds(singleWordLastTime/2);
        }
        yield return new WaitForSeconds(singleWordLastTime*2);
        foreach (char c in newScore)
        {
            scoreText.text += c;
            yield return new WaitForSeconds(singleWordLastTime);
        }
        scorePlusAnimator.SetBool("plus", false);
    }
    public void UIScorePlus(int num)
    {
        scorePlusText.text ="+"+ num.ToString();
        scorePlusAnimator.SetBool("plus", true);
        StartCoroutine(PrintScore(0.1f));
    }
}
