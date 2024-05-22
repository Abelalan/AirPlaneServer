using AirPlaneData;
using Google.Protobuf;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody))]
public class PlayerControl : MonoBehaviour
{
    // �ƶ��ٶ�
    public float MoveSpeed = 60;
    // ˮƽ�ٶ�
    public float HorizontalSpeed = 30;
    // ��ֱ�ٶ�
    public float VerticalSpeed = 15;
    // ��б�Ƕ�X
    public float LeanAmountX = 30;
    // ��б�Ƕ�Y
    public float LeanAmountY = 30;
    // ƽ��ת��
    public float SteeringSmoothing = 1.5f;
    // ƽ���ƽ�
    public float ThrustSmoothing;
    // ģ�͵�Transform
    public Transform model;

    // ƽ��ת��
    private Vector3 smoothInputSteering;
    // ��ʻ����
    private Vector3 steeringInput;
    // δ�����ת������
    private Vector3 rawInputSteering;
    // �ƽ�����
    private float thrustInput;
    // δ������ƽ�����
    private float rawInputThrust;
    // ƽ�����ƽ�����
    private float smoothInputThrust;
    // ����ĸ���
    private Rigidbody rb;
    private bool IsAutoDrive = true;
    private bool isCalculate = true;
    private Coroutine coroutine;
    private int index;
    private AirPlaneMoveData data = new AirPlaneMoveData();
    private List<Vector3> pointList = new List<Vector3>();
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        model = GetComponent<Transform>();
    }


    private void OnEnable()
    {
        MassageCenter.GetInstance().AddListener<bool>(MassageID.AutoDrive, (obj) => { IsAutoDrive = obj;});
        MassageCenter.GetInstance().AddListener<bool>(MassageID.ManualDrive, (obj) => { IsAutoDrive = obj; });

        InputManager.GetInstance().OnInputSpace += InputSpaceHandle;
        InputManager.GetInstance().OnInputHorizontalOrVertical += InputHorizontalOrVerticalHandler;
    }

    private void Start()
    {
        for (int i = 0; i < 6; i++)
        {
            Vector2 point = UnityEngine.Random.insideUnitCircle * 500;
            pointList.Add(new Vector3(point.x, 70, point.y));
        }
    }


    private void Update()
    {

    }

    private void FixedUpdate()
    {
        InputSmoothing();

        Move();
        if (!IsAutoDrive)
        {
            
            Turn();
        }
        else
        {
            AutoDirve();
        }

        for (int i = 0; i < OnLineManager.GetInstance().OnLineUser.Count; i++)
        {
            data.Px = transform.position.x;
            data.Py = transform.position.y;
            data.Pz = transform.position.z;
            data.Speed = MoveSpeed;
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

        // �������
        if (isCalculate)
        {
            index = CalculateDis();
        }
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
        float dotsing = Mathf.Sign(Vector3.Dot(pointList[index], transform.forward));
        while (Quaternion.Angle(transform.rotation, q) > 0f)
        {
            t += Time.deltaTime;
            //transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(pointList[index] - transform.position), t);
            if (t >= 1)
            {
                t = 1;
            }
            rawInputSteering = new Vector3(0, 0, -1 * dotsing * t);
            yield return null;
        }
        transform.LookAt(point);
    }


    int CalculateDis()
    {
        float dis = 99999999999;
        int index = -1;
        //Debug.Log(IsAutoDrive);
        // ��������ľ���
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
    private void OnDisable()
    {
        InputManager.GetInstance().OnInputSpace -= InputSpaceHandle;
        InputManager.GetInstance().OnInputHorizontalOrVertical -= InputHorizontalOrVerticalHandler;
    }

    private void OnDestroy()
    {
        InputManager.GetInstance().OnInputSpace -= InputSpaceHandle;
        InputManager.GetInstance().OnInputHorizontalOrVertical -= InputHorizontalOrVerticalHandler;
    }

    private void InputHorizontalOrVerticalHandler(float arg1, float arg2)
    {
        // input.GetAxix("Horizontal")    input.GetAxix("Vertical")
        Vector2 rawInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        rawInputSteering = new Vector3(rawInput.y, 0, -rawInput.x);
    }
    /// <summary>
    /// �ƽ�����
    /// </summary>
    /// <param name="space"></param>
    private void InputSpaceHandle(float space)
    {
        rawInputThrust = space;
    }

    private void InputSmoothing()
    {
        // ƽ��ת������ ת������  δ�����ת�����룬
        smoothInputSteering = Vector3.Lerp(smoothInputSteering, rawInputSteering, Time.deltaTime * SteeringSmoothing);

        steeringInput = smoothInputSteering;

        smoothInputThrust = Mathf.Lerp(smoothInputThrust, rawInputThrust, Time.deltaTime * ThrustSmoothing);
        thrustInput = smoothInputThrust;
    }

    private void Move()
    {
        // ����
        if (Input.GetKeyDown(KeyCode.Q))
        {
            MoveSpeed += 10;
        }
        // ����
        if (Input.GetKeyDown(KeyCode.E))
        {
            MoveSpeed -= 10;
        }
        // ���ø����ٶ�
        rb.velocity = transform.forward * MoveSpeed;
    }

    private void Turn()
    {
        // ����Ť��
        Vector3 newTorqus = new Vector3(steeringInput.x * HorizontalSpeed, -steeringInput.z * VerticalSpeed, 0);
        rb.AddRelativeTorque(newTorqus);
        rb.rotation = Quaternion.Slerp(rb.rotation, Quaternion.Euler(new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, 0)), 0.5f);
        TurnModel();
    }

    private void TurnModel()
    {
        model.localEulerAngles = new Vector3(steeringInput.x * LeanAmountY, model.localEulerAngles.y, steeringInput.z * LeanAmountX);
    }
}
