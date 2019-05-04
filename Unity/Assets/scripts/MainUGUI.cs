using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainUGUI : Main
{
    [Header("UI")]
    public Slider volumeSlider;
    public Slider pitchSlider;
    public Slider throttleSlider;
    public Image fillImage;
    public TMPro.TextMeshProUGUI rpmText;
    public Animator animator;

    private IEnumerator Start()
    {
        base.Initialize();

        pitchSlider.minValue = 0.8f;
        pitchSlider.maxValue = 1.2f;
        pitchSlider.value = 1f;

        throttleSlider.minValue = -0.3f;
        throttleSlider.maxValue = 0.7f;

        volumeSlider.value = 0.8f;

        // Add handlers after setting initial values
        pitchSlider.onValueChanged.AddListener(PitchSlider_OnValueChanged);
        volumeSlider.onValueChanged.AddListener(VolumeSlider_OnValueChanged);
        throttleSlider.onValueChanged.AddListener(ThrottleSlider_OnValueChanged);

        // Wait animation to end
        float animationDuration = this.animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(animationDuration + 0.5f);

        // Unlock elements
        animator.enabled = false;

        // Start Application/Engine
        base.StartEngine();
    }

    protected new void Update()
    {
        if (!base.isEngineRunning) return;

        base.Update();

        // There is an offset visually, so add offset to roughly match
        float visualRpm = Retarget(base.GetRPM(), 0f, 1f, 0.15f, 0.85f);
        fillImage.fillAmount = visualRpm;

        // Remap rpm (0-1) to (1000-8000)
        float textRpm = Retarget(base.GetRPM(), 0f, 1f, 1000f, 8000f);
        rpmText.text = textRpm.ToString("0000");
    }
    
    #region EventHandlers

    private void VolumeSlider_OnValueChanged(float arg0)
    {
        base.SetVolume(arg0);
    }

    private void PitchSlider_OnValueChanged(float arg0)
    {
        base.SetPitch(arg0);
    }

    private void ThrottleSlider_OnValueChanged(float arg0)
    {
        // Break slider values into 2 chunks. 30% for brake. 70% for throttle
        float throttle = 0;
        if (arg0 >= 0f)
        {
            throttle = Retarget(arg0, 0f, 0.7f, 0f, 1f);
        }
        float breaking = 0f;
        if (arg0 < 0f)
        {
            arg0 *= -1f;
            breaking = Retarget(arg0, 0f, 0.3f, 0f, 1f);
        }
        base.SetThrottle(throttle, breaking);
    }

    #endregion


    public static float Retarget(float value, float currentMin, float currentMax, float targetMin, float targetMax, bool clamp = true)
    {
        float temp = value;

        // Normalize
        temp -= currentMin;
        temp /= (currentMax - currentMin);

        // if (clamp)
        //    temp = Mathf.Clamp(temp, 0, 1);

        // Retarget
        temp *= (targetMax - targetMin);
        temp += targetMin;

        if (clamp)
            temp = Mathf.Clamp(temp, targetMin, targetMax);

        return temp;
    }
}
