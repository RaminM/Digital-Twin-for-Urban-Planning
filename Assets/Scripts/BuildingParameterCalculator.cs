using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mapbox.Utils;
using Mapbox.Unity.Map;

public class BuildingParameterCalculator : MonoBehaviour
{
    private float minElevation = 0f, maxElevation = 30f, minRoadDistance = 0f, maxRoadDistance = 300f, minWalkability, maxWalkability, maxVisDistance = 50f;
    private int buildingHeight = 6;
    private float powerScore, trafficScore, walkScore, visibilityScore, roadDistance;
    UIManager uim;
    Vector3 buildingLoc;
    private AbstractMap _map;
    private List<string[]> _walkdData, _roadData, _trafficData;
    void Awake()
    {
        buildingLoc = this.transform.position;

        uim = GameObject.Find("Canvas").GetComponent<UIManager>();

    }
    public void CalculateParameters(List<string[]> waDataSet, List<string[]> roDataset, List<string[]> trDataSet, AbstractMap myMap)
    {
        _walkdData = waDataSet;
        _roadData = roDataset;
        _map = myMap;
        _trafficData = trDataSet;
        CalcWalkabilityScore();
        CalcPowerConsumption();
        CalcTrafficScore();
        CalcLightScore();
        // CheckOnLand();
    }
    // private void CheckOnLand()
    // {
    //     RaycastHit hitData;
    //     Ray ray = new Ray(this.transform.position, Vector3.down);
    //     if (Physics.Raycast(ray, out hitData, Mathf.Infinity,6))
    //     {
    //         Debug.Log("Hit terrain");
    //     }
    // }
    private void CalcPowerConsumption()
    {
        float elveation = this.gameObject.transform.position.y;
        powerScore = 1 - (elveation / maxElevation);
        uim.SetPowerScore(powerScore);

    }
    private void CalcWalkabilityScore()
    {
        float minDist = Mathf.Infinity;

        buildingLoc = this.transform.position;
        foreach (string[] row in _walkdData)
        {
            Vector2d tempLoc = new Vector2d(double.Parse(row[0]), double.Parse(row[1]));
            Vector3 tempLocalLoc = _map.GeoToWorldPosition(tempLoc, true);
            float dist = Vector3.Distance(tempLocalLoc, buildingLoc);
            if (dist < minDist)
            {
                minDist = dist;
                walkScore = float.Parse(row[2]);
            }
        }
        uim.SetWalkScore(walkScore);
    }
    private void CalcDistanceFromRoad()
    {
        float minDist = Mathf.Infinity;

        buildingLoc = this.transform.position;
        foreach (string[] row in _roadData)
        {
            Vector2d tempLoc = new Vector2d(double.Parse(row[1]), double.Parse(row[0]));
            Vector3 tempLocalLoc = _map.GeoToWorldPosition(tempLoc, true);
            float dist = Vector3.Distance(tempLocalLoc, buildingLoc);
            if (dist < minDist)
            {
                minDist = dist;
            }
        }
        roadDistance = minDist;

    }
    private void CalcTrafficScore()
    {
        CalcDistanceFromRoad();
        float div = roadDistance / maxRoadDistance;
        if (div > 1) div = 1;
        trafficScore = 1 - div;
        uim.SetAccessibilityScore(trafficScore);


    }
    private void CalcLightScore()
    {
        float numHit = 0, numNothit = 0;
        Ray ray = new Ray();
        for (int i = 1; i <= buildingHeight; i++)
        {
            ray.origin = new Vector3(this.transform.position.x, this.transform.position.y + i, this.transform.position.z);
            for (int j = 0; j < 360; j++)
            {
                float angle = Mathf.Deg2Rad * j;
                ray.direction = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));
                // Debug.DrawRay(ray.origin, ray.direction * 10, Color.red, 100f);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, maxVisDistance))
                {
                    numHit++;
                }
                else
                {
                    numNothit++;
                }
            }
        }
        visibilityScore = (numNothit / (numNothit + numHit));
        uim.SetVisibilityScore(visibilityScore);

    }


}
