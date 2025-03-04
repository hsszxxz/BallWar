using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathEffect : MonoBehaviour
{
    public ParticleSystem explosionEffect;
    public Animator animator;
    private void OnDisable()
    {
        animator.SetBool("Dead",false);
    }
    public void BeginToDeath()
    {
        animator.SetBool("Dead", true);
        explosionEffect.Play();
        GameObjectPool.Instance.CollectObject(gameObject, explosionEffect.main.duration);
    }
}
