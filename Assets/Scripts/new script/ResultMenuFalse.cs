using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultMenuFalse : MonoBehaviour
{
    public GameObject resultUI;

    void OnClick()
    {
        resultUI.gameObject.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
