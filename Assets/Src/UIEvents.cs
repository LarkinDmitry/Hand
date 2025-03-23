using System;
using UnityEngine;
using UnityEngine.UI;

public class UIEvents : MonoBehaviour
{
    [SerializeField] Button reset;
    [SerializeField] Toggle filtration;
    [SerializeField] Toggle showGrabZones;
    [SerializeField] Toggle showBones;

    public static event Action OnPressReset;
    public static event Action<bool> OnSwitchFiltrationState;
    public static event Action<bool> OnSwitchShowGrabZoneState;
    public static event Action<bool> OnSwitchShowBonesState;

    public static bool FiltrationState { get; private set; }
    public static bool ShowGrabZoneState { get; private set; }
    public static bool ShowBonesState { get; private set; }

    private void Awake()
    {
        reset.onClick.AddListener(() => OnPressReset?.Invoke());

        filtration.onValueChanged.AddListener((b) => OnSwitchFiltrationState?.Invoke(b));
        showGrabZones.onValueChanged.AddListener((b) => OnSwitchShowGrabZoneState?.Invoke(b));
        showBones.onValueChanged.AddListener((b) => OnSwitchShowBonesState?.Invoke(b));

        filtration.onValueChanged.AddListener((b) => FiltrationState = b);
        showGrabZones.onValueChanged.AddListener((b) => ShowGrabZoneState = b);
        showBones.onValueChanged.AddListener((b) => ShowBonesState = b);

        FiltrationState = filtration.isOn;
        ShowGrabZoneState = showGrabZones.isOn;
        ShowBonesState = showBones.isOn;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            OnPressReset?.Invoke();
        }
    }
}