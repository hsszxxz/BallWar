using UnityEngine;
namespace ScoreSpace
{
    public class ScoreItem : MonoBehaviour
    {
        public void CollectScore()
        {
            ScoreControl.Instance.PlusScore(1);
            GameObjectPool.Instance.CollectObject(gameObject);
            AudioManager.Instance.PlaySoundEffect(AudioManager.Instance.Peek);
        }
    }
}
