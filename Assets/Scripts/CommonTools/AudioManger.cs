using UnityEngine;

public class AudioManager : MonoSingleton<AudioManager>
{
    private AudioSource audioSource;

    public AudioClip BGM;
    public AudioClip Kill;
    public AudioClip Hurt;
    public AudioClip Peek;

    public override void Init()
    {
        base.Init();
        audioSource = GetComponent<AudioSource>();
    }
    //播放背景音乐
    public void PlayBackgroundMusic(AudioClip clip)
    {
        audioSource.clip = clip;
        audioSource.loop = true;  // 背景音乐通常是循环播放的
        audioSource.Play();
    }

    //停止背景音乐
    public void StopBackgroundMusic()
    {
        audioSource.Stop();
    }

    //播放音效
    public void PlaySoundEffect(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }
}