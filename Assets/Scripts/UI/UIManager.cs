using System.Collections.Generic;
using UnityEngine;
public class UIWindow : MonoBehaviour
{
    private CanvasGroup canvasGroup;
    protected virtual void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }
    public virtual void ShutAndOpen(bool flag)
    {
        canvasGroup.alpha = flag ? 1 : 0;
        canvasGroup.interactable = flag;
        canvasGroup.blocksRaycasts = flag;
    }
}
public class UIManager : MonoSingleton<UIManager>
{
    private Dictionary<string, UIWindow> UIWindowDic;
    public override void Init()
    {
        base.Init();
        UIWindowDic = new Dictionary<string, UIWindow>();
        UIWindow[] uiWindows = FindObjectsByType<UIWindow>(FindObjectsSortMode.None);
        foreach (UIWindow uiWindow in uiWindows)
        {
            UIWindowDic.Add(uiWindow.GetType().Name, uiWindow);
        }
    }
    public T GetUIWindow<T>() where T : class
    {
        string name = typeof(T).Name;
        if (UIWindowDic.ContainsKey(name))
        {
            return UIWindowDic[name] as T;
        }
        else
        {
            return null;
        }
    }
}

