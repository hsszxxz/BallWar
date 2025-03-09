using JetBrains.Annotations;
using ScoreSpace;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class UIControl :UIWindow
{
    public GameObject levelNum;
    public GameObject scoreGo;
    public GameObject scorePlusGo;
    private Text scoreText;
    private Text scorePlusText;
    private Text levelNumText;
    private Animator scorePlusAnimator;
    private Animator levelNumAnimator;
    private string[] bigWriteNum = new string[10] { "十", "一", "二", "三", "四", "五", "六", "七", "八", "九" };
    protected override void Awake()
    {
        base.Awake();
        levelNumAnimator = levelNum.GetComponent<Animator>();
        scorePlusAnimator = scorePlusGo.GetComponent<Animator>();
        scoreText = scoreGo.GetComponent<Text>();
        scorePlusText = scorePlusGo.GetComponent<Text>();
        levelNumText = levelNum.GetComponent<Text>();
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
        scoreText.text = null;
        foreach (char c in newScore)
        {
            scoreText.text += c;
            yield return new WaitForSeconds(singleWordLastTime);
        }
        scorePlusAnimator.SetBool("plus", false);
    }
    public void UIScorePlus(int num)
    {
        if (num != 0)
        {
            scorePlusText.text = "+" + num.ToString();
            scorePlusAnimator.SetBool("plus", true);
            StartCoroutine(PrintScore(0.1f));
        }
    }
    public void LevelFade()
    {
        levelNumAnimator.SetBool("showNum", false);
    }
    public void LevelShow(int level)
    {
        string FinalStr = null;
        if (level < 10)
        {
            FinalStr = bigWriteNum[level];
        }
        else if (level < 20)
        {
            FinalStr = bigWriteNum[0];
            if (level != 10)
            {
                FinalStr += bigWriteNum[level - 10];
            }
        }
        else if (level<100)
        {
            FinalStr = bigWriteNum[(int)(level / 10)];
            FinalStr += bigWriteNum[0];
            if (level%10!=0)
            {
                FinalStr += bigWriteNum[level % 10];
            }
        }
        levelNumAnimator.SetBool("showNum", true);
        levelNumText.text = "第" + FinalStr + "关";
    }
}
