// System
using System;
// Unity 
using UnityEngine;
using Debug = UnityEngine.Debug;

public class elevator_door_4R : MonoBehaviour
{
    void FixedUpdate()
    {
        try
        {
            transform.localPosition = new Vector3((float)elevator_data_processing.elevator_Stream_Data.door_position[7], 0.0f, 0.0f);
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