using AirPlaneData;
using Google.Protobuf;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class AirPlaneControl : MonoBehaviour
{
    private bool IsAutoDrive = true;
    private bool isCalculate = true;
    // private bool isRotation = false;

    private int index = -1;
    private float speed = 25;
    // private float calculateAngle = 0;

    private Coroutine coroutine;
    List<Vector3> pointList = new List<Vector3>();

    AirPlaneMoveData data = new AirPlaneMoveData();
    // Start is called before the first frame update
    void Start()
    {
        // 监听自动飞行，手动飞行的方法
        MassageCenter.GetInstance().AddListener<bool>(MassageID.AutoDrive, (obj) => { IsAutoDrive = obj; });
        MassageCenter.GetInstance().AddListener<bool>(MassageID.ManualDrive, (obj) => { IsAutoDrive = obj; });

        for (int i = 0; i < 6; i++)
        {
            Vector2 point = Random.insideUnitCircle * 500;
            pointList.Add(new Vector3(point.x, 70, point.y));
        }

    }



    // Update is called once per frame
    void Update()
    {
        if (IsAutoDrive)
        {
            // 自动驾驶
            AutoDirve();

        }
        else
        {
            // 手动驾驶
            ManualDrive();
        }

        for (int i = 0; i < OnLineManager.GetInstance().OnLineUser.Count; i++)
        {
            data.Px = transform.position.x;
            data.Py = transform.position.y;
            data.Pz = transform.position.z;
            data.Speed = speed;
            data.Elevation = transform.rotation.x;
            data.Roll = transform.rotation.z;
            data.Rotation = transform.rotation.y;
            data.EulerAnglesX = transform.eulerAngles.x;
            data.EulerAnglesY = transform.eulerAngles.y;
            data.EulerAnglesZ = transform.eulerAngles.z;
            NetManager.GetInstance().SendDataToClient(MassageID.RegisteruserCall, data.ToByteArray(), OnLineManager.GetInstance().OnLineUser[i]);
        }



    }

    void AutoDirve()
    {

        // 计算距离
        if (isCalculate)
        {
            index = CalculateDis();
        }

        //Vector3 point = pointList[index];

        transform.Translate(Vector3.forward * speed * Time.deltaTime);
        //Quaternion q = Quaternion.LookRotation(pointList[index] - transform.position);
        //// transform.LookAt(pointList[index]);
        //if (Quaternion.Angle(transform.rotation, q) < 0.1f)
        //{
        //    isRotation = false;
        //    Debug.Log(isRotation);
        //}

        //if (isRotation)
        //{
        //    transform.rotation = Quaternion.Slerp(transform.rotation, q, Time.deltaTime / 2);
        //}

        //if (isCalculateTime)
        //{
        //    if (calculateTime <= 1)
        //    {
        //        calculateTime += Time.deltaTime;
        //        transform.eulerAngles = new Vector3(0, 0, calculateTime * 30);
        //    }
        //    else
        //    {
        //        calculateTime += Time.deltaTime;
        //        transform.eulerAngles = new Vector3(0, 0, (calculateTime - 1) * 30);

        //        if (calculateTime >= 2)
        //        {
        //            isCalculateTime = false;
        //        }
        //    }
        //}

        if (coroutine == null)
        {
            transform.LookAt(pointList[index]);
        }

        if (Vector3.Distance(pointList[index], transform.position) < 0.1f)
        {
            index++;
            if (index > pointList.Count - 1)
            {
                index = 0;
            }

            //isRotation = true;
            //Debug.Log(index);
            coroutine = StartCoroutine(Rotate(pointList[index]));
        }

    }

    IEnumerator Rotate(Vector3 point)
    {
        float t = 0;
        Quaternion q = Quaternion.LookRotation(pointList[index] - transform.position);
        while (Quaternion.Angle(transform.rotation, q) > 0f)
        {
            t += Time.deltaTime * 0.001f;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(pointList[index] - transform.position), t);
            yield return null;
        }
        transform.LookAt(point);
    }


    int CalculateDis()
    {
        float dis = 99999999999;
        int index = -1;
        //Debug.Log(IsAutoDrive);
        // 计算最近的距离
        for (int i = 0; i < pointList.Count; i++)
        {
            float myDis = Vector3.Distance(transform.position, pointList[i]);
            if (myDis < dis)
            {
                index = i;
                dis = myDis;
            }
        }
        // isRotation = true;
        isCalculate = false;
        return index;
    }

    void ManualDrive()
    {
        //Debug.Log(IsAutoDrive);
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        if (h != 0)
        {
            float anglez = -Mathf.Clamp(h * 30f, -30, 30);
            //float angley = transform.rotation.y + Time.deltaTime * h * 5;

            float rotation = h * 60 * Time.deltaTime;
            transform.rotation = Quaternion.Euler(0, rotation, anglez);
            transform.Rotate(0, rotation, 0);
            //transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y + Time.deltaTime * 5, transform.eulerAngles.z);
        }
        if (v != 0)
        {
            float anglex = Mathf.Clamp(v * 30f, -30, 30);
            transform.rotation = Quaternion.Euler(anglex, 0, 0);
        }
        // 加速
        if (Input.GetKeyDown(KeyCode.Q))
        {
            speed += 10;
        }
        // 减速
        if (Input.GetKeyDown(KeyCode.E))
        {
            speed -= 10;
        }
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }


}
