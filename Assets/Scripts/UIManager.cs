using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    TMP_InputField inptYear, inptMonth, inptDay, inptHr, inptMin, simSpeed,inWalk,inAcc,inVis,inEner;
    [SerializeField]
    TMP_Dropdown lightDropDown, camDropDown, heatDropDown, saveDropDown, itemDropDown;
    [SerializeField]
    private GameObject sun, spawnManager;
    [SerializeField]
    TMP_Text txtWalkScore, txtPowerScore, txtAccScore, txtVisScore;
    float waWe, accWe,visWe,enWe;
    void Start(){
        inWalk.text = "25";
        inAcc.text = "25";
        inVis.text = "25";
        inEner.text = "25";
    }
    public float ReadWalkWieght(){
        return float.Parse(inWalk.text);
    }
        public float ReadEnergyWieght(){
        return float.Parse(inEner.text);
    }
        public float ReadAccessiblityWieght(){
        return float.Parse(inAcc.text);
    }
        public float ReadVisiblityWieght(){
        return float.Parse(inVis.text);
    }
    public void UpdateTimeDate(DateTime dateTime)
    {
        inptYear.text = dateTime.Year.ToString();
        inptMonth.text = dateTime.Month.ToString();
        inptDay.text = dateTime.Day.ToString();
        inptHr.text = dateTime.Hour.ToString();
        inptMin.text = dateTime.Minute.ToString();

    }
    public void PauseScene()
    {
        sun.GetComponent<SunPosition>().PauseRotation();
    }
    public void ResumeScene()
    {

        sun.GetComponent<SunPosition>().ResumeRotation();
    }
    public void ReadTimeDate()
    {
        DateTime dateTime = new DateTime(int.Parse(inptYear.text), int.Parse(inptMonth.text), int.Parse(inptDay.text), int.Parse(inptHr.text), int.Parse(inptMin.text), 0);
        sun.GetComponent<SunPosition>().UpdateDateTime(dateTime);
    }
    public void UpdateSimSpeed()
    {
        float ss;
        if (float.TryParse(simSpeed.text, out ss))
        {
            sun.GetComponent<SunPosition>().ChangeSimSpeed(ss);
        }
        else
        {
            simSpeed.text = "1";
        }

    }
    public void ResetTime()
    {
        sun.GetComponent<SunPosition>().ResetTime();

    }

    public void DropDownChange()
    {
        spawnManager.GetComponent<SpawnManager>().ToggleItemSpawn(itemDropDown.value);

    }
    public void UpdateLightList(List<GameObject> ll)
    {
        List<string> lightNames = new List<string>();
        foreach (var light in ll)
        {
            lightNames.Add(light.GetComponent<lightInfo>().lightName);
        }
        lightDropDown.ClearOptions();
        lightDropDown.AddOptions(lightNames);
    }
    public void UpdateCameraList(List<Camera> cl)
    {
        List<string> camNames = new List<string>();
        foreach (var cam in cl)
        {
            camNames.Add(cam.GetComponent<CameraInfo>().camName);
        }
        camDropDown.ClearOptions();
        camDropDown.AddOptions(camNames);
    }
    public void UpdateHeatmapList(List<GameObject> hl)
    {
        List<string> hmNames = new List<string>();
        foreach (var hm in hl)
        {
            hmNames.Add(hm.GetComponent<Heatmap>().heatmapName);
        }
        heatDropDown.ClearOptions();
        heatDropDown.AddOptions(hmNames);
    }
    public void DelLight()
    {
        spawnManager.GetComponent<SpawnManager>().DeleteLight(lightDropDown.value);
    }
    public void DelCamera()
    {

        spawnManager.GetComponent<SpawnManager>().DeleteCamera(camDropDown.value);
    }
    public void LightListSelect()
    {
        spawnManager.GetComponent<SpawnManager>().FocusLight(lightDropDown.value);
    }
    public void HeatmapListSelect()
    {
        spawnManager.GetComponent<SpawnManager>().FocusHeatmap(heatDropDown.value);
    }
    public void CamListSelect()
    {
        spawnManager.GetComponent<SpawnManager>().swapCamera(camDropDown.value);
    }
    public void DefaultCamClicked()
    {
        spawnManager.GetComponent<SpawnManager>().DefaultCam();
    }
    public void EditLight()
    {
        spawnManager.GetComponent<SpawnManager>().EditLight(lightDropDown.value);
    }
    public void EditCam()
    {
        spawnManager.GetComponent<SpawnManager>().EditCam(camDropDown.value);
    }
    public void DailyHeatMapButton()
    {
        spawnManager.GetComponent<SpawnManager>().DailyHeat(heatDropDown.value);
    }
    public void LiveHeatMapButton()
    {
        spawnManager.GetComponent<SpawnManager>().LiveHeat(heatDropDown.value);

    }
    public void DeleteHeatmapButton()
    {

        spawnManager.GetComponent<SpawnManager>().DeleteHeatmap(heatDropDown.value);
    }
    public void Save()
    {
        spawnManager.GetComponent<SpawnManager>().Save();
    }
    public void Load()
    {

        spawnManager.GetComponent<SpawnManager>().Load(saveDropDown.value);
    }
    public void UpdateSaveList(List<string> sl)
    {
        saveDropDown.ClearOptions();
        saveDropDown.AddOptions(sl);
    }
    public void SetWalkScore(float score)
    {
        txtWalkScore.SetText("Walkability Score: " + Math.Round(score, 2));
    }
    public void SetPowerScore(float score)
    {

        txtPowerScore.SetText("Energy Score: " + Math.Round(score, 2));
    }
    public void SetAccessibilityScore(float score)
    {

        txtAccScore.SetText("Accessibility Score: " + Math.Round(score, 2));
    }
    public void SetVisibilityScore(float score)
    {

        txtVisScore.SetText("Visibility Score: " + Math.Round(score, 2));
    }
}
