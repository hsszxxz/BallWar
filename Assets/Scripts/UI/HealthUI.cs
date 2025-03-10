using UnityEngine;

public class HealthUI : MonoBehaviour
{
    public CanvasGroup H1;
    public CanvasGroup H2;
    public CanvasGroup H3;
    public PlayerCtrl player;

    void Update()
    {
        switch (player.health)
        {
            case 2:
                H3.alpha = 0;
                break;
            case 1:
                    H2.alpha = 0;
                break;
            case 0:
                    H1.alpha = 0;
                break;
            default:
                H1.alpha = 1;
                H2.alpha = 1;
                H3.alpha = 1;
                break;
        }
    }
}
