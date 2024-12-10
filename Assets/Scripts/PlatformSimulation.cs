//using System.Net.Http.Headers;
//using UnityEngine;

//public class PlatformSimulator : MonoBehaviour
//{
//    public Transform MotorA;
//    public Transform MotorB;
//    public Transform MotorC;
//    public Transform vehicle;
//    public float limit = 20f;

//    void Update()
//    {
//        Vector3 vehicleRotation = vehicle.localEulerAngles;

//        ControlMotor(MotorA, vehicleRotation.x);
//        ControlMotor(MotorB, vehicleRotation.y);
//        ControlMotor(MotorC, vehicleRotation.z);
//    }

//    void ControlMotor(Transform motor, float rotationValue)
//    {
//        //float clampedRotation = Mathf.Clamp(rotationValue, -limit, limit);

//        //motor.localRotation = Quaternion.Euler(clampedRotation, 0f, 0f);

//        // Clampear el valor de rotación dentro del límite definido
//        float clampedRotation = Mathf.Clamp(rotationValue, -limit, limit);

//        // Obtener la rotación actual del motor y ajustarla solo en el eje Z
//        Vector3 currentRotation = motor.localEulerAngles;

//        currentRotation.z = clampedRotation;

//        // Asignar la nueva rotación al motor
//        motor.localEulerAngles = currentRotation;
//    }
//}
using UnityEngine;

public class PlatformSimulator : MonoBehaviour
{
    public Transform MotorA;
    public Transform MotorB;
    public Transform MotorC;
    public Transform vehicle;
    public float limit = 20f;

    void Update()
    {
        Vector3 vehicleRotation = vehicle.localEulerAngles;

        // Sincronizar MotorA
        ControlMotorA(MotorA, vehicleRotation.x);

        // Sincronizar MotorB
        ControlMotorB(MotorB, vehicleRotation.y);

        // Sincronizar MotorC
        ControlMotorC(MotorC, vehicleRotation.z);
    }

    void ControlMotorA(Transform motorA, float rotationValue)
    {
        // Clampear la rotación para MotorA
        float clampedRotationA = Mathf.Clamp(rotationValue, -limit, limit);
        motorA.localEulerAngles = new Vector3(0f, 0f, clampedRotationA);

        // Sincronizar inversamente los otros dos motores
        float inverseRotation = -clampedRotationA;
        MotorB.localEulerAngles = new Vector3(0f, 0f, inverseRotation);
        MotorC.localEulerAngles = new Vector3(0f, 0f, inverseRotation);
    }

    void ControlMotorB(Transform motorB, float rotationValue)
    {
        // Clampear la rotación para MotorB
        float clampedRotationB = Mathf.Clamp(rotationValue, -limit, limit);
        motorB.localEulerAngles = new Vector3(0f, 0f, clampedRotationB);

        //// Sincronizar inversamente MotorC
        //float inverseRotationC = -clampedRotationB;
        //MotorC.localEulerAngles = new Vector3(0f, 0f, inverseRotationC);
    }

    void ControlMotorC(Transform motorC, float rotationValue)
    {
        // Clampear la rotación para MotorC
        float clampedRotationC = Mathf.Clamp(rotationValue, -limit, limit);
        motorC.localEulerAngles = new Vector3(0f, 0f, clampedRotationC);

        //// Sincronizar inversamente MotorC
        float inverseRotationB = -clampedRotationC;
        MotorC.localEulerAngles = new Vector3(0f, 0f, inverseRotationB);

        // NOTA: MotorC no afecta a los demás motores.
    }
}
