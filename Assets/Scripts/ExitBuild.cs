using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitBuild : MonoBehaviour
{
    public void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            QuitApplication();
    }
    }

    public void QuitApplication(){
        Application.Quit();
    }
}


