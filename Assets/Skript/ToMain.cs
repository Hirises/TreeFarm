using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ToMain : MonoBehaviour
{
    private void OnMouseUpAsButton()
    {
        SoundManager.instance.ShootSound(0);
        Run();
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            Run();
        }
    }

    void Run()
    {
        SceneManager.LoadScene("MainScene");
    }
}
