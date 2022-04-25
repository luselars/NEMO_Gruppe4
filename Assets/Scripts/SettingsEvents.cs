using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsEvents : MonoBehaviour
{
    public List<GameObject> ActivePanelList;
    public GameObject ActivePanel;
    public GameObject CloseButton;
    public List<GameObject> DeactivePanelList;
     
    public void OpenSettings()
    {
        foreach(GameObject panel in ActivePanelList)
        {
            panel.SetActive(true);
        }
        
    }

    public void ActivatePanel()
    {
        ActivePanel.SetActive(true);
        DeactivatePanels();
    }

    public void DeactivatePanels()
    {
        foreach(GameObject panel in DeactivePanelList)
        {
            panel.SetActive(false);
        }
    }
}
