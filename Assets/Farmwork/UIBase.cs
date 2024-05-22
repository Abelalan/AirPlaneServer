using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// ui基类
/// </summary>
public class UIBase : MonoBehaviour
{
    public Button ButClose;
    /// <summary>
    /// 初始化
    /// </summary>
    public virtual void Init()
    {
        ButClose.onClick.AddListener(HideUI);
    }
    public void ShowUI()
    {
        gameObject.SetActive(true);
    }
    public void HideUI()
    {
        gameObject.SetActive(false);
    }
}
