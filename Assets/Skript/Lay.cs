using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lay : MonoBehaviour
{
    private Touch touch;
    private Vector3 vec;
    private Vector3 vec1;
    private Ray ray;
    private RaycastHit2D hit;

    // Update is called once per frame
    void Update()
    {
        vec = new Vector3(touch.position.x, touch.position.y, 10);
        vec1 = Camera.main.ScreenToWorldPoint(vec);
        if (Input.touchCount > 0)
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                touch = Input.GetTouch(i);
                vec = new Vector3(touch.position.x, touch.position.y, 10);
                vec1 = Camera.main.ScreenToWorldPoint(vec);
                hit = Physics2D.Raycast(vec1, Vector3.forward, -10);

                if (hit == true)
                {
                    if (hit.collider.tag == "pad_left")
                    {
                        if (touch.phase == TouchPhase.Began)
                        {
                            TouchPad_Left.Touch_Left = true;
                        }
                        if (touch.phase == TouchPhase.Ended)
                        {
                            TouchPad_Left.Touch_Left = false;
                        }
                    }

                    if (hit.collider.tag == "pad_right")
                    {
                        if (touch.phase == TouchPhase.Began)
                        {
                            TouchPad_Right.Touch_Right = true;
                        }
                        if (touch.phase == TouchPhase.Ended)
                        {
                            TouchPad_Right.Touch_Right = false;
                        }
                    }
                }

                hit = Physics2D.Raycast(vec1, Vector3.forward, -10, LayerMask.GetMask("tree"));

                if (hit == true)
                {
                    if (hit.collider.tag == "tree")
                    {
                        print("123");
                        hit.transform.gameObject.GetComponent<TreeController>()?.Apples();
                    }
                }
            }
        }
    }
}
