using System;
using System.Collections;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;
using Slider = UnityEngine.UI.Slider;
using TMPro;
using Unity.VisualScripting;
public class UDPManager : MonoBehaviour
{
    IPEndPoint remoteEndPoint;
    UDPDATA mUDPDATA = new UDPDATA();

    private string IP;  // define in init
    public int port;  // define in init
    public TextMeshPro engineA;
    public TextMeshProUGUI engineAHex;
    public Slider sliderA;
    public TextMeshPro engineB;
    public TextMeshProUGUI engineBHex;
    public Slider sliderB;
    public TextMeshPro engineC;
    public TextMeshProUGUI engineCHex;
    public Slider sliderC;

    public TextMeshProUGUI Data;

    UdpClient client;

    public bool active = false;

    public float SmoothEngine = 0.5f;

    public float A = 0, B = 0, C = 0, longg;

    public Transform vehicle;

    // start from unity3d
    public void Start()
    {
        init();
    }
    void FixedUpdate()
    {
        if (active)
        {
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

            engineA.text = ((int)A).ToString();
            engineB.text = ((int)B).ToString();
            engineC.text = ((int)C).ToString();

            Data.text = "Data: " + mUDPDATA.GetToString();

            sendString(mUDPDATA.GetToString());
        }
        CalcularRotacion();
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
    public void ResertPositionEngine()
    {

        mUDPDATA.mAppDataField.RelaTime = "00001F40";

        mUDPDATA.mAppDataField.PlayMotorA = "00000000";
        mUDPDATA.mAppDataField.PlayMotorB = "00000000";
        mUDPDATA.mAppDataField.PlayMotorC = "00000000";

        sendString(mUDPDATA.GetToString());

        mUDPDATA.mAppDataField.RelaTime = "00000064";

    }
    //private void CalcularRotacion()
    //{
    //    // Obtenemos la rotación del vehículo
    //    Vector3 rotation = vehicle.transform.eulerAngles;

    //    // Normalizamos las rotaciones en el rango [-180, 180]
    //    rotation.x = NormalizeAngle(rotation.x);
    //    rotation.z = NormalizeAngle(rotation.z);

    //    // A está determinado por la rotación en X
    //    A = Mathf.Clamp(100 + rotation.x, 0, 200);

    //    // B y C están determinados por la rotación en Z
    //    B = Mathf.Clamp(100 - rotation.z, 0, 200);
    //    C = Mathf.Clamp(100 + rotation.z, 0, 200);
    //}
    private void CalcularRotacion()
    {
        // Obtenemos la rotación del vehículo
        Vector3 rotation = vehicle.transform.eulerAngles;

        // Normalizamos las rotaciones en el rango [-180, 180]
        rotation.x = NormalizeAngle(rotation.x);
        rotation.z = NormalizeAngle(rotation.z);

        // A está determinado por la rotación en X
        A = Mathf.Clamp(100 + rotation.x, 0, 200);

        // B y C están determinados por la rotación en Z
        B = Mathf.Clamp(100 - rotation.z, 0, 200);
        C = Mathf.Clamp(100 + rotation.z, 0, 200);

        // Ajustamos B y C según el valor de A
        float diffA = A - 100;  // Calculamos la diferencia de A respecto al valor central 100
        B = Mathf.Clamp(B - diffA, 0, 200);  // Restamos la diferencia de A a B
        C = Mathf.Clamp(C - diffA, 0, 200);  // Restamos la diferencia de A a C
    }
    public void ActiveSend()
    {
        active = true;
    }
    private float NormalizeAngle(float angle)
    {
        angle %= 360;
        if (angle > 180) angle -= 360;
        return angle;
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
    void OnApplicationQuit()
    {

        ResertPositionEngine();



        if (client != null)
            client.Close();
        Application.Quit();
    }
    void OnDisable()
    {

        if (client != null)
            client.Close();
    }
    IEnumerator UpMovePlatform(float wait)
    {
        active = false;

        yield return new WaitForSeconds(3f);

        active = true;
    }
    private void OnDrawGizmos()
    {
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