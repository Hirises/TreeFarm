using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Light : MonoBehaviour  //번개의 정보를 담은 클래스
{
    [SerializeField]
    public float Damage;    //데미지
    [SerializeField]
    public float Stun;  //스턴시간
    static public List<Stuff> stuff = new List<Stuff>();
    [SerializeField]
    private BoxCollider2D hitBox;
    [SerializeField]
    private UIManger uiManager;

    public void add(Stuff a)
    {
        uiManager.addNotice(0, a.transform.position.x);
        stuff.Add(a);
    }

    public void remove(Stuff a)
    {
        uiManager.removeNotice(0, a.transform.position.x);
        stuff.Remove(a);
    }

    public void OnEnable()
    {
        Stuff s = null;
        hitBox.enabled = true;
        foreach (Stuff xpos in stuff)
        {
            if (Mathf.Abs(transform.position.x - xpos.transform.position.x) < 4)
            {
                hitBox.enabled = false;
                s = xpos;
            }
        }
        if(hitBox.enabled == false)
        {
            transform.position = new Vector3(s.transform.position.x, transform.position.y, transform.position.z);
            s.Del();
        }
    }
}
