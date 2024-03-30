using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stuff : MonoBehaviour
{
    [SerializeField]
    private Sprite[] Stuffs;
    [SerializeField]
    private SpriteRenderer spriteRenderer;
    private PlayerController player;
    [SerializeField]
    private GameObject Bomb;

    private void OnEnable()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

        spriteRenderer.enabled = true;
        spriteRenderer.sprite = Stuffs[0];
        player.lightBlot.GetComponent<Light>().add(this);
        StartCoroutine("stuffRun");
    }

    private void OnDisable()
    {
        player.lightBlot.GetComponent<Light>().remove(this);
    }

    public void Del()
    {
        StopCoroutine("stuffRun");
        gameObject.SetActive(false);
    }

    IEnumerator stuffRun()
    {
        yield return new WaitForSeconds(2);
        spriteRenderer.sprite = Stuffs[1];
        yield return new WaitForSeconds(2);
        spriteRenderer.sprite = Stuffs[2];
        yield return new WaitForSeconds(2);
        spriteRenderer.sprite = Stuffs[3];
        yield return new WaitForSeconds(2);
        spriteRenderer.sprite = Stuffs[4];
        yield return new WaitForSeconds(2);

        Bomb.SetActive(true);
        spriteRenderer.enabled = false;
        yield return new WaitForSeconds(0.2f);
        Bomb.SetActive(false);
        Del();
    }
}
