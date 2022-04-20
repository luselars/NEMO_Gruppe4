using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsEvents : MonoBehaviour
{
    public GameObject SettingsPanel;
    public GameObject CloseButton;
    
    public void OpenPanel()
    {
        if(SettingsPanel != null)
        {
            SettingsPanel.SetActive(true);
        }
    }

    public void ClosePanel()
    {
        if(SettingsPanel != null)
        {
            SettingsPanel.SetActive(false);
        }
    }
}
