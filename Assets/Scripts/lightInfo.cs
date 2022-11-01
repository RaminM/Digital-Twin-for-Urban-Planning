using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lightInfo : MonoBehaviour
{
    public string lightName,lat,lon,rotation,lightIntensity;
    public GameObject light;
    void Start(){
        lightIntensity = light.GetComponent<Light>().intensity.ToString();
    }


}
