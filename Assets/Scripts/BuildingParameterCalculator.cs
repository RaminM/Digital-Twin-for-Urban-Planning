using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mapbox.Utils;
using Mapbox.Unity.Map;

public class BuildingParameterCalculator : MonoBehaviour
{
    private float minElevation = 0f, maxElevation = 30f, minRoadDistance = 0f, maxRoadDistance, minWalkability, maxWalkability;
    private float powerScore, roadDistScore, walkScore;
    UIManager uim;
    Vector3 buildingLoc;
    void Start()
    {
        buildingLoc = this.transform.position;
        uim = GameObject.Find("Canvas").GetComponent<UIManager>();
    }
    void Update()
    {

    }
    public void CalcDistanceFromRoad(List<string[]> roDataset)
    {

    }
    public void CalcPowerConsumption()
    {
        float elveation = this.gameObject.transform.position.y;
        powerScore = 1 - (elveation / maxElevation);
        uim.SetPowerScore(powerScore);

    }
    public void CalcWalkabilityScore(List<string[]> waDataSet, AbstractMap myMap)
    {
        float minDist = Mathf.Infinity;

        buildingLoc = this.transform.position;
        foreach (string[] row in waDataSet)
        {
            Vector2d tempLoc = new Vector2d(double.Parse(row[0]), double.Parse(row[1]));
            Vector3 tempLocalLoc = myMap.GeoToWorldPosition(tempLoc, true);
            float dist = Vector3.Distance(tempLocalLoc, buildingLoc);
            if (dist < minDist)
            {
                minDist = dist;
                walkScore = float.Parse(row[2]);
            }
        }
        uim.SetWalkScore(walkScore);
    }
    public void CalcTrafficScore(List<string[]> trDataSet)
    {

    }


}
