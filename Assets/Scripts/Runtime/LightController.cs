using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightController : MonoBehaviour
{
    private Light[] _mainLights;
    private Light[] _emergencyLights;

    public void SetMainLightLevel(float level)
    {
        GameObject[] lights = GameObject.FindGameObjectsWithTag("MainLight");

        foreach (GameObject light in lights)
        {
            light.GetComponent<Light>().intensity = level;
        }
    }

    public void SetEmergencyLightLevel(float level)
    {
        GameObject[] lights = GameObject.FindGameObjectsWithTag("EmergencyLight");

        foreach (GameObject light in lights)
        {
            light.GetComponent<Light>().intensity = level;
        }
    }
}
