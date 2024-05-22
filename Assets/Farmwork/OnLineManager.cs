using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 注册用户管理
/// </summary>
public class OnLineManager : Singleton<OnLineManager>
{
    public List<ClientInformation> OnLineUser = new List<ClientInformation>();

    

    public void InitRegister()
    {
        MassageCenter.GetInstance().AddListener<MassageData>(MassageID.RegisterUser, RegisterUserCall);
        

    }

    private void RegisterUserCall(MassageData data)
    {
        //AirPlaneMoveData user = AirPlaneMoveData.Parser.ParseFrom(data.Information);
        // 添加进客户端集合
        OnLineUser.Add(data.Client);
        Debug.Log("客户端放进集合");
        //NetManager.GetInstance().SendDataToClient(MassageID.RegisterUserCall, BitConverter.GetBytes(isReg), data.Client);
    }

    
}
