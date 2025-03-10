using UnityEngine;

public class BasicInformation : MonoSingleton<BasicInformation>
{
    [Tooltip("整张地图x，y值最小点（最左下角的点）")]
    public Transform WholeMapMinPoint;
    [Tooltip("整张地图x，y值最大点（最右上角的点）")]
    public Transform WholeMapMaxPoint;
    [Tooltip("主角")]
    public Transform Character;
    public bool isOutMap(Vector2 position)
    {
        return position.x>WholeMapMaxPoint.position.x ||position.x< WholeMapMinPoint.position.x
            ||position.y>WholeMapMaxPoint.position.y||position.y<WholeMapMinPoint.position.y;
    }
}
