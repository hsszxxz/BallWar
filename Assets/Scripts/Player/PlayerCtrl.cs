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

    [Header("主属性")]
    [Tooltip("生命值")]public int health = 3;
    [Tooltip("按钮是否按住")]public bool buttonHold = false;
    [Tooltip("是否在飞行")]public bool isFly = false;
    [Tooltip("是否能受伤")]public bool canHurt = true;

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
    [Tooltip("连击个数")]public int times = 1;
    [Tooltip("进入攻击的半径")] public float hitRadius = 3;
    [Tooltip("反弹力")] public float boundForce = 3f;
    [Tooltip("是否有护盾")] public bool isShield;

    [Header("其他")]
    [Tooltip("左下点")] public Vector3 LD;
    [Tooltip("右上点")] public Vector3 RU;
    [Tooltip("护盾")] public GameObject shieldObject;
    [Tooltip("角色图像")] public List<Sprite> playerTex = new();
    [Tooltip("光照")][ColorUsage(true, true)] public Color litColor;

    [Header("震动")]
    [Tooltip("蓄力震动强度")] public float powerStr = 0.05f;
    [Tooltip("击杀震动强度")] public float killStr = 0.2f;
    [Tooltip("反弹震动强度")] public float boundStr = 0.2f;
    [Tooltip("死亡震动强度")] public float deadStr = 1f;

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

    private void Start()
    {
        LD = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0));
        RU = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
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
                {
                    CameraShake.Instance.TriggerShake(powerStr);
                    sprite.material.SetColor("_Color", litColor);
                }
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

    private bool WallCheck()
    {
        Vector3 p = transform.position + (target.position - transform.position).normalized * currentRadius;
        float l = LD.x;
        float d = LD.y;
        float r = RU.x;
        float u = RU.y;

        if (p.x > l && p.y > d && p.x < r && p.y < u && target.position != Vector3.zero)
        {
            return false;
        }

        float e = 0f;
        //竖直
        if (p.x <= l + e || p.x >= r - e)
            target.position = p.x >= r ? FindIntersection(new Vector2(r, d), RU) : FindIntersection(LD, new Vector2(l, u));
        //水平
        if (p.y <= d + e || p.y >= u - e)
            target.position = p.y >= u ? FindIntersection(new Vector2(l, u), RU) : FindIntersection(LD, new Vector2(r, d));
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
            AudioManager.Instance.PlaySoundEffect(AudioManager.Instance.Kill);
            CameraShake.Instance.TriggerShake(killStr);
            times++;
            return;
        }

        if (!canHurt && health > 0)
        {
            enemy.EnemyDie();
            return;
        }

        if (isShield)
        {
            isShield = false;
            enemy.EnemyDie();
            var sprite = shieldObject.GetComponent<SpriteRenderer>();
            shieldObject.transform.DOScale(10, 0.8f);
            DOTween.To((val) => { sprite.color = new Vector4(sprite.color.r, sprite.color.g, sprite.color.b, val); }, 1, 0, 0.8f)
                .OnComplete(() =>
                {
                    shieldObject.SetActive(isShield);
                });
            return;
        }

        health--;
        if (health > 0)
        {
            sprite.sprite = playerTex[3 - health];
            sprite.material.SetTexture("_MainTex", sprite.sprite.texture);
        }
        enemy.EnemyDie();
        StartCoroutine(GetHurt());
        if (health == 0)
        {
            StartCoroutine(Dead());
        }
    }

    private void Move()
    {
        isFly = true;
        holdTime = 0;

        List<Vector2> boundPoints = new();
        List<ShakeType> shakeModeList = new();
        Vector2 start = transform.position, end = target.position;
        Vector2 dir = (end - start).normalized;

        float e = 0.5f;
        for (int i = 0; i < 3; i++)
        {
            List<Vector2> normals = new List<Vector2>();
            List<ShakeType> currentShakes = new List<ShakeType>();

            if (end.x <= LD.x + e)
            {
                normals.Add(Vector2.right);
                currentShakes.Add(ShakeType.Horizontal);
            }
            if (end.x >= RU.x - e)
            {
                normals.Add(Vector2.left);
                currentShakes.Add(ShakeType.Horizontal);
            }
            if (end.y <= LD.y + e)
            {
                normals.Add(Vector2.up);
                currentShakes.Add(ShakeType.Vertical);
            }
            if (end.y >= RU.y - e)
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

        if (boundPoints.Count != 0 
            && (1 - Mathf.Abs(Vector2.Dot(dir, Vector2.right)) < 0.01f || 1 - Mathf.Abs(Vector2.Dot(dir, Vector2.up)) < 0.01f))
        {
            var p = boundPoints[0];
            boundPoints.Clear();
            boundPoints.Add(p);
        }
        var sequence = DOTween.Sequence();
        AudioManager.Instance.PlaySoundEffect(AudioManager.Instance.Dash);
        int m = 0;
        foreach (var p in boundPoints)
        {
            int currentM = m;
            sequence.Append(transform.DOMove(p, moveTime)
            .OnComplete(() =>
            {
                Debug.Log(shakeModeList[currentM]);
                CameraShake.Instance.TriggerShake(boundStr, 0.5f, shakeModeList[currentM]);
                AudioManager.Instance.PlaySoundEffect(AudioManager.Instance.Bound[currentM]);
            })
            ).SetEase(ease);
            m++;
        }
        sequence.Append(transform.DOMove(end, moveTime)).SetEase(ease);
        sequence.OnComplete(() =>
        {
            float r = 0.5f;
            if (end.x + r < LD.x)
                end = new Vector2(LD.x + r, end.y);
            if (end.y + r < LD.y)
                end = new Vector2(end.x, LD.y + r);
            if (end.x + r > RU.x)
                end = new Vector2(RU.x - r, end.y);
            if (end.y + r > RU.y)
                end = new Vector2(end.x, RU.y - r);
            transform.DOMove(end, 0.1f).SetEase(ease);

            isFly = false;
            currentRadius = defaultRadius;
            sprite.material.SetColor("_Color", Color.black);
            ScoreControl.Instance.PlusScore(times - 1, transform.position, 1.8f);
            times = 1;
        });
    }

    private IEnumerator GetHurt()
    {
        if (!canHurt)
            yield break;

        canHurt = false;
        AudioManager.Instance.PlaySoundEffect(AudioManager.Instance.Hurt);
        Sequence sequence = DOTween.Sequence();
        for (int i = 0; i < 5; i++) 
        {
            sequence.Append(DOTween.To((val) => { sprite.material.SetVector("_Alpha", new Vector4(val, val, val, val)); }, 1, 0.1f, 0.15f)
                .OnComplete(() =>
                {
                    DOTween.To((val) => { sprite.material.SetVector("_Alpha", new Vector4(val, val, val, val)); }, 0.1f, 1f, 0.15f);
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
        sprite.sortingOrder = -5;
        target.GetComponent<SpriteRenderer>().sortingOrder = -5;
        shieldObject.transform.GetComponent<SpriteRenderer>().sortingOrder = -5;
        CameraShake.Instance.TriggerShake(deadStr, 3);
        GameObjectPool.Instance.CreateObject("deatheffection", Resources.Load("Prefab/DeathEffection") as GameObject, transform.position, Quaternion.identity).transform.GetComponent<DeathEffect>().BeginToDeath();
        yield return new WaitForSeconds(3);
        ScoreControl.Instance.FinishGameScoreShow();
    }

}