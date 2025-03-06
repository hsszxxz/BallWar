using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using EnemySpace;
using ScoreSpace;

public class PlayerCtrl : MonoBehaviour
{
    public Transform target;

    [Tooltip("��ť�Ƿ�ס")]public bool buttonHold = false;
    [Tooltip("�Ƿ��ڷ���")]public bool isFly = false;

    [Header("�뾶")]
    [Tooltip("���ڰ뾶")] public float currentRadius = 1f;
    [Tooltip("Ĭ�ϰ뾶")] public float defaultRadius = 1f;
    [Tooltip("���뾶")] public float maxRadius = 5f;
    [Tooltip("��ǰ����ʱ��")] public float holdTime = 0f;
    [Tooltip("�������ʱ��")] public float maxHoldTime = 4f;
    [Tooltip("ʱ��-�뾶����")] public AnimationCurve radiusAddSpeedCurve;
    [Tooltip("�뾶���Ӽ��ʱ��")] public float radiusAddPerTimes = 0.1f;

    [Header("�ƶ�")]
    [Tooltip("�յ���ת�ٶ�")] public float rotateSpeed = 30f;
    [Tooltip("�ƶ���ʽ")] public Ease ease = Ease.Linear;
    [Tooltip("�ƶ�ʱ��")] public float moveTime = 0.2f;

    [Header("����")]
    public int times = 1;
    public float hitRadius = 3;
    public float boundForce = 3f;
    public bool isShield;

    [Tooltip("���µ�")] public Transform LD;
    [Tooltip("���ϵ�")] public Transform RU;


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
        if(other.TryGetComponent<GameToolItem>(out var shield) /*����������������other.gameObject.layer == LayerMask.NameToLayer("Shield")*/)
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

        //��ֱ
        if (p.x <= l || p.x >= r)
            target.position = p.x >= r ? FindIntersection(new Vector2(r, d), RU.position) : FindIntersection(LD.position, new Vector2(l, u));
        //ˮƽ
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
        Debug.Log("���ߣ�"+normal);
        Debug.Log("����ǣ�"+dir);
        boundPoint = target.position;
        BoundDir = boundDir;
        Debug.Log("����ǣ�"+BoundDir);
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