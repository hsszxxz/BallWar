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

    [Header("������")]
    [Tooltip("����ֵ")]public int health = 3;
    [Tooltip("��ť�Ƿ�ס")]public bool buttonHold = false;
    [Tooltip("�Ƿ��ڷ���")]public bool isFly = false;
    [Tooltip("�Ƿ�������")]public bool canHurt = true;

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
    [Tooltip("��������")]public int times = 1;
    [Tooltip("���빥���İ뾶")] public float hitRadius = 3;
    [Tooltip("������")] public float boundForce = 3f;
    [Tooltip("�Ƿ��л���")] public bool isShield;

    [Header("����")]
    [Tooltip("���µ�")] public Transform LD;
    [Tooltip("���ϵ�")] public Transform RU;
    [Tooltip("����")] public GameObject shieldObject;
    [Tooltip("״̬ͼ��")] public List<Sprite> C = new();

    [Header("��")]
    [Tooltip("������ǿ��")] public float powerStr = 0.05f;
    [Tooltip("��ɱ��ǿ��")] public float killStr = 0.2f;
    [Tooltip("������ǿ��")] public float boundStr = 0.2f;
    [Tooltip("������ǿ��")] public float deadStr = 1f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<Enemy>(out var enemy))
        {
            CalulateScore(enemy);
        }
        if(other.TryGetComponent<GameToolItem>(out var shield))
        {
            isShield = true;
            shieldObject.SetActive(isShield); 
            var s = shieldObject.GetComponent<SpriteRenderer>();
            s.color = Color.white;
            shieldObject.transform.localScale = Vector3.zero;
            shieldObject.transform.DOScale(1, 0.8f);
            shield.CollectTool();
        } 
        if(other.TryGetComponent<ScoreItem>(out var candy))
        {
            candy.CollectScore();
        }
    }


    void Update()
    {
        if(!isFly && health > 0) 
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

    float addTime = 0;
    private void TargetRotate()
    {
        if (isFly)
            return;

        if (!WallCheck())
            target.localPosition = new Vector3(0, currentRadius, 0);

        addTime += Time.deltaTime;
        Quaternion playerRotation = Quaternion.Euler(0, 0, addTime * rotateSpeed);
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
                    CameraShake.Instance.TriggerShake(powerStr);
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

        RayBound();
    }

    private bool WallCheck()
    {
        Vector3 p = transform.position + (target.position - transform.position).normalized * currentRadius;
        float l = LD.position.x;
        float d = LD.position.y;
        float r = RU.position.x;
        float u = RU.position.y;

        if (p.x > l && p.y > d && p.x < r && p.y < u && target.position != Vector3.zero)
        {
            return false;
        }

        float e = 0f;
        //��ֱ
        if (p.x <= l + e || p.x >= r - e)
            target.position = p.x >= r ? FindIntersection(new Vector2(r, d), RU.position) : FindIntersection(LD.position, new Vector2(l, u));
        //ˮƽ
        if (p.y <= d + e || p.y >= u - e)
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
            CameraShake.Instance.TriggerShake(killStr);
            times++;
            return;
        }

        if (!canHurt)
            return;

        if (isShield)
        {
            isShield = false;
            var sprite = shieldObject.GetComponent<SpriteRenderer>();
            shieldObject.transform.DOScale(10, 0.8f);
            DOTween.To((val) => { sprite.color = new Vector4(sprite.color.r, sprite.color.g, sprite.color.b, val); }, 1, 0, 0.8f)
                .OnComplete(() =>
                {
                    shieldObject.SetActive(isShield);
                });
            return;
        }

        if (!canHurt)
            return;

        health--;
        sprite.sprite = C[3 - health];
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
        Vector2 start = transform.position, end = target.position;
        Vector2 dir = (end - start).normalized;

        float e = 0.5f;
        for (int i = 0; i < 4; i++)
        {
            List<Vector2> normals = new List<Vector2>();
            List<ShakeType> currentShakes = new List<ShakeType>();

            if (end.x <= LD.position.x + e)
            {
                normals.Add(Vector2.right);
                currentShakes.Add(ShakeType.Horizontal);
            }
            if (end.x >= RU.position.x - e)
            {
                normals.Add(Vector2.left);
                currentShakes.Add(ShakeType.Horizontal);
            }
            if (end.y <= LD.position.y + e)
            {
                normals.Add(Vector2.up);
                currentShakes.Add(ShakeType.Vertical);
            }
            if (end.y >= RU.position.y - e)
            {
                normals.Add(Vector2.down);
                currentShakes.Add(ShakeType.Vertical);
            }

            if (normals.Count == 0)
                break;

            boundPoints.Add(end);
            shakeModeList.AddRange(currentShakes);
            foreach (Vector2 n in normals)
            {
                dir = Vector2.Reflect(dir, n).normalized;
            }
            start = end;
            end = start + dir * boundForce;
        }

        var sequence = DOTween.Sequence();
        for (int i = 0; i < boundPoints.Count; i++)
        {
            sequence.Append(transform.DOMove(boundPoints[i], moveTime)
                .OnComplete(()=>
                {
                    CameraShake.Instance.TriggerShake(boundStr, 0.5f,shakeModeList[i - 1]);
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
        if (!canHurt)
            yield break;
        
        canHurt = false;
        Color col = sprite.color;
        Sequence sequence = DOTween.Sequence();
        for (int i = 0; i < 5; i++) 
        {
            sequence.Append(DOTween.To((val) => { sprite.color = new Vector4(col.r, col.g, col.b, val); }, 1, 0.1f, 0.15f)
                .OnComplete(() =>
                {
                    sprite.DOFade(1, 0.15f);
                }).SetDelay(0.3f)
            );
        }
        sequence.OnComplete(() =>
        {
            canHurt = true;
        });

    }

    private IEnumerator Dead()
    {
        target.GetComponent<SpriteRenderer>().sortingOrder = -5;
        CameraShake.Instance.TriggerShake(deadStr, 3);
        GameObjectPool.Instance.CreateObject("deatheffection", Resources.Load("Prefab/DeathEffection") as GameObject, transform.position, Quaternion.identity).transform.GetComponent<DeathEffect>().BeginToDeath();
        yield return new WaitForSeconds(3);
        ScoreControl.Instance.FinishGameScoreShow();
    }

}