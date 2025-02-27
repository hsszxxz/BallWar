using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ScoreSpace
{
    public class ScoreItem : MonoBehaviour
    {
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision == BasicInformation.Instance.Character.GetComponent<Collision2D>())
            {
                ScoreControl.Instance.PlusScore(1);
            }
        }
    }
}
