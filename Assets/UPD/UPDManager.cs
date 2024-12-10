using System;
using System.Collections;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;
using Slider = UnityEngine.UI.Slider;
using TMPro;
public class UDPManager : MonoBehaviour
{
    IPEndPoint remoteEndPoint;
    UDPDATA mUDPDATA = new UDPDATA();


    private string IP;  // define in init
    public int port;  // define in init
    public TextMeshPro engineA;
    public Text engineAHex;
    public Slider sliderA;
    public TextMeshPro engineB;
    public Text engineBHex;
    public Slider sliderB;
    public TextMeshPro engineC;
    public Text engineCHex;
    public Slider sliderC;

    public Text Data;

    UdpClient client;

    public bool active = false;

    public float SmoothEngine = 0.5f;

    public float A = 0, B = 0, C = 0, longg;

    public Transform vehicle;
    public Vector2 axis;
    private Vector2 previousAxis = Vector2.zero;

    // start from unity3d
    public void Start()
    {
        init();
    }
    void Update()
    {
        // Obtenemos la rotación del vehículo
        Vector3 rotation = vehicle.transform.eulerAngles;

        // Normalizamos las rotaciones en el rango [-180, 180]
        rotation.x = NormalizeAngle(rotation.x);
        rotation.z = NormalizeAngle(rotation.z);

        // Determinamos el valor del eje basado en la rotación
        float xValue = Mathf.Clamp(rotation.x / 90f, -1f, 1f); // Escalamos y limitamos entre -1 y 1
        float zValue = Mathf.Clamp(rotation.z / 90f, -1f, 1f); // Escalamos y limitamos entre -1 y 1

        Vector2 newAxis;
        // Si predomina X, representamos en el eje X, si no, en el Z
        if (Mathf.Abs(rotation.x) > Mathf.Abs(rotation.z))
        {
            newAxis = new Vector2(Mathf.Sign(xValue), 0); // Dirección en el eje X (-1 o 1)
        }
        else
        {
            newAxis = new Vector2(0, Mathf.Sign(zValue)); // Dirección en el eje Z (-1 o 1)
        }

        // Verificamos si el cambio en el axis es significativo
        if (Vector2.Distance(newAxis, previousAxis) >= 0.15f)
        {
            axis = newAxis;
            previousAxis = newAxis;

            // Llamamos a la función CalcularRotacion
            CalcularRotacion(axis);
        }
    }
    private float NormalizeAngle(float angle)
    {
        angle %= 360;
        if (angle > 180) angle -= 360;
        return angle;
    }
    public void init()
    {

        // define
        IP = "192.168.15.201";
        port = 7408;

        // ----------------------------
        // Senden
        // ----------------------------
        remoteEndPoint = new IPEndPoint(IPAddress.Parse(IP), port);
        client = new UdpClient(53342);


        // AppControlField
        mUDPDATA.mAppControlField.ConfirmCode = "55aa";
        mUDPDATA.mAppControlField.PassCode = "0000";
        mUDPDATA.mAppControlField.FunctionCode = "1301";
        // AppWhoField
        mUDPDATA.mAppWhoField.AcceptCode = "ffffffff";
        mUDPDATA.mAppWhoField.ReplyCode = "";//"00000001";
                                             // AppDataField
        mUDPDATA.mAppDataField.RelaTime = "00000064";
        mUDPDATA.mAppDataField.PlayMotorA = "00000000";
        mUDPDATA.mAppDataField.PlayMotorB = "00000000";
        mUDPDATA.mAppDataField.PlayMotorC = "00000000";

        mUDPDATA.mAppDataField.PortOut = "12345678";

        A = 100;
        B = 100;
        C = 100;

        sliderA.value = A;
        sliderB.value = B;
        sliderC.value = C;

        string HexA = DecToHexMove(A);
        string HexB = DecToHexMove(B);
        string HexC = DecToHexMove(C);

        engineAHex.text = "Engine A: " + HexA;
        engineBHex.text = "Engine B: " + HexB;
        engineCHex.text = "Engine C: " + HexC;

        mUDPDATA.mAppDataField.PlayMotorC = HexC;
        mUDPDATA.mAppDataField.PlayMotorA = HexA;
        mUDPDATA.mAppDataField.PlayMotorB = HexB;


        engineA.text = ((int)sliderA.value).ToString();
        engineB.text = ((int)sliderB.value).ToString();
        engineC.text = ((int)sliderC.value).ToString();

        Data.text = "Data: " + mUDPDATA.GetToString();

        sendString(mUDPDATA.GetToString());

        StartCoroutine(UpMovePlatform(3));
    }
    public void ActiveSend()
    {
        active = true;

    }
    public void ResertPositionEngine()
    {

        mUDPDATA.mAppDataField.RelaTime = "00001F40";

        mUDPDATA.mAppDataField.PlayMotorA = "00000000";
        mUDPDATA.mAppDataField.PlayMotorB = "00000000";
        mUDPDATA.mAppDataField.PlayMotorC = "00000000";

        sendString(mUDPDATA.GetToString());

        mUDPDATA.mAppDataField.RelaTime = "00000064";

    }
    public void SetPositionEngine()
    {
        mUDPDATA.mAppDataField.RelaTime = "00001F40";

        string HexA = DecToHexMove(A);
        string HexB = DecToHexMove(B);
        string HexC = DecToHexMove(C);

        mUDPDATA.mAppDataField.PlayMotorC = HexC;
        mUDPDATA.mAppDataField.PlayMotorA = HexA;
        mUDPDATA.mAppDataField.PlayMotorB = HexB;

        //Data.text = "Data: " + mUDPDATA.GetToString();

        sendString(mUDPDATA.GetToString());

        mUDPDATA.mAppDataField.RelaTime = "00000064";

    }

    IEnumerator UpMovePlatform(float wait)
    {
        active = false;

        yield return new WaitForSeconds(3f);

        active = true;
    }

    void CalcularRotacion(Vector2 acsis)
    {

        if (A >= 0 && A <= 200 && B >= 0 && B <= 200 && C >= 0 && C <= 200)
        {
            if (axis == new Vector2(1, 0))
            {
                B += 1f; // Aumenta B
                C += 1f; // Aumenta C
                A -= 1f; // Disminuye A
            }
        }
    }

    void FixedUpdate()
    {

        //if (active)
        //{
        //    sliderA.value = A;
        //    sliderB.value = B;
        //    sliderC.value = C;

        //    string HexA = DecToHexMove(A);
        //    string HexB = DecToHexMove(B);
        //    string HexC = DecToHexMove(C);

        //    engineAHex.text = "Engine A: " + HexA;
        //    engineBHex.text = "Engine B: " + HexB;
        //    engineCHex.text = "Engine C: " + HexC;

        //    mUDPDATA.mAppDataField.PlayMotorC = HexC;
        //    mUDPDATA.mAppDataField.PlayMotorA = HexA;
        //    mUDPDATA.mAppDataField.PlayMotorB = HexB;


        //    engineA.text = ((int)A).ToString();
        //    engineB.text = ((int)B).ToString();
        //    engineC.text = ((int)C).ToString();

        //    Data.text = "Data: " + mUDPDATA.GetToString();

        //    //sendString(mUDPDATA.GetToString());
        //}
        //CalcularRotacion();
        //engineA.text = ((int)A).ToString();
        //engineB.text = ((int)B).ToString();
        //engineC.text = ((int)C).ToString();
        //SetPositionEngine();
    }

    void OnApplicationQuit()
    {

        ResertPositionEngine();



        if (client != null)
            client.Close();
        Application.Quit();
    }

    byte[] StringToByteArray(string hex)
    {
        return Enumerable.Range(0, hex.Length)
                         .Where(x => x % 2 == 0)
                         .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                         .ToArray();
    }

    string DecToHexMove(float num)
    {
        int d = (int)((num / 5f) * 10000f);
        return "000" + d.ToString("X");
    }

    private void sendString(string message)
    {

        try
        {
            // Bytes empfangen.
            if (message != "")
            {

                //byte[] data = StringToByteArray(message);
                print(message);
                // Den message zum Remote-Client senden.
                //client.Send(data, data.Length, remoteEndPoint);

            }


        }
        catch (Exception err)
        {
            print(err.ToString());
        }
    }

    void OnDisable()
    {

        if (client != null)
            client.Close();
    }

    private void OnDrawGizmos()
    {
        //#region Axis WordSpace
        //Gizmos.color = Color.blue;
        //Gizmos.DrawSphere(Vector3.forward * longg, 0.5f);
        //Gizmos.DrawLine(Vector3.zero, Vector3.forward * longg);

        //Gizmos.color = Color.red;
        //Gizmos.DrawSphere(Vector3.right * longg, 0.5f);
        //Gizmos.DrawLine(Vector3.zero, Vector3.right * longg);

        //Gizmos.color = Color.green;
        //Gizmos.DrawSphere(Vector3.up * longg, 0.5f);
        //Gizmos.DrawLine(Vector3.zero, Vector3.up * longg);
        //#endregion

        #region Axis Vechicle
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(vehicle.position + vehicle.forward * longg, 0.5f);
        Gizmos.DrawLine(vehicle.position, vehicle.position + vehicle.forward * longg);

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(vehicle.position + vehicle.right * longg, 0.5f);
        Gizmos.DrawLine(vehicle.position, vehicle.position + vehicle.right * longg);

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(vehicle.position + vehicle.up * longg, 0.5f);
        Gizmos.DrawLine(vehicle.position, vehicle.position + vehicle.up * longg);
        #endregion
    }
}