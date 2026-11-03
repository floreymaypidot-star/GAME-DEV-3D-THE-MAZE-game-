// - ElectricTorchOnOff - Script by Marcelli Michele

// This script is attached in primary model (default) of the Electric Torch.
// You can On/Off the light and choose any letter on the keyboard to control it
// Use the "battery" or no and the duration time
// Change the intensity of the light

using UnityEngine;

public class ElectricTorchOnOff : MonoBehaviour
{
    public enum LightChoose { noBattery, withBattery }
    public LightChoose modoLightChoose = LightChoose.withBattery;

    [Space]
    public string onOffLightKey = "F";
    private KeyCode _kCode = KeyCode.F;

    [Space]
    public bool _PowerPickUp = false;     // hindi ginagamit, iniwan para hindi masira ang BatteryPowerPickup.cs

    [Space]
    public float intensityLight = 2.5f;   // kasalukuyang charge
    public float maxIntensity = 2.5f;
    public float drainPerSecond = 0.02f;  // 2.5 / 0.02 = mga 125 segundo bawat buong charge

    private float _drainMultiplier = 1f;
    private bool _flashLightOn = true;
    private Light _light;
    private EmissionMaterialGlassTorchFadeOut _emissionMaterialFade;

    public float Charge01 => maxIntensity > 0f ? intensityLight / maxIntensity : 0f;

    void Awake()
    {
        _light = GetComponent<Light>();
    }

    void Start()
    {
        GameObject model = GameObject.Find("default");
        if (model != null)
            _emissionMaterialFade = model.GetComponent<EmissionMaterialGlassTorchFadeOut>();

        if (!System.Enum.TryParse(onOffLightKey, true, out _kCode))
            _kCode = KeyCode.F;
    }

    void Update()
    {
        bool batteryMode = modoLightChoose == LightChoose.withBattery;

        if (Input.GetKeyDown(_kCode) && (!batteryMode || intensityLight > 0f))
            _flashLightOn = !_flashLightOn;

        if (batteryMode && _flashLightOn)
            intensityLight = Mathf.Max(0f, intensityLight - drainPerSecond * _drainMultiplier * Time.deltaTime);

        bool lit = _flashLightOn && (!batteryMode || intensityLight > 0f);
        _light.intensity = lit ? maxIntensity * Mathf.Clamp01(Charge01 / 0.25f) : 0f;

        if (_emissionMaterialFade != null)
        {
            if (lit) _emissionMaterialFade.OnEmission();
            else _emissionMaterialFade.OffEmission();
        }
    }

    public void Recharge(float amount)
    {
        intensityLight = Mathf.Min(maxIntensity, intensityLight + amount);
    }

    // Para sa Tiktik mamaya: mas mabilis ang drain kapag malapit siya
    public void SetDrainMultiplier(float m) { _drainMultiplier = m; }
}