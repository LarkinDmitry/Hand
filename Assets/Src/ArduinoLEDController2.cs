using UnityEngine;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;
using System.Collections;
using System;

public class ArduinoLEDController2 : MonoBehaviour
{
    private SerialPort arduinoPort;
    private bool ledState = false;
    private string detectedPort;
    private bool isSearching = false;
    private CancellationTokenSource cancellationTokenSource;

    void Start()
    {
        StartArduinoSearch();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            ToggleLED();
        }
    }

    void StartArduinoSearch()
    {
        if (isSearching) return;
        isSearching = true;

        cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;

        // Запускаем поиск в отдельном потоке
        Task.Run(() =>
        {
            try
            {
                Task searchTask = Task.Factory.StartNew
                (() => FindArduinoPort(cancellationToken), cancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Default);

                searchTask.Wait(cancellationToken);

                if (!cancellationToken.IsCancellationRequested && !string.IsNullOrEmpty(detectedPort))
                {
                    // Инициализация порта должна быть в главном потоке
                    UnityMainThreadDispatcher.Instance().Enqueue(() => InitializeArduinoConnection());
                }
            }
            finally
            {
                isSearching = false;
            }
        }, cancellationToken);
    }

    void FindArduinoPort(CancellationToken ct)
    {
        string[] ports = SerialPort.GetPortNames();
        Debug.Log($"Searching Arduino in {ports.Length} ports...");

        foreach (var port in ports)
        {
            if (ct.IsCancellationRequested)
            {
                return;
            }

            Debug.Log($"Checking port {port}...");
            using SerialPort sp = new(port, 9600);
            try
            {
                sp.Open();
                sp.ReadTimeout = 500;
                sp.WriteTimeout = 500;
                sp.DtrEnable = true;
                sp.RtsEnable = true;

                Thread.Sleep(2000); // Даем время на инициализацию

                sp.DiscardInBuffer();
                sp.DiscardOutBuffer();
                sp.WriteLine("?");

                var timeout = 1000;
                while (timeout > 0 && sp.BytesToRead == 0)
                {
                    Thread.Sleep(100);
                    timeout -= 100;
                    if (ct.IsCancellationRequested) return;
                }

                if (sp.BytesToRead > 0)
                {
                    string response = sp.ReadLine().Trim();
                    Debug.Log($"Port {port} response: {response}");

                    if (response == "1" || response == "0" || response.Contains("LED") || response.Contains("Arduino"))
                    {
                        detectedPort = port;
                        return;
                    }
                }
                else
                {
                    //Debug.Log($"Port {port} response: {response}");
                }
            }
            catch(Exception e)
            {
                Debug.Log(e);
                // Пропускаем ошибки
            }
            finally
            {
                if (sp.IsOpen) sp.Close();
            }
        }
    }

    void InitializeArduinoConnection()
    {
        arduinoPort = new SerialPort(detectedPort, 9600)
        {
            ReadTimeout = 500,
            WriteTimeout = 500
        };

        try
        {
            arduinoPort.Open();
            Debug.Log($"Connected to Arduino on {detectedPort}");
            StartCoroutine(ReadDataCoroutine());
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Connection failed: {e.Message}");
        }
    }

    IEnumerator ReadDataCoroutine()
    {
        while (arduinoPort != null && arduinoPort.IsOpen)
        {
            try
            {
                if (arduinoPort.BytesToRead > 0)
                {
                    var data = arduinoPort.ReadLine();
                    Debug.Log($"Arduino: {data}");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"Read error: {e.Message}");
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    public void ToggleLED()
    {
        if (arduinoPort == null || !arduinoPort.IsOpen)
        {
            Debug.LogWarning("Arduino not connected!");
            StartArduinoSearch();
            return;
        }

        Task.Run(() =>
        {
            try
            {
                ledState = !ledState;
                var command = ledState ? "1" : "0";
                arduinoPort.WriteLine(command);
                Debug.Log($"Command sent: {command}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Send failed: {e.Message}");
            }
        });
    }

    void OnDestroy()
    {
        cancellationTokenSource?.Cancel();

        if (arduinoPort != null && arduinoPort.IsOpen)
        {
            try
            {
                arduinoPort.WriteLine("0");
                arduinoPort.Close();
            }
            catch { /* Игнорируем ошибки закрытия */ }
        }
    }
}

// Помощник для выполнения действий в главном потоке
public class UnityMainThreadDispatcher : MonoBehaviour
{
    private static UnityMainThreadDispatcher instance;

    public static UnityMainThreadDispatcher Instance()
    {
        if (instance == null)
        {
            var go = new GameObject("MainThreadDispatcher");
            instance = go.AddComponent<UnityMainThreadDispatcher>();
            DontDestroyOnLoad(go);
        }
        return instance;
    }

    private readonly System.Collections.Concurrent.ConcurrentQueue<System.Action>
        actions = new System.Collections.Concurrent.ConcurrentQueue<System.Action>();

    void Update()
    {
        while (actions.TryDequeue(out var action))
        {
            action?.Invoke();
        }
    }

    public void Enqueue(System.Action action)
    {
        actions.Enqueue(action);
    }
}