/*************************************************************************
Z-position of each floor
F1: 11100
F2: 7400
F3: 3700
F4: 0
F5: -3700
F6: -7400

X-posotion of elevator door
L: -490
R: 490
**************************************************************************/

//System
using System;
using System.Text;
//Unity
using UnityEngine;
using UnityEngine.UI;
//TM
using TMPro;

public class main_ui_control : MonoBehaviour
{
    //----------------------------GameObject-------------------------//
    public GameObject camera_obj;
    //----------------------------Image-----------------------------//
    public Image connection_panel_img, diagnostic_panel_img, stick_panel_img;
    public Image connection_info_img;
    //-----------------------------TMP_InputField-------------------------------//
    public TMP_InputField ip_address_txt;
    //----------------------------Float-----------------------------//
    private float ex_param = 2000f;
    //----------------------------TextMeshProUGUI-------------------//
    public TextMeshProUGUI height_txt;
    public TextMeshProUGUI current_floor_txt;
    public TextMeshProUGUI target_floor_txt;
    public TextMeshProUGUI connectionInfo_txt;
    private UTF8Encoding utf8 = new UTF8Encoding();
    

    // Start is called before the first frame update
    void Start()
    {
        // Connection information {image} -> Connect/Disconnect
        connection_info_img.GetComponent<Image>().color = new Color32(255, 0, 48, 50);
        // Connection information {text} -> Connect/Disconnect
        connectionInfo_txt.text = "Disconnect";
        // Panel Initialization -> Connection/Diagnostic/Joystick Panel

        // Panel Initialization -> Connection/Diagnostic/Joystick Panel
        connection_panel_img.transform.localPosition = new Vector3(25361f + (ex_param), 0f, 0f);
        diagnostic_panel_img.transform.localPosition = new Vector3(9696f + (ex_param), 0f, 0f);
        stick_panel_img.transform.localPosition = new Vector3(17381f + (ex_param), 0f, 0f);

        //Height_txt
        height_txt.text = "0.00";
        current_floor_txt.text = "1";
        target_floor_txt.text = "1";
        ip_address_txt.text = "127.0.0.1";
        

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        elevator_data_processing.elevator_Stream_Data.ip_address = ip_address_txt.text;
        // ------------------------ Connection Information ------------------------//
        // If the button (connect/disconnect) is pressed, change the color and text
        if (elevator_data_processing.GlobalVariables_Main_Control.connect == true)
        {
            // green color
            connection_info_img.GetComponent<Image>().color = new Color32(135, 255, 0, 50);
            connectionInfo_txt.text = "Connect";
        }
        else if(elevator_data_processing.GlobalVariables_Main_Control.disconnect == true)
        {
            // red color
            connection_info_img.GetComponent<Image>().color = new Color32(255, 0, 48, 50);
            connectionInfo_txt.text = "Disconnect";
        }

        // ------------------------ Cyclic read parameters {diagnostic panel}------------------------------//
        // Height_txt
        height_txt.text = ((float)Math.Round((11100f-elevator_data_processing.elevator_Stream_Data.height_parameter)/(1000f),3)).ToString();
        current_floor_txt.text = ((int)Math.Round(((11100f-elevator_data_processing.elevator_Stream_Data.height_parameter)/(3700f)),1)+1).ToString();
        target_floor_txt.text = (elevator_data_processing.elevator_Stream_Data.target_floor_num).ToString();

    }

    // ------------------------------------------------------------------------------------------------------------------------//
    // -------------------------------------------------------- FUNCTIONS -----------------------------------------------------//
    // ------------------------------------------------------------------------------------------------------------------------//

    // -------------------- Destroy Blocks -------------------- //
    void OnApplicationQuit()
    {
        // Destroy all
        Destroy(this);
    }
    // -------------------- Connection Panel -> Visible On -------------------- //
    public void TaskOnClick_ConnectionBTN()
    {
        // visible on
        connection_panel_img.transform.localPosition = new Vector3(-20000f, 0f, 0f);
        // visible off
        diagnostic_panel_img.transform.localPosition = new Vector3(9696f + (ex_param), 0f, 0f);
        stick_panel_img.transform.localPosition = new Vector3(17381f + (ex_param), 0f, 0f);
    }
    // -------------------- Connection Panel -> Visible off -------------------- //
    public void TaskOnClick_EndConnectionBTN()
    {
        connection_panel_img.transform.localPosition = new Vector3(25361f + (ex_param), 0f, 0f);
    }

     // -------------------- Diagnostic Panel -> Visible On -------------------- //
    public void TaskOnClick_DiagnosticBTN()
    {
        // visible on
        diagnostic_panel_img.transform.localPosition = new Vector3(-20000f, -1500f, 0f);
        // visible off
        connection_panel_img.transform.localPosition = new Vector3(25361f + (ex_param), 0f, 0f);
        //stick_panel_img.transform.localPosition = new Vector3(17381f + (ex_param), 0f, 0f);
    }

    // -------------------- Diagnostic Panel -> Visible Off -------------------- //
    public void TaskOnClick_EndDiagnosticBTN()
    {
        diagnostic_panel_img.transform.localPosition = new Vector3(9696f + (ex_param), 0f, 0f);
    }

    // -------------------- Stick Panel -> Visible On -------------------- //
    public void TaskOnClick_StickBTN()
    {
        // visible on
        stick_panel_img.transform.localPosition = new Vector3(-20000f, 1500f, 0f);
        // visible off
        connection_panel_img.transform.localPosition = new Vector3(25361f + (ex_param), 0f, 0f);
        //diagnostic_panel_img.transform.localPosition = new Vector3(9696f + (ex_param), 0f, 0f);
    }

    // -------------------- Stick Panel -> Visible Off -------------------- //
    public void TaskOnClick_EndStickBTN()
    {
        stick_panel_img.transform.localPosition = new Vector3(17381f + (ex_param), 0f, 0f);
    }

    // -------------------- Camera Position -> Right -------------------- //
    public void TaskOnClick_CamViewRBTN()
    {
        camera_obj.transform.localPosition    = new Vector3(0.114f, 2.64f, -2.564f);
        camera_obj.transform.localEulerAngles = new Vector3(10f, -30f, 0f);
    }

    // -------------------- Camera Position -> Left -------------------- //
    public void TaskOnClick_CamViewLBTN()
    {
        camera_obj.transform.localPosition = new Vector3(-3.114f, 2.64f, -2.564f);
        camera_obj.transform.localEulerAngles = new Vector3(10f, 30f, 0f);
    }

    // -------------------- Camera Position -> Home (in front) -------------------- //
    public void TaskOnClick_CamViewHBTN()
    {
        camera_obj.transform.localPosition = new Vector3(-0.5f, 1.5f, -2.3f);
        camera_obj.transform.localEulerAngles = new Vector3(4.635f, 0.165f, 0.603f);
    }

    // -------------------- Camera Position -> Top -------------------- //
    public void TaskOnClick_CamViewTBTN()
    {
        camera_obj.transform.localPosition = new Vector3(-0.6f, 1.5f, 2.38f);
        camera_obj.transform.localEulerAngles = new Vector3(-184f, -2.13f, 180.15f);
    }

    // -------------------- Connect Button -> is pressed -------------------- //
    public void TaskOnClick_ConnectBTN()
    {
        elevator_data_processing.GlobalVariables_Main_Control.connect    = true;
        elevator_data_processing.GlobalVariables_Main_Control.disconnect = false;
    }

    // -------------------- Disconnect Button -> is pressed -------------------- //
    public void TaskOnClick_DisconnectBTN()
    {
        elevator_data_processing.GlobalVariables_Main_Control.connect    = false;
        elevator_data_processing.GlobalVariables_Main_Control.disconnect = true;
    }

}
