using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quit : MonoBehaviour
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
        Application.Quit();
    }
}
