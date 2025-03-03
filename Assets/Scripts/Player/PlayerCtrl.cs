using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using EnemySpace;
using ScoreSpace;

public class PlayerCtrl : MonoBehaviour
{
    public Transform target;

    [Tooltip("按钮是否按住")]public bool buttonHold = false;
    [Tooltip("是否在飞行")]public bool isFly = false;

    [Header("半径")]
    [Tooltip("现在半径")] public float currentRadius = 1f;
    [Tooltip("默认半径")] public float defaultRadius = 1f;
    [Tooltip("最大半径")] public float maxRadius = 5f;
    [Tooltip("当前蓄力时间")] public float holdTime = 0f;
    [Tooltip("最大蓄力时间")] public float maxHoldTime = 4f;
    [Tooltip("时间-半径曲线")] public AnimationCurve radiusAddSpeedCurve;
    [Tooltip("半径增加间隔时间")] public float radiusAddPerTimes = 0.1f;

    [Header("移动")]
    [Tooltip("终点旋转速度")] public float rotateSpeed = 30f;
    [Tooltip("移动方式")] public Ease ease = Ease.Linear;
    [Tooltip("移动时间")] public float moveTime = 0.2f;

    [Header("增益")]
    public int times = 1;
    public float hitRadius = 3;

    void Start()
    {

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<Enemy>(out var enemy))
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
            if (currentRadius < maxRadius && holdTime < maxHoldTime)
            {
                //currentRadius += radiusAddSpeed;
                holdTime += Time.deltaTime;
                currentRadius = radiusAddSpeedCurve.Evaluate(holdTime / maxHoldTime) * maxRadius + defaultRadius;
                if (currentRadius >= hitRadius)
                    CameraShake.Instance.TriggerShake();
            }
            else
            {
                currentRadius = maxRadius;
                holdTime = maxHoldTime;
            }
            yield return new WaitForSeconds(radiusAddPerTimes);
        }

        isFly = true;
        var targetPos = target.position;
        holdTime = 0;
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
        if (isFly && currentRadius >= hitRadius)
        {
            enemy.EnemyDie();
            CameraShake.Instance.TriggerShake(0.3f);
            ScoreControl.Instance.PlusScore(times*2);
            times++;
        }

        //GAME OVER
    }
}