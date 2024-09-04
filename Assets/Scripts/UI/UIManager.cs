using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public CrossHairUI[] crossHairs;

    public static UIManager Ins { get; private set; }

    private void Awake()
    {
        if(Ins)
        {
            Destroy(gameObject);
            return;
        }

        Ins = this;

        crossHairs = GetComponentsInChildren<CrossHairUI>();
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
