using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using EnemySpace;

public class PlayerCtrl : MonoBehaviour
{
    public Transform target;

    [Tooltip("��ť�Ƿ�ס")]public bool buttonHold = false;
    [Tooltip("�Ƿ��ڷ���")]public bool isFly = false;

    [Header("�뾶")]
    [Tooltip("���ڰ뾶")] public float currentRadius = 1f;
    [Tooltip("Ĭ�ϰ뾶")] public float defaultRadius = 1f;
    [Tooltip("���뾶")] public float maxRadius = 5f;
    [Tooltip("�뾶�����ٶ�")] public float radiusAddSpeed = 0.5f;
    [Tooltip("�뾶���Ӽ��ʱ��")] public float radiusAddPerTimes = 0.1f;

    [Header("�ƶ�")]
    [Tooltip("�յ���ת�ٶ�")] public float rotateSpeed = 30f;
    [Tooltip("�ƶ���ʽ")] public Ease ease = Ease.Linear;
    [Tooltip("�ƶ�ʱ��")] public float moveTime = 0.2f;

    [Header("����")]
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