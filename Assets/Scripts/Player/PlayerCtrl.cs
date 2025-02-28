using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using EnemySpace;

public class PlayerCtrl : MonoBehaviour
{
    public Transform target;

    [Tooltip("按钮是否按住")]public bool buttonHold = false;
    [Tooltip("是否在飞行")]public bool isFly = false;

    [Header("半径")]
    [Tooltip("现在半径")] public float currentRadius = 1f;
    [Tooltip("默认半径")] public float defaultRadius = 1f;
    [Tooltip("最大半径")] public float maxRadius = 5f;
    [Tooltip("半径增加速度")] public float radiusAddSpeed = 0.5f;
    [Tooltip("半径增加间隔时间")] public float radiusAddPerTimes = 0.1f;

    [Header("移动")]
    [Tooltip("终点旋转速度")] public float rotateSpeed = 30f;
    [Tooltip("移动方式")] public Ease ease = Ease.Linear;
    [Tooltip("移动时间")] public float moveTime = 0.2f;

    [Header("增益")]
    public int times = 1;

    void Start()
    {

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.TryGetComponent<Enemy>(out var enemy))
        {
            CalulateScore(enemy);
        }
    }


    void Update()
    {
        buttonHold = Input.GetKey(KeyCode.Space);
        KeyControl();
        TargetRotate();
    }

    private void KeyControl()
    {
        if (!buttonHold)
            return;
        StartCoroutine(RadiusAdd());
    }

    private void TargetRotate()
    {
        target.localPosition = new Vector3(0, currentRadius, 0);
        Quaternion targetRotation = Quaternion.Euler(0, 0, rotateSpeed * Time.time);
        transform.rotation = targetRotation;
    }

    private IEnumerator RadiusAdd()
    {
        while(buttonHold) 
        {
            if (currentRadius < maxRadius)
                currentRadius += radiusAddSpeed;
            else
                currentRadius = maxRadius;

            yield return new WaitForSeconds(radiusAddPerTimes);
        }

        isFly = true;
        var targetPos = target.position;
        target.DOMove(targetPos, moveTime).SetEase(ease);
        transform.DOMove(targetPos, moveTime).SetEase(ease).OnComplete(() =>
        {
            currentRadius = defaultRadius; 
            isFly = false;
            times = 1;
        });

        StopAllCoroutines();
    }

    private void CalulateScore(Enemy enemy)
    {
        if (isFly)
        {
            enemy.EnemyDie();
            //Score Add
        }

        //GAME OVER
    }
}