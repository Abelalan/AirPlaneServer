using UnityEngine;

/// <summary>
/// ·ºÐÍµ¥Àý
/// </summary>
/// <typeparam name="T"></typeparam>
public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    static T instance;
    private static T GetOrFindTComponent()
    {
        if (instance == null)
        {
            instance = FindObjectOfType<T>();
            if (instance == null)
            {
                GameObject obj = new GameObject();
                obj.name = typeof(T).Name;
                instance = obj.AddComponent<T>();
            }
        }
        return instance;
    }

    public static T GetInstance() => GetOrFindTComponent();
}
