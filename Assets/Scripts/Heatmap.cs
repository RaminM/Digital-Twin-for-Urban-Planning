using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heatmap : MonoBehaviour
{
    [SerializeField]
    private float heatmapWidth, heatmapHeight, pixelSize;
    [SerializeField]
    private GameObject heatmapCubePrefab;
    private List<GameObject> cubeList = new List<GameObject>();

    public string heatmapName;
    void Start()
    {
        for (int i = 1; i <= heatmapWidth; i++)
        {
            for (int j = 1; j <= heatmapHeight; j++)
            {
                GameObject insCube = Instantiate(heatmapCubePrefab, this.transform.position, Quaternion.identity);
                insCube.transform.SetParent(this.transform);
                insCube.transform.localPosition = new Vector3(i, 0, j);
                cubeList.Add(insCube);
            }
        }
    }
    public void SetDaily(){
        foreach (var cube in cubeList)
        {
            cube.GetComponent<CubeHeatmap>().ShowDaily();
        }
    }
    public void SetLive(){
        foreach (var cube in cubeList)
        {
            cube.GetComponent<CubeHeatmap>().ShowLive();
        }
    }
    public void SetDayEnd(){
        foreach (var cube in cubeList)
        {
            cube.GetComponent<CubeHeatmap>().GetDayEnd();
        }
    }


}
