using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private void Awake()
    {
        OnLineManager.GetInstance().InitRegister();
        NetManager.GetInstance().InitServer();
    }
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        // 打开主面板
        UIManager.GetInstance().OpenUI(PanelName.MainPanelView);
        // 加载飞机模型
        GameObject airplane = GameObject.Instantiate(Resources.Load<GameObject>("FA_N26_Color_4_Prefab"));
        airplane.name = "Airplane";
        airplane.transform.position = new Vector3(0, 70, 0);
        //airplane.AddComponent<PlayerControl>();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
