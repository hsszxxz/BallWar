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
    public float boundForce = 3f;
    public bool isShield;

    [Tooltip("左下点")] public Transform LD;
    [Tooltip("右上点")] public Transform RU;


    [Header("Debug")]
    public Vector2 BoundDir;
    public Vector2 start;
    public Vector2 end;
    public Vector2 boundPoint;
    public Vector2 AAA;



    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<Enemy>(out var enemy))
        {
            CalulateScore(enemy);
        }
        if(other.TryGetComponent<GameToolItem>(out var shield) /*有其他道具再区分other.gameObject.layer == LayerMask.NameToLayer("Shield")*/)
        {
            isShield = true;
            shield.CollectTool();
        }
    }


    void Update()
    {
        if(!isFly) 
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
        if (!WallCheck())
            target.localPosition = new Vector3(0, currentRadius, 0);

        Quaternion playerRotation = Quaternion.Euler(0, 0, rotateSpeed * Time.time);
        transform.rotation = playerRotation;
    }

    private IEnumerator RadiusAdd()
    {
        while(buttonHold) 
        {
            if (currentRadius < maxRadius && holdTime < maxHoldTime)
            {
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

        Move();
        StopAllCoroutines();
    }

    private void Move()
    {
        isFly = true;
        var targetPos = target.position;
        holdTime = 0;

        float distance = Vector2.Distance(transform.position, target.position);
        start = transform.position;

        target.DOMove(targetPos, moveTime).SetEase(ease);
        transform.DOMove(targetPos, moveTime).SetEase(ease).OnComplete(() =>
        {
            Debug.Log(currentRadius > distance);
            if (currentRadius > distance)
                ReBound();

            currentRadius = defaultRadius;
            ScoreControl.Instance.PlusScore(times-1, transform.position, 1.8f);
            times = 1;
            isFly = false;
        });
    }

    private bool WallCheck()
    {
        Vector3 p = transform.position + (target.position - transform.position).normalized * currentRadius;
        AAA = p;
        float l = LD.position.x;
        float d = LD.position.y;
        float r = RU.position.x;
        float u = RU.position.y;

        if (p.x > l && p.y > d && p.x < r && p.y < u && target.position != Vector3.zero) 
            return false;

        //竖直
        if (p.x <= l || p.x >= r)
            target.position = p.x >= r ? FindIntersection(new Vector2(r, d), RU.position) : FindIntersection(LD.position, new Vector2(l, u));
        //水平
        if (p.y <= d || p.y >= u)
            target.position = p.y >= u ? FindIntersection(new Vector2(l, u), RU.position) : FindIntersection(LD.position, new Vector2(r, d));
        return true;
    }

    public Vector3 FindIntersection(Vector3 b1, Vector3 b2)
    {
        Vector3 dirA = target.position - transform.position;
        Vector3 dirB = b2 - b1;
        float denominator = dirA.x * dirB.y - dirA.y * dirB.x;
        Vector3 offset = b1 - transform.position;
        float t = (offset.x * dirB.y - offset.y * dirB.x) / denominator;
        return transform.position + t * dirA;
    }

    private void ReBound()
    {
        Vector2 dir = (transform.position - target.position).normalized;
        Vector2 normal;

        float epsilon = 0.05f;
        if (Mathf.Abs(target.position.x - LD.position.x) < epsilon)
            normal = Vector2.right;
        else if (Mathf.Abs(target.position.x - RU.position.x) < epsilon)
            normal = Vector2.left;
        else if (Mathf.Abs(target.position.y - LD.position.y) < epsilon)
            normal = Vector2.up;
        else if (Mathf.Abs(target.position.y - RU.position.y) < epsilon)
            normal = Vector2.down;
        else
            return;

        Vector2 boundDir = Vector2.Reflect(dir, normal).normalized;
        Debug.Log("法线："+normal);
        Debug.Log("入射角："+dir);
        boundPoint = target.position;
        BoundDir = boundDir;
        Debug.Log("反射角："+BoundDir);
        transform.DOMove(new Vector2(target.position.x, target.position.y) + boundDir * boundForce, moveTime).SetEase(Ease.OutQuint);
        end = transform.position;
    }

    private void CalulateScore(Enemy enemy)
    {
        if (isFly && currentRadius >= hitRadius)
        {
            enemy.EnemyDie();
            CameraShake.Instance.TriggerShake(0.3f);
            //ScoreControl.Instance.PlusScore(times,transform.position,1.8f);
            times++;
        }
        if (isShield)
        {
            isShield = false;
            return;
        }
        //GAME OVER

    }


    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(start, boundPoint);
        Gizmos.DrawLine(end,boundPoint);
        Gizmos.DrawSphere(AAA, .3f);
    }
}