using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeHeatmap : MonoBehaviour
{
    GameObject _sun, _spawnManager;

    private List<GameObject> spawnedLights;


    float lightIntensity, dailyIntensity, dailyAve, ratio = 0.7f, maxDist = 20f, minAngle = 30f;

    int count = 0;
    private bool live = true, daily = false, dayEnd = false;




    // Start is called before the first frame update
    void Start()
    {
        _sun = GameObject.Find("Sun");
        _spawnManager = GameObject.Find("SpawnManager");

    }

    // Update is called once per frame
    void Update()
    {
        lightIntensity = 0;
        Vector3 pos = this.transform.position;
        Vector3 dir = (_sun.transform.position - this.transform.position);
        // Debug.DrawLine (pos, _sun.transform.position, Color.red, Mathf.Infinity);

        Ray ray = new Ray(pos, dir);
        float sunangle = _sun.GetComponent<SunPosition>().GetZenithAngle();
        RaycastHit hitData;

        if (Physics.Raycast(ray, out hitData, Mathf.Infinity))
        {
            if (hitData.transform.CompareTag("Sun"))
            {
                if (sunangle >= 0f && sunangle <= 90)
                {
                    lightIntensity += ratio * Mathf.Cos(Mathf.Deg2Rad * sunangle);

                }
            }
        }


        spawnedLights = _spawnManager.GetComponent<SpawnManager>().GetLightList();
        foreach (var light in spawnedLights)
        {
            Vector3 pos2 = this.transform.position;
            Vector3 dir2 = (new Vector3(light.transform.position.x, light.transform.position.y + 3.449f, light.transform.position.z + 0.876f) - this.transform.position);
            Ray ray2 = new Ray(pos2, dir2);
            float lightangle = Mathf.Abs(Vector3.SignedAngle(pos2, pos2 + dir2 * 10, Vector3.right));
            // Debug.DrawLine (pos2, new Vector3(light.transform.position.x, light.transform.position.y + 3.449f, light.transform.position.z + 0.876f), Color.red, Mathf.Infinity);

            if (Physics.Raycast(ray2, out hitData, Mathf.Infinity))
            {
                if (hitData.transform.CompareTag("light"))
                {
                    float dist = hitData.distance;
                    if (dist < maxDist)
                    {
                        if (lightangle >= minAngle && lightangle <= 90)
                        {
                            lightIntensity += (1 - ratio) * Mathf.Cos(Mathf.Deg2Rad * lightangle);

                        }
                    }
                }
            }

        }
        if (lightIntensity != 0)
            count++;
        if (live)
        {
            this.gameObject.GetComponent<Renderer>().material.color = new Color(lightIntensity, 1 - lightIntensity, 0);
        }
        else if (daily)
        {
            dailyIntensity += lightIntensity;
            if (count != 0)
                dailyAve = dailyIntensity / count;
            this.gameObject.GetComponent<Renderer>().material.color = new Color(dailyAve, 1 - dailyAve, 0);

        }
        if (dayEnd)
        {
            dailyIntensity = 0;
            count = 0;
            dayEnd = false;
        }


    }
    public void ShowDaily()
    {
        daily = true;
        live = false;
    }
    public void ShowLive()
    {
        daily = false;
        live = true;
    }
    public void GetDayEnd()
    {
        dayEnd = true;
    }
}
