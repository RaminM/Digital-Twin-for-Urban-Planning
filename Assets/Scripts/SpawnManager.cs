using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.IO;
using Mapbox.Utils;
using Mapbox.Unity.Map;
using UnityEngine.SceneManagement;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _lightPrefab, heatmapPrefab, canvas, buildingPrefab;
    [SerializeField]
    private Camera _cameraPrefab, mainCamera;
    [SerializeField]
    AbstractMap _map;
    private bool spawnHeatmap = false;

    private bool spawnLight = false, spawnCamera = false, spawnBuilding = false;

    private List<GameObject> spawnedLights = new List<GameObject>();
    private List<Camera> spawnedCameras = new List<Camera>();
    private List<GameObject> spawnedHeatmaps = new List<GameObject>();
    private List<GameObject> spawnedBuildings = new List<GameObject>();
    private List<string> saveNameList = new List<string>();
    private GameObject lightIns, heatmapPreviewIns, buildingPreviewIns;
    private int lightCounter = 1, camCounter = 1, heatCounter = 1, saveCounter = 1;
    private bool editingCam = false, editingLight = false;
    private int camToEdit, lightToEdit;
    [SerializeField]
    private float objectMoveSpeed = 10;
    //Input data
    List<string[]> waDataSet, trDataSet, roDataset;
    List<Vector3> trDatasetLocal, waDatasetLocal, roDatasetLocal;
    Vector2d[] waLocations, roLocations;

    void Start()
    {
        heatmapPreviewIns = Instantiate(heatmapPrefab, new Vector3(0, -10f, 0), Quaternion.identity);
        buildingPreviewIns = Instantiate(buildingPrefab, new Vector3(0, -10f, 0), Quaternion.identity);
        //Redading and preparing the data
        waDataSet = new List<string[]>();
        trDataSet = new List<string[]>();
        roDataset = new List<string[]>();
        trDatasetLocal = new List<Vector3>();
        waDatasetLocal = new List<Vector3>();
        roDatasetLocal = new List<Vector3>();
        ReadCSVDataFiles();
        waLocations = new Vector2d[waDataSet.Count];
        roLocations = new Vector2d[roDataset.Count];
        waDataSet.RemoveAt(0);
        trDataSet.RemoveAt(0);
    }
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitData;
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            if (spawnLight)
            {
                // Instantiate light
                if (Input.GetKeyUp(KeyCode.Mouse0))
                {
                    if (Physics.Raycast(ray, out hitData, Mathf.Infinity))
                    {
                        lightIns = Instantiate(_lightPrefab, hitData.point, Quaternion.identity);
                        lightIns.GetComponent<lightInfo>().lightName = "Light " + lightCounter;
                        lightIns.GetComponent<lightInfo>().lightName = "Light " + lightCounter;
                        lightIns.GetComponent<lightInfo>().lat = _map.WorldToGeoPosition(lightIns.transform.position)[0].ToString();
                        lightIns.GetComponent<lightInfo>().lon = _map.WorldToGeoPosition(lightIns.transform.position)[1].ToString();
                        lightIns.GetComponent<lightInfo>().rotation = lightIns.transform.rotation.eulerAngles.x.ToString();
                        spawnedLights.Add(lightIns);
                        lightCounter++;
                        canvas.GetComponent<UIManager>().UpdateLightList(spawnedLights);
                    }
                }
            }
            else if (spawnCamera)
            {
                // Instantiate camera
                if (Input.GetKeyUp(KeyCode.Mouse0))
                {
                    if (Physics.Raycast(ray, out hitData, Mathf.Infinity))
                    {
                        Camera insCam = Instantiate(_cameraPrefab, hitData.point + new Vector3(0, 4, 0), Quaternion.identity);
                        insCam.GetComponent<CameraInfo>().camName = "Camera " + camCounter;
                        insCam.GetComponent<CameraInfo>().lat = _map.WorldToGeoPosition(insCam.transform.position)[0].ToString();
                        insCam.GetComponent<CameraInfo>().lon = _map.WorldToGeoPosition(insCam.transform.position)[1].ToString();
                        insCam.GetComponent<CameraInfo>().rotation = insCam.transform.rotation.eulerAngles.x.ToString();
                        insCam.GetComponent<CameraInfo>().fov = insCam.GetComponent<Camera>().fieldOfView.ToString();
                        insCam.enabled = false;
                        spawnedCameras.Add(insCam);
                        camCounter++;
                        canvas.GetComponent<UIManager>().UpdateCameraList(spawnedCameras);
                    }
                }
            }
            else if (spawnHeatmap)
            {
                if (Physics.Raycast(ray, out hitData, Mathf.Infinity))
                {
                    heatmapPreviewIns.transform.position = hitData.point + Vector3.up * 0.2f;

                }
                if (Input.GetKeyUp(KeyCode.Mouse0))
                {
                    if (Physics.Raycast(ray, out hitData))
                    {
                        GameObject insHeat = Instantiate(heatmapPrefab, hitData.point + Vector3.up * 0.2f, Quaternion.identity);
                        insHeat.GetComponent<Heatmap>().heatmapName = "Heatmap" + heatCounter;
                        spawnedHeatmaps.Add(insHeat);
                        heatCounter++;
                        canvas.GetComponent<UIManager>().UpdateHeatmapList(spawnedHeatmaps);
                    }

                }
            }
            else if (spawnBuilding)
            {
                if (Physics.Raycast(ray, out hitData, Mathf.Infinity))
                {
                    buildingPreviewIns.transform.position = hitData.point;

                }
                if (Input.GetKeyUp(KeyCode.Mouse0))
                {
                    if (Physics.Raycast(ray, out hitData))
                    {
                        GameObject buildingIns = Instantiate(buildingPrefab, hitData.point, Quaternion.identity);
                        buildingIns.GetComponent<BuildingParameterCalculator>().CalcWalkabilityScore(waDataSet,_map);
                        buildingIns.GetComponent<BuildingParameterCalculator>().CalcPowerConsumption();
                        spawnedBuildings.Add(buildingIns);

                    }

                }

            }
            if (editingLight)
            {
                if (Input.GetKey(KeyCode.Y))
                    spawnedLights[lightToEdit].transform.position += Vector3.forward * 10f * Time.deltaTime;
                else if (Input.GetKey(KeyCode.H))
                    spawnedLights[lightToEdit].transform.position += Vector3.back * 10f * Time.deltaTime;
                if (Input.GetKey(KeyCode.G))
                    spawnedLights[lightToEdit].transform.position += Vector3.left * 10f * Time.deltaTime;
                else if (Input.GetKey(KeyCode.J))
                    spawnedLights[lightToEdit].transform.position += Vector3.right * 10f * Time.deltaTime;

                if (Input.GetKey(KeyCode.T))
                    spawnedLights[lightToEdit].transform.eulerAngles += Vector3.right * 10f * Time.deltaTime;
                else if (Input.GetKey(KeyCode.U))
                    spawnedLights[lightToEdit].transform.eulerAngles += Vector3.left * 10f * Time.deltaTime;

                if (Input.GetKey(KeyCode.B))
                    spawnedLights[lightToEdit].transform.eulerAngles += Vector3.up * 10f * Time.deltaTime;
                else if (Input.GetKey(KeyCode.N))
                    spawnedLights[lightToEdit].transform.eulerAngles += Vector3.down * 10f * Time.deltaTime;


                spawnedLights[lightToEdit].GetComponent<lightInfo>().lat = _map.WorldToGeoPosition(spawnedLights[lightToEdit].transform.position)[0].ToString();
                spawnedLights[lightToEdit].GetComponent<lightInfo>().lon = _map.WorldToGeoPosition(spawnedLights[lightToEdit].transform.position)[1].ToString();
                spawnedLights[lightToEdit].GetComponent<lightInfo>().rotation = spawnedLights[lightToEdit].transform.rotation.eulerAngles.x.ToString();


            }
            else if (editingCam)
            {
                if (Input.GetKey(KeyCode.Y))
                    spawnedCameras[camToEdit].transform.position += Vector3.forward * 10f * Time.deltaTime;
                else if (Input.GetKey(KeyCode.H))
                    spawnedCameras[camToEdit].transform.position += Vector3.back * 10f * Time.deltaTime;
                if (Input.GetKey(KeyCode.G))
                    spawnedCameras[camToEdit].transform.position += Vector3.left * 10f * Time.deltaTime;
                else if (Input.GetKey(KeyCode.J))
                    spawnedCameras[camToEdit].transform.position += Vector3.right * 10f * Time.deltaTime;

                if (Input.GetKey(KeyCode.T))
                    spawnedCameras[camToEdit].transform.eulerAngles += Vector3.right * 10f * Time.deltaTime;
                else if (Input.GetKey(KeyCode.U))
                    spawnedCameras[camToEdit].transform.eulerAngles += Vector3.left * 10f * Time.deltaTime;

                if (Input.GetKey(KeyCode.B))
                    spawnedCameras[camToEdit].transform.eulerAngles += Vector3.up * 10f * Time.deltaTime;
                else if (Input.GetKey(KeyCode.N))
                    spawnedCameras[camToEdit].transform.eulerAngles += Vector3.down * 10f * Time.deltaTime;

                spawnedCameras[camToEdit].GetComponent<CameraInfo>().lat = _map.WorldToGeoPosition(spawnedCameras[camToEdit].transform.position)[0].ToString();
                spawnedCameras[camToEdit].GetComponent<CameraInfo>().lon = _map.WorldToGeoPosition(spawnedCameras[camToEdit].transform.position)[1].ToString();
                spawnedCameras[camToEdit].GetComponent<CameraInfo>().rotation = spawnedCameras[camToEdit].transform.rotation.eulerAngles.x.ToString();
            }

        }
    }

    public List<GameObject> GetLightList()
    {
        return spawnedLights;
    }
    public void ToggleItemSpawn(int ind)
    {
        if (ind == 0)
        {
            spawnLight = false;
            spawnCamera = false;
            spawnHeatmap = false;
            spawnBuilding = false;
            Destroy(heatmapPreviewIns);
            Destroy(buildingPreviewIns);
        }
        else if (ind == 1)
        {
            spawnLight = true;
            spawnCamera = false;
            spawnHeatmap = false;
            spawnBuilding = false;
            Destroy(heatmapPreviewIns);
            Destroy(buildingPreviewIns);

        }
        else if (ind == 2)
        {
            spawnLight = false;
            spawnCamera = true;
            spawnHeatmap = false;
            spawnBuilding = false;
            Destroy(heatmapPreviewIns);
            Destroy(buildingPreviewIns);
        }
        else if (ind == 3)
        {
            if (heatmapPreviewIns == null)
                heatmapPreviewIns = Instantiate(heatmapPrefab, this.transform.position, Quaternion.identity);
            spawnHeatmap = true;
            spawnLight = false;
            spawnCamera = false;
            spawnBuilding = false;

        }
        else if (ind == 4)
        {
            if (buildingPreviewIns == null)
                buildingPreviewIns = Instantiate(heatmapPrefab, this.transform.position, Quaternion.identity);
            spawnHeatmap = false;
            spawnLight = false;
            spawnCamera = false;
            spawnBuilding = true;

        }
    }

    public void DeleteLight(int index)
    {
        if (spawnedLights.Count > 0)
        {
            Destroy(spawnedLights[index]);
            spawnedLights.RemoveAt(index);
            canvas.GetComponent<UIManager>().UpdateLightList(spawnedLights);
        }
    }
    public void DeleteHeatmap(int index)
    {
        if (spawnedHeatmaps.Count > 0)
        {
            Destroy(spawnedHeatmaps[index]);
            spawnedHeatmaps.RemoveAt(index);
            canvas.GetComponent<UIManager>().UpdateHeatmapList(spawnedHeatmaps);
        }
    }
    public void DeleteCamera(int index)
    {
        if (spawnedCameras.Count > 0)
        {

            Destroy(spawnedCameras[index]);
            spawnedCameras.RemoveAt(index);
            canvas.GetComponent<UIManager>().UpdateCameraList(spawnedCameras);
        }
    }
    public void FocusLight(int index)
    {
        mainCamera.transform.LookAt(spawnedLights[index].transform.position);
    }
    public void FocusHeatmap(int index)
    {
        mainCamera.transform.LookAt(spawnedHeatmaps[index].transform.position);
    }
    public void swapCamera(int index)
    {
        for (int i = 0; i < spawnedCameras.Count; i++)
        {
            if (i == index)
            {
                spawnedCameras[i].enabled = true;
            }
            else
            {
                spawnedCameras[i].enabled = false;
            }
        }

    }
    public void DefaultCam()
    {
        foreach (var cam in spawnedCameras)
        {
            cam.enabled = false;
        }
    }
    public void EditCam(int index)
    {
        editingCam = true;
        editingLight = false;
        camToEdit = index;

    }
    public void EditLight(int index)
    {
        editingLight = true;
        editingCam = false;
        lightToEdit = index;

    }

    public void DailyHeat(int index)
    {
        spawnedHeatmaps[index].GetComponent<Heatmap>().SetDaily();

    }
    public void LiveHeat(int index)
    {
        spawnedHeatmaps[index].GetComponent<Heatmap>().SetLive();
    }
    public void SetDayEnd()
    {
        Debug.Log("Dayend");
        foreach (var hm in spawnedHeatmaps)
        {
            hm.GetComponent<Heatmap>().SetDayEnd();
        }
    }
    public void Save()
    {
        string name = "Save " + saveCounter;
        WriteCSVSaveFile(name);
        saveNameList.Add(name);
        saveCounter++;
        canvas.GetComponent<UIManager>().UpdateSaveList(saveNameList);

    }
    public void Load(int index)
    {
        name = saveNameList[index];
        RemoveCamAndLights();
        ReadCSVSaveFile(name);
    }
    void ReadCSVSaveFile(string name)
    {
        Debug.Log(name);
        TextAsset myTextAsset = Resources.Load<TextAsset>(name);
        string csvText = myTextAsset.text;
        string[] rows = csvText.Split(
            new[] { "\r\n", "\r", "\n" },
            StringSplitOptions.None
        );
        for (int i = 1; i < rows.GetLength(0) - 1; i++)
        {
            string[] dataRowArray = rows[i].Split(',');
            if (dataRowArray[0] == "Light")
            {
                lightIns = Instantiate(_lightPrefab, _map.GeoToWorldPosition(new Vector2d(double.Parse(dataRowArray[2]), double.Parse(dataRowArray[3]))), Quaternion.identity);
                lightIns.GetComponent<lightInfo>().lightName = dataRowArray[1];
                lightIns.GetComponent<lightInfo>().lat = dataRowArray[2];
                lightIns.GetComponent<lightInfo>().lon = dataRowArray[3];
                lightIns.GetComponent<lightInfo>().rotation = dataRowArray[4];
                spawnedLights.Add(lightIns);
                lightCounter++;
                canvas.GetComponent<UIManager>().UpdateLightList(spawnedLights);
            }
            else if (dataRowArray[0] == "Camera")
            {
                Camera insCam = Instantiate(_cameraPrefab, _map.GeoToWorldPosition(new Vector2d(double.Parse(dataRowArray[2]), double.Parse(dataRowArray[3]))) + (Vector3.up * 4f), Quaternion.identity);
                insCam.GetComponent<CameraInfo>().camName = dataRowArray[1];
                insCam.GetComponent<CameraInfo>().lat = dataRowArray[2];
                insCam.GetComponent<CameraInfo>().lon = dataRowArray[3];
                insCam.GetComponent<CameraInfo>().rotation = dataRowArray[4];
                insCam.GetComponent<CameraInfo>().fov = dataRowArray[6];
                insCam.enabled = false;
                spawnedCameras.Add(insCam);
                camCounter++;
                canvas.GetComponent<UIManager>().UpdateCameraList(spawnedCameras);
            }
        }


    }
    void WriteCSVSaveFile(string name)
    {
        string namePath = Application.dataPath + "/Resources/" + name + ".csv";
        TextWriter tw = new StreamWriter(namePath, false);
        tw.WriteLine("Type,Name,Latitude,Longtitude,Rotation,Intensity,Fov");
        tw.Close();

        tw = new StreamWriter(namePath, true);
        foreach (var light in spawnedLights)
        {
            lightInfo lf = light.GetComponent<lightInfo>();
            tw.WriteLine("Light," + lf.lightName + "," + lf.lat + "," + lf.lon + "," + lf.rotation + "," + lf.lightIntensity + ",-");
        }
        foreach (var camera in spawnedCameras)
        {
            CameraInfo cf = camera.GetComponent<CameraInfo>();
            tw.WriteLine("Camera," + cf.camName + "," + cf.lat + "," + cf.lon + "," + cf.rotation + ",-," + cf.fov);
        }
        tw.Close();
    }
    void RemoveCamAndLights()
    {
        foreach (var light in spawnedLights)
        {
            Destroy(light);
        }
        foreach (var cam in spawnedCameras)
        {
            Destroy(cam);
        }
        spawnedLights.Clear();
        spawnedCameras.Clear();
        canvas.GetComponent<UIManager>().UpdateLightList(spawnedLights);
        canvas.GetComponent<UIManager>().UpdateCameraList(spawnedCameras);
        lightCounter = 0;
        camCounter = 0;
    }
    void ReadCSVDataFiles()
    {
        TextAsset myTextAsset;
        string csvText;
        string[] rows;
        //Walkability
        myTextAsset = Resources.Load<TextAsset>("walkability_n"); // omit file extension
        csvText = myTextAsset.text;
        rows = csvText.Split(
            new[] { "\r\n", "\r", "\n" },
            StringSplitOptions.None
        );
        for (int i = 1; i < rows.GetLength(0) - 1; i++)
        {
            string[] dataRowArray = rows[i].Split(',');
            waDataSet.Add(dataRowArray);
            // Vector2d tempLoc = new Vector2d(double.Parse(dataRowArray[1]), double.Parse(dataRowArray[0]));
            // Vector3 tempLocalLoc = _map.GeoToWorldPosition(tempLoc, true);
            // waDatasetLocal.Add(tempLocalLoc);
            // Debug.Log(tempLocalLoc);
        }
        //Traffic
        myTextAsset = Resources.Load<TextAsset>("Traffic_hour"); // omit file extension
        csvText = myTextAsset.text;
        rows = csvText.Split(
            new[] { "\r\n", "\r", "\n" },
            StringSplitOptions.None
        );
        for (int i = 1; i < rows.GetLength(0) - 1; i++)
        {
            string[] dataRowArray = rows[i].Split(',');
            trDataSet.Add(dataRowArray);
            // Vector2d tempLoc = new Vector2d(double.Parse(dataRowArray[1]), double.Parse(dataRowArray[0]));
            // Vector3 tempLocalLoc = _map.GeoToWorldPosition(tempLoc, true);
            // trDatasetLocal.Add(tempLocalLoc);

        }
        //Route
        myTextAsset = Resources.Load<TextAsset>("route"); // omit file extension
        csvText = myTextAsset.text;
        rows = csvText.Split(
            new[] { "\r\n", "\r", "\n" },
            StringSplitOptions.None
        );
        for (int i = 1; i < rows.GetLength(0) - 1; i++)
        {

            string[] dataRowArray = rows[i].Split(',');
            roDataset.Add(dataRowArray);
            // Vector2d tempLoc = new Vector2d(double.Parse(dataRowArray[1]), double.Parse(dataRowArray[0]));
            // Vector3 tempLocalLoc = _map.GeoToWorldPosition(tempLoc, true);
            // roDatasetLocal.Add(tempLocalLoc);

        }


    }
}
