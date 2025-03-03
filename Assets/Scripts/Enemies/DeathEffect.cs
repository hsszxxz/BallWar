using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathEffect : MonoBehaviour
{
    public ParticleSystem explosionEffect;
    public Animator animator;
    public void BeginToDeath()
    {
        animator.SetBool("Dead", true);
        explosionEffect.Play();
        GameObjectPool.Instance.CollectObject(gameObject, explosionEffect.main.duration);
    }
}
