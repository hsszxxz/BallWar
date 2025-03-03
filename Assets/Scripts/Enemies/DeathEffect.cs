using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathEffect : MonoBehaviour
{
    public ParticleSystem explosionEffect;
    private Animator animator;
    void Start()
    {
        animator = GetComponent<Animator>();
        BeginToDeath();
    }
    public void BeginToDeath()
    {
        animator.SetBool("Dead", true);
        explosionEffect.Play();
        Destroy(gameObject, explosionEffect.duration);
    }
}
