using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FinishUIWindow : UIWindow
{
    public Text score;
    public Animator animator;
    public GameObject lightPicture;
    private Image image;
    private Color origin;
    private void Start()
    {
        image =lightPicture.GetComponent<Image>();
        origin = image.color;
        image.color = origin - new Color(0, 0, 0, 1);
    }
    public float FinishShow()
    {
        animator.SetBool("Finish", true);
        StartCoroutine(ShowLight());
        return animator.GetCurrentAnimatorStateInfo(0).length+1;
    }
    IEnumerator ShowLight()
    {
        yield return new  WaitForSeconds(1);
        while (Mathf.Abs(image.color.a -1f)> 0.01f)
        {
            yield return null;
             image.color = Color.Lerp(image.color,origin,2.5f*Time.deltaTime);
        }
    }
}
