using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStartButton : MonoBehaviour
{
    private void OnMouseUp()
    {
        SoundManager.instance.ShootSound(0);
        SceneManager.LoadScene("MenuScene");
    }
}
