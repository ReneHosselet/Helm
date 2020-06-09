using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadMyScene : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            LoadNewScene();
        }
    }
    public void LoadNewScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
}