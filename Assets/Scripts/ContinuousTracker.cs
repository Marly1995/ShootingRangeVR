using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContinuousTracker : MonoBehaviour
{
    public float checkTime;
    float lastTime;

    MousePositionRecorder rec;
	void Start ()
    {
        rec = GetComponent<MousePositionRecorder>();
	}
	
	void Update ()
    {
	    if (Time.time - checkTime >= lastTime)
        {
            lastTime = Time.time;
            rec.ContinuousCheckRecognized();
        }
	}
}
