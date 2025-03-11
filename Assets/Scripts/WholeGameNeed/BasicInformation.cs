using UnityEngine;

public class BasicInformation : MonoSingleton<BasicInformation>
{
    [HideInInspector]
    public Transform WholeMapMinPoint;
    [HideInInspector]
    public Transform WholeMapMaxPoint;
    [Tooltip("Ö÷½Ç")]
    public Transform Character;
    public override void Init()
    {
        WholeMapMinPoint.position = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0));
        WholeMapMaxPoint.position = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
    }
    public bool isOutMap(Vector2 position)
    {
        return position.x>=WholeMapMaxPoint.position.x ||position.x<= WholeMapMinPoint.position.x
            ||position.y>=WholeMapMaxPoint.position.y||position.y<=WholeMapMinPoint.position.y;
    }
}
