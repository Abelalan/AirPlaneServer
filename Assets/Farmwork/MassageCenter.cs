using System;
using System.Collections.Generic;
/// <summary>
/// 消息中心
/// </summary>
public class MassageCenter : Singleton<MassageCenter>
{
    Dictionary<int, Delegate> _dic = new Dictionary<int, Delegate>();
    /// <summary>
    /// 添加侦听
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="id"></param>
    /// <param name="callBack"></param>
    public void AddListener<T>(int id, Action<T> callBack)
    {
        if (_dic.ContainsKey(id))
        {
            _dic[id] = _dic[id] as Action<T> + callBack;
        }
        else
        {
            _dic.Add(id, callBack);
        }
    }
    /// <summary>
    /// 派发消息
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="id"></param>
    /// <param name="t"></param>
    public void Dispatch<T>(int id, T t)
    {
        if (_dic.ContainsKey(id))
        {
            Action<T> back = _dic[id] as Action<T>;
            if (back != null)
            {
                back(t);
            }
        }
    }

 
}
