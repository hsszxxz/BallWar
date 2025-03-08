using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using EnemySpace;
using ScoreSpace;

public class PlayerCtrl : MonoBehaviour
{
    public Transform target;
    public SpriteRenderer sprite;

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
    public int health = 3;
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
        if(other.TryGetComponent<GameToolItem>(out var shield))
        {
            isShield = true;
            shield.CollectTool();
        } 
        if(other.TryGetComponent<ScoreItem>(out var candy))
        {
            candy.CollectScore();
        }
    }


    void Update()
    {
        if(!isFly) 
            buttonHold = Input.GetKey(KeyCode.Space);

        target.gameObject.SetActive(!isFly);
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
        if (isFly)
            return;

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
                if (currentRadius - hitRadius is >= 0 and <= 0.1f)
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
        holdTime = 0;

        start = transform.position;
        RayBound();
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

    private void CalulateScore(Enemy enemy)
    {
        if (isFly && currentRadius >= hitRadius)
        {
            enemy.EnemyDie();
            CameraShake.Instance.TriggerShake();
            times++;
            return;
        }
        if (isShield)
        {
            isShield = false;
            return;
        }

        health--;
        StartCoroutine(GetHurt());
        if (health == 0)
        {
            StartCoroutine(Dead());
        }
    }

    private void RayBound()
    {
        List<Vector2> boundPoints = new();
        List<ShakeType> shakeModeList = new();
        Vector2 start = transform.position, end = target.position, normal;
        Vector2 dir = (end - start).normalized;

        float e = 0.2f;
        for (int i = 0; i < 5; i++)
        {
            if (end.x <= LD.position.x + e)
            {
                normal = Vector2.right;
                shakeModeList.Add(ShakeType.Horizontal);
            }
            else if (end.x >= RU.position.x - e)
            {
                normal = Vector2.left;
                shakeModeList.Add(ShakeType.Horizontal);
            }
            else if (end.y <= LD.position.y + e)
            {
                normal = Vector2.up;
                shakeModeList.Add(ShakeType.Vertical);
            }
            else if (end.y >= RU.position.y - e)
            {
                normal = Vector2.down;
                shakeModeList.Add(ShakeType.Vertical);
            }
            else
                break;

            boundPoints.Add(end);
            dir = Vector2.Reflect(dir, normal).normalized;
            start = end;
            end = start + dir * boundForce;
        }

        var sequence = DOTween.Sequence();
        for (int i = 0; i < boundPoints.Count; i++)
        {
            sequence.Append(transform.DOMove(boundPoints[i], moveTime)
                .OnComplete(()=>
                {
                    CameraShake.Instance.TriggerShake(0.5f, 0.5f,shakeModeList[i - 1]);
                })
            ).SetEase(ease);
        }
        sequence.Append(transform.DOMove(end, moveTime)).SetEase(ease);
        sequence.OnComplete(() =>
        {
            isFly = false;
            currentRadius = defaultRadius;
            ScoreControl.Instance.PlusScore(times - 1, transform.position, 1.8f);
            times = 1;
        });
    }

    private IEnumerator GetHurt()
    {
        Color col = sprite.color;
        for (int i = 0; i < 4; i++)
        {
            DOTween.To((val) => { sprite.color = new Vector4(col.r, col.g, col.b, val); }, 1, 0.1f, 0.15f)
                .OnComplete(()=>
                {
                    DOTween.To((val) => { sprite.color = new Vector4(col.r, col.g, col.b, val); }, 0.1f, 1, 0.15f);
                });
            yield return new WaitForSeconds(0.3f);
        }
    }

    private IEnumerator Dead()
    {
        gameObject.SetActive(false);
        CameraShake.Instance.TriggerShake(1, 3);
        GameObjectPool.Instance.CreateObject("deatheffection", Resources.Load("Prefab/DeathEffection") as GameObject, transform.position, Quaternion.identity).transform.GetComponent<DeathEffect>().BeginToDeath();
        yield return new WaitForSeconds(3);
        ScoreControl.Instance.FinishGameScoreShow();
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(start, boundPoint);
        Gizmos.DrawLine(end, boundPoint);
        Gizmos.DrawSphere(AAA, .3f);
    }

}