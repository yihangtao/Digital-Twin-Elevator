// System
using System;
// Unity 
using UnityEngine;
using Debug = UnityEngine.Debug;

public class elevator_box : MonoBehaviour
{
    void FixedUpdate()
    {
        try
        {
            transform.localPosition = new Vector3(0.0f, 0.0f, (float)elevator_data_processing.elevator_Stream_Data.height_parameter);
        }
        catch (Exception e)
        {
            Debug.Log("Exception:" + e);
        }
    }
    void OnApplicationQuit()
    {
        Destroy(this);
    }
}