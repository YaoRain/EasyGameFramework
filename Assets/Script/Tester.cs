using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Tester : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        EventCenter.Instance.AddEventListener("w",MoveW);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            BufferPoolManager.Instance.GetObjAsyn("Cube",(obj)=>
            {
                obj.transform.localScale *= 2;
            });
        }
    }
    
    public void MoveW(object obj)
    {
        if(this.gameObject.activeInHierarchy)
            this.gameObject.SetActive(false);
        else this.gameObject.SetActive(true);
    }
}
