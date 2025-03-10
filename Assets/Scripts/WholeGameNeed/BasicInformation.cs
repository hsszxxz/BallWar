using UnityEngine;

public class BasicInformation : MonoSingleton<BasicInformation>
{
    [Tooltip("���ŵ�ͼx��yֵ��С�㣨�����½ǵĵ㣩")]
    public Transform WholeMapMinPoint;
    [Tooltip("���ŵ�ͼx��yֵ���㣨�����Ͻǵĵ㣩")]
    public Transform WholeMapMaxPoint;
    [Tooltip("����")]
    public Transform Character;
    public bool isOutMap(Vector2 position)
    {
        return position.x>WholeMapMaxPoint.position.x ||position.x< WholeMapMinPoint.position.x
            ||position.y>WholeMapMaxPoint.position.y||position.y<WholeMapMinPoint.position.y;
    }
}
