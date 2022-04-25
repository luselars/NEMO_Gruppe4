using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    public void LoadFishFarmScene() {
        SceneManager.LoadScene("Farm_model");
    }

    public void LoadMenuScene() {
        SceneManager.LoadScene("Start_menu");
    }
}


 
 