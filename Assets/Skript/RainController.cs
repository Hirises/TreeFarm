using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainController : MonoBehaviour //비 이펙트 클래스
{
    [SerializeField]
    private Sprite[] Rains;
    [SerializeField]
    private SpriteRenderer spriteRenderer;
    [SerializeField]
    private float RainSpeed;
    [SerializeField]
    private AudioSource audioSource;

    private void OnEnable() //활성화시 초기화
    {
        spriteRenderer.sprite = Rains[0];
    }

    private void Update()   //떨어지기
    {
        if (RainSpeed > 0)
        {
            transform.Translate(Vector3.down * Time.deltaTime * RainSpeed);
            if (transform.position.y < -4.15)   //땅에 닿으면 터짐
            {
                StartCoroutine("pop");
                RainSpeed -= 100;
            }
        }
    }

    IEnumerator pop()   //터지기
    {
        //audioSource.Play();

        spriteRenderer.sprite = Rains[1];
        yield return new WaitForSeconds(0.1f);

        spriteRenderer.sprite = Rains[2];
        yield return new WaitForSeconds(0.1f);

        spriteRenderer.sprite = Rains[3];
        yield return new WaitForSeconds(0.1f);

        RainSpeed += 100;
        gameObject.SetActive(false);    //제거(비활성화)
    }
}
