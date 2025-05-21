using System.Collections;
using System.Collections.Generic;
using Blync.Core;
using UnityEngine;

public class BlyncTest : MonoBehaviour
{
    BlyncController blyncController;
    [SerializeField]
    BlyncControllerData blyncControllerData;
    // Start is called before the first frame update
    void Start()
    {
        blyncControllerData.ChangeSession(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (blyncControllerData.isSessionStarted()) {
            Debug.Log(blyncControllerData.sensorSpeed.value);
        }
    }
}
