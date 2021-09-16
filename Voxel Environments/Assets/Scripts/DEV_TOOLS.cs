using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DEV_TOOLS : MonoBehaviour
{
    [SerializeField] private KeyCode RegenWorldKey = KeyCode.T;
    [SerializeField] private World world;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyUp(RegenWorldKey))
        {
            world.RegenWorld();
        }
    }
}
