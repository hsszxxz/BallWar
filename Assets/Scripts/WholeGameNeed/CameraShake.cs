using UnityEngine;

public class CameraShake : MonoSingleton<CameraShake>
{
    public enum ShakeType
    {
        Horizontal,     // 水平震动
        Vertical,       // 垂直震动
        RandomDirection,// 随机方向
        Explosion       // 爆炸式震动（先强后弱）
    }

    [Header("震动参数")]
    [Tooltip("震动类型")]
    public ShakeType shakeType = ShakeType.RandomDirection;
    [Tooltip("震动强度")]
    public float intensity = 0.1f;
    [Tooltip("持续时间")]
    public float duration = 0.5f;
    [Tooltip("震动频率")]
    public float frequency = 25f;
    [Tooltip("使用平滑噪声")]
    public bool useSmoothNoise = true;
    [Tooltip("启用衰减")]
    public bool enableDecay = true;

    private Vector3 originalPos;
    private Coroutine shakeCoroutine;

    void Start()
    {
        originalPos = transform.localPosition;
    }

    // 外部调用触发震动
    public void TriggerShake(float intensity = 0.1f)
    {
        if (shakeCoroutine != null)
        {
            StopCoroutine(shakeCoroutine);
        }
        shakeCoroutine = StartCoroutine(Shake(intensity));
    }

    System.Collections.IEnumerator Shake(float intensity)
    {
        float elapsed = 0f;
        float currentIntensity = intensity;

        while (elapsed < duration)
        {
            // 计算衰减
            if (enableDecay)
            {
                currentIntensity = intensity * (1 - elapsed / duration);
            }

            // 计算噪声
            float noiseX = useSmoothNoise ?
                Mathf.PerlinNoise(Time.time * frequency, 0) * 2 - 1 :
                Random.Range(-1f, 1f);

            float noiseY = useSmoothNoise ?
                Mathf.PerlinNoise(0, Time.time * frequency) * 2 - 1 :
                Random.Range(-1f, 1f);

            // 应用震动方向
            Vector3 offset = Vector3.zero;
            switch (shakeType)
            {
                case ShakeType.Horizontal:
                    offset = transform.right * noiseX * currentIntensity;
                    break;
                case ShakeType.Vertical:
                    offset = transform.up * noiseY * currentIntensity;
                    break;
                case ShakeType.RandomDirection:
                    offset = new Vector3(noiseX, noiseY, 0) * currentIntensity;
                    break;
                case ShakeType.Explosion:
                    float explosionForce = Mathf.Lerp(intensity, 0, elapsed / duration);
                    offset = Random.insideUnitSphere * explosionForce;
                    break;
            }

            transform.localPosition = originalPos + offset;

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPos;
    }

#if UNITY_EDITOR
    // 示例：按回退键测试震动
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            TriggerShake(intensity);
        }
    }
#endif

}