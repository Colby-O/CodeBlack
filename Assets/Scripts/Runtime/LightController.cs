using CodeBlack;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightController : MonoBehaviour
{
    private List<Light> _mainLights;

    private PatientManager _pm;

    public void SetMainLightLevel(float level)
    {
        foreach (Light light in _mainLights)
        {
            light.GetComponent<Light>().intensity = level;
        }
    }

    public void SetMainLightColorf(Color col)
    {
        foreach (Light light in _mainLights)
        {
            light.GetComponent<Light>().color = col;
        }
    }

    private void Awake()
    {
        _mainLights = new List<Light>();
        GameObject[] objs = GameObject.FindGameObjectsWithTag("MainLight");
        foreach (GameObject obj in objs)
        {
            _mainLights.Add(obj.GetComponent<Light>());
        }
        SetMainLightColorf(Color.white);

        _pm = FindAnyObjectByType<PatientManager>();
    }

    private void Update()
    {
        if (_pm.AnyPatientsFlatLine())
        {
            SetMainLightColorf(Color.red);
        }
        else
        {
            SetMainLightColorf(Color.white);
        }
    }

}
