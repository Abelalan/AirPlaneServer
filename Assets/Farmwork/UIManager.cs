using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIManager : Singleton<UIManager>
{
    Dictionary<PanelName, UIBase> _uiDic = new Dictionary<PanelName, UIBase>();

    public Canvas Canvas;
    private void Awake()
    {
        Canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
    }

    /// <summary>
    /// 打开Ui面板
    /// </summary>
    /// <param name="name"></param>
    public void OpenUI(PanelName name)
    {
        UIBase temp = LoadUI(name);
        temp.ShowUI();
        temp.transform.SetAsLastSibling();
    }

    private UIBase LoadUI(PanelName name)
    {
        UIBase temp;
        if (!_uiDic.TryGetValue(name, out temp))
        {
            GameObject gg = Resources.Load<GameObject>("UIPanel/" + name.ToString());
            if (gg == null)
            {
                Debug.Log("加载错误，请重新加载");
            }
            else
            {
                GameObject clone = GameObject.Instantiate(gg, Canvas.transform, false);
                temp = clone.GetComponent<UIBase>();
                if (temp == null)
                {
                    Debug.Log("没有挂载脚本，或没有继承UIBase基类");
                }
                else
                {
                    _uiDic.Add(name, temp);
                    _uiDic[name].Init();
                }
            }
        }
        return temp;
    }
    /// <summary>
    /// 关闭ui面板
    /// </summary>
    /// <param name="name"></param>
    public void CloseUI(PanelName name)
    {
        if (_uiDic.ContainsKey(name))
        {
            _uiDic[name].HideUI();
        }
    }
    /// <summary>
    /// 获取UI面板的脚本
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="name"></param>
    /// <returns></returns>
    public T GetPanelScript<T>(PanelName name) where T : UIBase
    {
        if (_uiDic.ContainsKey(name))
        {
            return _uiDic[name] as T;
        }
        else
        {
            return null;
        }
    }
}
public enum PanelName
{
    MainPanelView,
}
