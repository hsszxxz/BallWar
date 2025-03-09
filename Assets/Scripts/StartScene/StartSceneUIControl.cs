using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartSceneUIControl : MonoBehaviour
{
    [Tooltip("Ö÷³¡¾°Ãû")]
    public string nextSceneName;
    public GameObject BGM;
    private void Start()
    {
        AudioManager.Instance.PlayBackgroundMusic(AudioManager.Instance.BGM);
        DontDestroyOnLoad(BGM);
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Time.timeScale = 1;
            SceneManager.LoadScene(nextSceneName);
        }
    }
}
