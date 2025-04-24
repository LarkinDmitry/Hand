using System.Collections;
using UnityEngine;
using System.IO.Ports;
using System;

public class VibrationFeedbackController : MonoBehaviour
{
    public static VibrationFeedbackController Instance;

    [Range(0, 100)] public int impactForce;
    [Range(0, 0.1f)] public float impactDuration;
    [Range(0, 100)] public int holdingForce;
    [Range(0, 0.5f)] public float impactTrashhold;

    private SerialPort btPort = new ("COM22", 115200);
    private bool ledState = false;
    private string detectedPort;
    private bool portFound = false;

    private char[] command = {'D', 'V', '!', '!', '!', '!', '!', '!' }; // '!' = 0 .... '`' = 100 смещение значений char на "charOffset" (нужно для сокращения команды)
    private const int charOffset = 33;
    private const int commandPrefixLength = 2;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        btPort.Open();

        InvokeRepeating(nameof(UpdVibroState), 0, 0.01f);
    }

    private void UpdVibroState()
    {

        if (btPort != null && btPort.IsOpen)
        {
            string s = new string(command);
            btPort.WriteLine(s);
        }
    }

    IEnumerator DetectArduinoCoroutine()
    {
        string[] ports = SerialPort.GetPortNames();
        Debug.Log("Searching for Arduino...");

        if (ports.Length == 0)
        {
            Debug.LogError("No COM ports found!");
            yield break;
        }

        foreach (string port in ports)
        {
            Debug.Log("Checking port: " + port);

            SerialPort sp = new SerialPort(port, 9600);
            try
            {
                sp.Open();
                sp.ReadTimeout = 500;
                sp.WriteTimeout = 500;
                sp.DtrEnable = true;
                sp.RtsEnable = true;


                // Очищаем буфер
                sp.BaseStream.Flush();
                sp.DiscardInBuffer();
                sp.DiscardOutBuffer();

                // Отправляем запрос
                sp.WriteLine("STATUS");
            }
            catch (System.Exception e)
            {
                Debug.LogWarning("Port " + port + " error: " + e.Message);
                if (sp.IsOpen) sp.Close();
                continue;
            }

            yield return new WaitForSeconds(0.1f);

            try
            {
                // Ждем ответа
                float timeout = Time.time + 0.1f;
                while (Time.time < timeout && sp.BytesToRead == 0)
                {
                    //yield return null;
                }

                if (sp.BytesToRead > 0)
                {
                    string response = sp.ReadLine().Trim();
                    Debug.Log("Response from " + port + ": " + response);

                    if (response.Contains("READY"))
                    {
                        detectedPort = port;
                        portFound = true;
                        sp.Close();
                        break;
                    }
                }

                sp.Close();
            }
            catch (System.Exception e)
            {
                Debug.LogWarning("Port " + port + " error: " + e.Message);
                if (sp.IsOpen) sp.Close();
            }

            yield return null;
        }

        if (portFound)
        {
            Debug.Log("Arduino found on port: " + detectedPort);
            PlayerPrefs.SetString("port", detectedPort);
            InitializeArduinoConnection();
        }
        else
        {
            PlayerPrefs.SetString("port", string.Empty);
            Debug.LogError("Arduino not found on any port!");
        }
    }

    void InitializeArduinoConnection()
    {
        btPort = new(detectedPort, 9600);
        btPort.ReadTimeout = 500;
        btPort.WriteTimeout = 500;

        try
        {
            btPort.Open();
            Debug.Log("Successfully connected to Arduino");
        }
        catch (System.Exception e)
        {
            Debug.LogError("Connection error: " + e.Message);
            StartCoroutine(DetectArduinoCoroutine());
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            if (btPort != null && btPort.IsOpen)
            {
                ledState = !ledState;
                string command = ledState ? "ON" : "OFF";
                btPort.WriteLine(command);
            }
            else
            {
                Debug.LogWarning("Arduino not connected!");
            }
        }
    }

    public void SetVibrationPercent(BodySide side, HandPart part, int value)
    {
        if (!btPort.IsOpen)
        {
            return;
        }

        value = Mathf.Clamp(value, 0, 100);

        if (side == BodySide.Right)
        {
            int handPart = part switch
            {
                HandPart.Palm => 0,
                HandPart.Thumb => 1,
                HandPart.Index => 2,
                HandPart.Middle => 3,
                HandPart.Ring => 4,
                HandPart.Pinkie => 5,
                _ => throw new Exception("Длина имени меньше 2 символов")
            };

            command[commandPrefixLength + handPart] = (char)(value + charOffset);
        }
    }

    void OnApplicationQuit()
    {
        if (btPort != null && btPort.IsOpen)
        {
            btPort.WriteLine("OFF");
            btPort.Close();
        }
    }
}