using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathEffect : MonoBehaviour
{
    private ParticleSystem explosionEffect;
    private Animator animator;
    void Start()
    {
        explosionEffect = GetComponentInChildren<ParticleSystem>();
        animator = GetComponentInChildren<Animator>();
    }
    public void BeginToDeath()
    {
        animator.SetBool("Dead", true);
        explosionEffect.Play();
        GameObjectPool.Instance.CollectObject(gameObject, explosionEffect.main.duration);
    }
}
