using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainPanelView : UIBase
{
    public Button AutoDriveButton;
    public Button ManualDriveButton;

    public bool IsAutoDrive = true;
    public override void Init()
    {

        AutoDriveButton.onClick.AddListener(() =>
        {
            IsAutoDrive = true;
            MassageCenter.GetInstance().Dispatch<bool>(MassageID.AutoDrive, IsAutoDrive);
        });
        ManualDriveButton.onClick.AddListener(() =>
        {
            IsAutoDrive = false;
            MassageCenter.GetInstance().Dispatch<bool>(MassageID.ManualDrive, IsAutoDrive);
        });

    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
