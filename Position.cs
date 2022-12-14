using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using System;
using UnityEngine.UI;

using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Microsoft.MixedReality.Toolkit;
using System.IO;
using System.Diagnostics;
using Debug = UnityEngine.Debug;


public class Position : MonoBehaviour
{
    System.Diagnostics.Stopwatch stopwatch = new Stopwatch();
    StreamWriter streamWriter;
    StreamWriter streamWriterT;
    public GameObject end_effector;
    public GameObject background_color;
    public Text my_text;
    public GameObject circle;
    public Text circle_text;
    //private Vector3 a = new Vector3(0, 0, 0);
    float time = 0;
    private InputDevice targetDevice;
    public Vector3 position;
    public Vector3 oriP;
    private int num = 0;
    public void Start()
    {
        /*List<InputDevice> devices = new List<InputDevice>();
        InputDevices.GetDevices(devices);
        //InputDeviceCharacteristics rightControllerCharacteristics = InputDeviceCharacteristics.HeldInHand | InputDeviceCharacteristics.Right | InputDeviceCharacteristics.Controller;
        //InputDevices.GetDevicesWithCharacteristics(rightControllerCharacteristics, devices);

        foreach (var item in devices)
        {
            Debug.Log(item.name + item.characteristics);
        }
        if (devices.Count > 0)
        {
            targetDevice = devices[0];
        }*/
        streamWriter = new StreamWriter(Application.dataPath + "/this_is_a_txt.txt", true);
        streamWriterT = new StreamWriter(Application.dataPath + "/this_is_time_txt.txt", true);
        GameObject.Find("GameObject").GetComponent<Position>().enabled = false;
        //UDPClientIP = "127.0.0.1";//Server IP address
        UDPClientIP = "192.168.95.1";//Server IP address
        UDPClientIP = UDPClientIP.Trim();       
        InitSocket();
        Timemeasure();
    }

    void Update()
    {
        if (OVRInput.Get(OVRInput.Button.One))
        {
            GameObject.Find("GameObject").GetComponent<Position>().enabled = false;
            my_text.text = "You can put down the controller";
            circle.GetComponent<Renderer>().material.SetColor("_Color", Color.red);
            circle_text.text = "Off";
        }
        if (OVRInput.Get(OVRInput.Button.Two))
        {
            SocketSend("O");
        }
        if (OVRInput.Get(OVRInput.Button.SecondaryThumbstickUp))
        {
            SocketSend("C");
        }
        time += Time.deltaTime;
        if (time >= 0.4f)
        {
            time = 0;
            List<InputDevice> devices = new List<InputDevice>();
            InputDeviceCharacteristics rightControllerCharacteristics = InputDeviceCharacteristics.Right | InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.HeldInHand;
            InputDevices.GetDevicesWithCharacteristics(rightControllerCharacteristics, devices);
        
            if (devices.Count > 0)
            {
                targetDevice = devices[0];
            }
    
            targetDevice.TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 position);
            
            if (position != Vector3.zero && targetDevice.TryGetFeatureValue(CommonUsages.deviceRotation, out Quaternion rotation))
            {
                if (num == 0)
                {
                    oriP = position;
                    num += 1;
                }
                Debug.LogFormat(MixedRealityPlayspace.Transform.TransformPoint(position-oriP).ToString() + (MixedRealityPlayspace.Rotation * rotation).ToString());
                SocketSend(MixedRealityPlayspace.Transform.TransformPoint(position-oriP).ToString() + (MixedRealityPlayspace.Rotation * rotation).ToString());
                streamWriter.WriteLine(MixedRealityPlayspace.Transform.TransformPoint(position-oriP));
                if (MixedRealityPlayspace.Transform.TransformPoint(position-oriP).y > -0.045)
                {
                    end_effector.GetComponent<Renderer>().material.SetColor("_Color", Color.blue);
                    background_color.GetComponent<RawImage>().color = Color.blue;
                }
                if (MixedRealityPlayspace.Transform.TransformPoint(position-oriP).y <= -0.045 & MixedRealityPlayspace.Transform.TransformPoint(position-oriP).y >= -0.13)
                {
                    end_effector.GetComponent<Renderer>().material.SetColor("_Color", Color.green);
                    background_color.GetComponent<RawImage>().color = Color.green;
                }
                if (MixedRealityPlayspace.Transform.TransformPoint(position-oriP).y < -0.13)
                {
                    end_effector.GetComponent<Renderer>().material.SetColor("_Color", Color.red);
                    background_color.GetComponent<RawImage>().color = Color.red;
                }
            }
        }
    }

    public void Timemeasure()
    {
        if (GameObject.Find("GameObject").GetComponent<Position>().enabled == true)
        {
            stopwatch.Start(); //  ??????????????????????????????
            if (GameObject.Find("GameObject").GetComponent<Position>().enabled == false)
            {
                stopwatch.Stop(); //  //  ????????????
                TimeSpan timespan = stopwatch.Elapsed; //  ??????????????????????????????????????????
                double seconds = timespan.TotalSeconds;  //  ?????????
                streamWriterT.WriteLine(seconds);
                stopwatch.Reset(); 
            }
        }
        
    }

    //public string recvStr;
    private string UDPClientIP;
    //string str = "?????????01????????????";
    Socket socket;
    EndPoint serverEnd;
    IPEndPoint ipEnd;
 
    //byte[] recvData = new byte[1024];
    byte[] sendData = new byte[1024];
    //int recvLen = 0;
    //Thread connectThread;
 
    
    public void InitSocket()
    {
        ipEnd = new IPEndPoint(IPAddress.Parse(UDPClientIP), 5500);
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
        serverEnd = (EndPoint)sender;
        print("????????????");
        //SocketSend(str);
        print("??????");
        //????????????????????????
        //connectThread = new Thread(new ThreadStart(SocketReceive));
        //connectThread.Start();
    }

    public void SocketSend(string sendStr)
    {
        //Empty
        sendData = new byte[1024];
        //Data transformation
        sendData = Encoding.UTF8.GetBytes(sendStr);
        //Send to designated server
        socket.SendTo(sendData, sendData.Length, SocketFlags.None, ipEnd);
    }

 
    //?????????????????????
    /*void SocketReceive()
    {
        while (true)
        {
 
            recvData = new byte[1024];
            try
            {
                recvLen = socket.ReceiveFrom(recvData, ref serverEnd);
            }
            catch (Exception e)
            {
            }
 
            //print("????????????: " + serverEnd.ToString());
            if (recvLen > 0)
            {
                recvStr = Encoding.UTF8.GetString(recvData, 0, recvLen);
            }
 
            print(recvStr);
        }
    }*/
 
    //????????????
    public void SocketQuit()
    {
        /*//????????????
        if (connectThread != null)
        {
            connectThread.Interrupt();
            connectThread.Abort();
        }*/
        //????????????socket
        if (socket != null)
            socket.Close();
    }
    void OnApplicationQuit()
    {
        SocketQuit();
        streamWriter.Close();
    }

}
