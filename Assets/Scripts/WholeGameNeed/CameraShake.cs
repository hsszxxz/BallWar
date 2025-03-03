using UnityEngine;

public class CameraShake : MonoSingleton<CameraShake>
{
    public enum ShakeType
    {
        Horizontal,     // ˮƽ��
        Vertical,       // ��ֱ��
        RandomDirection,// �������
        Explosion       // ��ըʽ�𶯣���ǿ������
    }

    [Header("�𶯲���")]
    [Tooltip("������")]
    public ShakeType shakeType = ShakeType.RandomDirection;
    [Tooltip("��ǿ��")]
    public float intensity = 0.1f;
    [Tooltip("����ʱ��")]
    public float duration = 0.5f;
    [Tooltip("��Ƶ��")]
    public float frequency = 25f;
    [Tooltip("ʹ��ƽ������")]
    public bool useSmoothNoise = true;
    [Tooltip("����˥��")]
    public bool enableDecay = true;

    private Vector3 originalPos;
    private Coroutine shakeCoroutine;

    void Start()
    {
        originalPos = transform.localPosition;
    }

    // �ⲿ���ô�����
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
            // ����˥��
            if (enableDecay)
            {
                currentIntensity = intensity * (1 - elapsed / duration);
            }

            // ��������
            float noiseX = useSmoothNoise ?
                Mathf.PerlinNoise(Time.time * frequency, 0) * 2 - 1 :
                Random.Range(-1f, 1f);

            float noiseY = useSmoothNoise ?
                Mathf.PerlinNoise(0, Time.time * frequency) * 2 - 1 :
                Random.Range(-1f, 1f);

            // Ӧ���𶯷���
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
    // ʾ���������˼�������
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            TriggerShake(intensity);
        }
    }
#endif

}