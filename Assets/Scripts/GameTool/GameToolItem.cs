using UnityEngine;

public class GameToolItem : MonoBehaviour
{
    public void CollectTool()
    { 
        GameObjectPool.Instance.CollectObject(gameObject);
        AudioManager.Instance.PlaySoundEffect(AudioManager.Instance.Peek);
    }
}
