using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Setting : MonoBehaviour
{
    private void OnMouseDown()
    {
        SoundManager.instance.ShootSound(0);
        SceneManager.LoadScene("SettingScene");
    }
}
