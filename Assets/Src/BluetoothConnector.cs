using System.IO.Ports;
using UnityEngine;

public class BluetoothConnector : MonoBehaviour
{
    SerialPort btPort = new SerialPort("COM22", 9600); // Укажи нужный порт

    void Start()
    {
        try
        {
            btPort.Open();
            Debug.Log("Bluetooth подключен");
        }
        catch (System.Exception e)
        {
            Debug.LogError("Ошибка подключения: " + e.Message);
        }
    }

    void Update()
    {
        if (btPort.IsOpen && btPort.BytesToRead > 0)
        {
            string data = btPort.ReadLine();
            Debug.Log("Получено: " + data);
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            print("A");
            btPort.Write("A"); // Пример: отправка символа
        }
    }

    void OnApplicationQuit()
    {
        if (btPort.IsOpen) btPort.Close();
    }
}
