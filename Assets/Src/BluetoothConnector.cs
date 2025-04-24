using System.IO.Ports;
using UnityEngine;

public class BluetoothConnector : MonoBehaviour
{
    SerialPort btPort = new SerialPort("COM22", 9600); // ����� ������ ����

    void Start()
    {
        try
        {
            btPort.Open();
            Debug.Log("Bluetooth ���������");
        }
        catch (System.Exception e)
        {
            Debug.LogError("������ �����������: " + e.Message);
        }
    }

    void Update()
    {
        if (btPort.IsOpen && btPort.BytesToRead > 0)
        {
            string data = btPort.ReadLine();
            Debug.Log("��������: " + data);
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            print("A");
            btPort.Write("A"); // ������: �������� �������
        }
    }

    void OnApplicationQuit()
    {
        if (btPort.IsOpen) btPort.Close();
    }
}
