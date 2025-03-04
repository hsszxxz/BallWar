using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MultItem : MonoBehaviour
{
    private Text text;
    private Animator animator;
    public void ShowMult(int num)
    {
        text.text = "¡Á" + num;
        animator.SetBool("show", true);
    }
    public void ShutMult(float lastTime)
    {
        GameObjectPool.Instance.CollectObject(gameObject,lastTime);
    }
    private void Awake()
    {
        text = GetComponent<Text>();
        animator = GetComponent<Animator>();
    }
    private void OnDisable()
    {
        animator.SetBool("show", false);
    }
}
