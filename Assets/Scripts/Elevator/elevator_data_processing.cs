// System
using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading;
// Unity 
using UnityEngine;
using Debug = UnityEngine.Debug;

public class elevator_data_processing : MonoBehaviour
{
    public static class GlobalVariables_Main_Control
    {
        public static bool connect, disconnect;
    }
    public static class elevator_Stream_Data
    {
        // Comunication Speed (ms)
        public static int time_step;
        // height
        public static double height_parameter;
        // floor
        public static int current_floor_num;
        public static int target_floor_num;
        public static string ip_address;
        public static bool is_alive = false;
        public static double[] door_position = new double[12];
    }

    public static class elevator_Control_Data
    {
        public static string ip_address;
        public static int time_step;
        public static bool[] button_pressed = new bool[9];
        public static bool stick_button_pressed;
        public static double height_parameter;
        public static int current_floor_num;
        public static int target_floor_num;
        public static bool is_alive = false;
        public static double[] door_position = new double[12];
    }

    private elevator_Stream elevator_stream_robot;
    private elevator_Control elevator_ctrl_robot;
    private int main_elevator_state;

    void Start() 
    {
        elevator_Stream_Data.height_parameter = 11100f;
        elevator_Stream_Data.current_floor_num = 1;
        elevator_Stream_Data.target_floor_num = 1;
        elevator_Stream_Data.ip_address = "127.0.0.1";
        elevator_Control_Data.height_parameter = 11100f;
        elevator_Control_Data.current_floor_num = 1;
        elevator_Control_Data.target_floor_num = 1;
        elevator_Control_Data.ip_address = "127.0.0.1";
        elevator_Stream_Data.time_step = 8;
        elevator_Control_Data.time_step = 8;

/*************************door position initializition*********************************/

        elevator_Control_Data.door_position[6] = 18;
        elevator_Control_Data.door_position[7] = -490;
        elevator_Control_Data.door_position[10] = 370;
        elevator_Control_Data.door_position[11] = -360;
        elevator_Stream_Data.door_position[6] = 18;
        elevator_Stream_Data.door_position[7] = -490;
        elevator_Stream_Data.door_position[10] = 370;
        elevator_Stream_Data.door_position[11] = -360;
/**********************create reading and writing thread********************************/

        elevator_stream_robot = new elevator_Stream();
        elevator_ctrl_robot = new elevator_Control();
    }

    void FixedUpdate() 
    {
        switch (main_elevator_state)
        {
            case 0:
                {
                    if(GlobalVariables_Main_Control.connect == true)
                    {
                        elevator_stream_robot.Start();
                        elevator_ctrl_robot.Start();
                        main_elevator_state = 1;
                    }
                    
                }
                break;
            case 1:
                {
                    for (int i = 0; i < elevator_Control_Data.button_pressed.Length; i++)
                    {
                        if(elevator_Control_Data.button_pressed[i] == true && i < 6)
                        {
                            elevator_Control_Data.target_floor_num = i + 1;
                        }
                    }

                     if (GlobalVariables_Main_Control.disconnect == true)
                    {
                        // Stop threading block {TCP/Ip -> read data}
                        if (elevator_Stream_Data.is_alive == true)
                        {
                            elevator_stream_robot.Stop();
                        }
                        // Stop threading block {TCP/Ip  -> write data}
                        if (elevator_Control_Data.is_alive == true)
                        {
                            elevator_ctrl_robot.Stop();
                        }
                        if (elevator_Stream_Data.is_alive == false && elevator_Control_Data.is_alive == false)
                        {
                            // go to initialization state {wait state -> disconnect state}
                            main_elevator_state = 0;
                        }
                    }
                }
                break;
        }
    }

    void OnApplicationQuit() 
    {
        try
        {
            //Destory data stream reading thread
            elevator_stream_robot.Destroy();
            //Destory data write(simulated elevator) thread
            elevator_ctrl_robot.Destroy();

            Destroy(this);    
        }
        catch(Exception e)
        {
            Debug.Log("Application Quit Exception:" + e);
        }
    }

    class elevator_Stream
    {
        // Initialization of Class variables
        //  Thread
        private Thread robot_thread = null;
        private bool exit_thread = false;

        public void elevator_Stream_Thread()
        {
            try
            {
                // Initialization timer
                var t = new Stopwatch();

                while (exit_thread == false)
                {
                    // Get the data from the robot
                            // t_{0}: Timer start.
                            t.Start();

                            elevator_Stream_Data.height_parameter = elevator_Control_Data.height_parameter;
                            elevator_Stream_Data.current_floor_num = elevator_Control_Data.current_floor_num;
                            elevator_Stream_Data.target_floor_num = elevator_Control_Data.target_floor_num;
                            for (int i = 0; i < elevator_Control_Data.door_position.Length; i++)
                            {
                                elevator_Stream_Data.door_position[i] = elevator_Control_Data.door_position[i];
                            } 

                            // t_{1}: Timer stop.
                            t.Stop();

                            // Recalculate the time: t = t_{1} - t_{0} -> Elapsed Time in milliseconds
                            if (t.ElapsedMilliseconds < elevator_Stream_Data.time_step)
                            {
                                Thread.Sleep(elevator_Stream_Data.time_step - (int)t.ElapsedMilliseconds);
                            }

                            // Reset (Restart) timer.
                            t.Restart();
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
        }

        public void Start()
        {
            // Start thread
            exit_thread = false;
            // Start a thread and listen to incoming messages
            robot_thread = new Thread(new ThreadStart(elevator_Stream_Thread));
            robot_thread.IsBackground = true;
            robot_thread.Start();
            // Thread is active
            elevator_Stream_Data.is_alive = true;
        }
        public void Stop()
        {
            // Stop and exit thread
            exit_thread = true;
            if (robot_thread.IsAlive == true)
            {
                Thread.Sleep(100);
                elevator_Stream_Data.is_alive = false;
            }
        }
        public void Destroy()
        {
            Thread.Sleep(100);
        }
    }

    
    class elevator_Control
    {
        // Initialization of Class variables
        //  Thread
        private Thread robot_thread = null;
        private bool exit_thread = false;

        private bool door_ready = false;

        private float box_speed = 10f;
        private float door_speed = 2f;
        private int door_step_cnt = 0;

        public void elevator_Control_Thread()
        {
            try
            {
                // Initialization timer
                var t = new Stopwatch();

                while (exit_thread == false)
                {
                    // t_{0}: Timer start.
                    t.Start();

                    // Note:
                    //  For more information about commands, see the URScript Programming Language document 

                    // if (elevator_Control_Data.stick_button_pressed == true)
                    // {
                    //     // Send command (byte) -> speed control of the robot (X,Y,Z and EA{RX, RY, RZ})
                    //     network_stream.Write(UR_Control_Data.command, 0, UR_Control_Data.command.Length);
                    // }
                    if (Math.Abs(elevator_Control_Data.height_parameter - (double)(11100f-(elevator_Control_Data.target_floor_num-1)*3700f))>box_speed/2){
                        // change door state for door's switching
                        door_ready = true;

                        if (elevator_Control_Data.height_parameter < (double)(11100f-(elevator_Control_Data.target_floor_num-1)*3700f))
                        {
                            elevator_Control_Data.height_parameter += box_speed;
                            
                        }
                        else
                        {
                            elevator_Control_Data.height_parameter -= box_speed;

                        }
                        //update current floor
                        elevator_Control_Data.current_floor_num = (int)Math.Round(((11100f-elevator_Control_Data.height_parameter)/(3700f)),1)+1;
                    } 
                    // step into door switching state
                    else if (door_ready) {
                        if(door_step_cnt < (int)(490f/door_speed))
                        {
                            // door open
                            elevator_Control_Data.door_position[(elevator_Control_Data.current_floor_num-1)*2] -= door_speed;
                            elevator_Control_Data.door_position[(elevator_Control_Data.current_floor_num-1)*2+1] += door_speed;
                            door_step_cnt += 1;
                        } 
                        else if (door_step_cnt < (int)(750f/door_speed))
                        {
                            //door pause
                            door_step_cnt += 1;
                        }
                        else if (door_step_cnt < (int)(1240f/door_speed))
                        {
                            // door close
                            elevator_Control_Data.door_position[(elevator_Control_Data.current_floor_num-1)*2] += door_speed;
                            elevator_Control_Data.door_position[(elevator_Control_Data.current_floor_num-1)*2+1] -= door_speed;
                            door_step_cnt += 1;
                        }
                        else 
                        {
                            // door stop
                            door_step_cnt = 0;
                            door_ready = false;
                        }
                        
                    }

                    // t_{1}: Timer stop.
                    t.Stop();

                    // Recalculate the time: t = t_{1} - t_{0} -> Elapsed Time in milliseconds
                    if (t.ElapsedMilliseconds < elevator_Stream_Data.time_step)
                    {
                        Thread.Sleep(elevator_Stream_Data.time_step - (int)t.ElapsedMilliseconds);
                    }

                    // Reset (Restart) timer.
                    t.Restart();
                }
            }
            catch (SocketException e)
            {
                Debug.Log("Socket Exception:" + e);
            }
        }

        public void Start()
        {
            // Start thread
            exit_thread = false;
            // Start a thread and listen to incoming messages
            robot_thread = new Thread(new ThreadStart(elevator_Control_Thread));
            robot_thread.IsBackground = true;
            robot_thread.Start();
            // Thread is active
            elevator_Control_Data.is_alive = true;
        }
        public void Stop()
        {
            // Stop and exit thread
            exit_thread = true;
            if (robot_thread.IsAlive == true)
            {
                // Disconnect communication
                Thread.Sleep(100);
                elevator_Control_Data.is_alive = false;
            }
        }
        public void Destroy()
        {
            Thread.Sleep(100);
        }
    }
    

}