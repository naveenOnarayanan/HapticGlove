using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using SimpleJSON;

//deals with all the calls to our servers
public class NetworkController : MonoBehaviour
{
	private static NetworkController ncInstance;
	Thread dataThread;
    UdpClient dataClient;

    Socket client;
    IPEndPoint servoServer;
    IPEndPoint peltierServer;
    IPEndPoint accelGyroServer;

    public static string PELTIER = "peltier";
	public static string SERVO = "servo";
	public static string ACCEL_GYRO = "accel_gyro";
    
    int servoPort = 3000;
    int peltierPort = 3001;
    int accelGyroPort = 3002;
    string IP = "10.22.78.165";

    Dictionary<string, IPEndPoint> serverMap;

    bool connected = true;

	public static NetworkController instance() {
		if (NetworkController.ncInstance == null) {
			NetworkController.ncInstance = new NetworkController();
		}
		return NetworkController.ncInstance;
	}

    //connect to the servers
    public NetworkController() {
        serverMap = new Dictionary<string, IPEndPoint>();

        servoServer = new IPEndPoint (IPAddress.Parse (IP), servoPort);
        peltierServer = new IPEndPoint (IPAddress.Parse (IP), peltierPort);
        accelGyroServer = new IPEndPoint (IPAddress.Parse(IP), accelGyroPort);
        client = new Socket (AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        try {
            //TODO: this doesn't work for UDP, how do a check if connected?
            //client.Connect (servoServer);
            //client.Connect (peltierServer);
            //connected = true;
            serverMap["peltier"] = peltierServer;
            serverMap["servo"] = servoServer;
            serverMap["accel_gyro"] = accelGyroServer;
        } catch {
            connected = false;
        }
    }
    
    public void sendData(string data, string serverName) {
        if (!connected || !serverMap.ContainsKey(serverName))
            return;
        
        //TODO: process data as json
        Int32 timestamp = (Int32)(DateTime.UtcNow.Subtract (new DateTime (1970, 1, 1))).TotalSeconds;
        string json = "{\"timestamp\":" + timestamp + ", " + data + "}";

        byte[] d = Encoding.UTF8.GetBytes (json);
        //UdpClient receivingClient = new UdpClient(8000);
        client.SendTo (d, d.Length, SocketFlags.None, (IPEndPoint) serverMap[serverName]);

    }
	
	//TODO: set this to be pulling to neutral
    public void resetServo() {
        sendData ("\"angle\": 0", SERVO);
    }

	//TODO: set this to be pulling to taught
    public void pullServo() {
        sendData ("\"angle\": 180", SERVO);
    }
	
    public void resetPeltier() {
        sendData("\"temperature\": 0", PELTIER);
    }

    public void heatPeltier(Magic.Size size) {
		int temp;

		switch (size) {
			case Magic.Size.Small:
				temp = 5;
				break;
			case Magic.Size.Medium:
				temp = 10;
				break;
			default:
				temp = 15;
				break;
		}
        sendData("\"temperature\": " +  temp.ToString(), PELTIER);
    }

    public void coolPeltier() {
        sendData("\"temperature\": -15", PELTIER);
    }

    public void accelGyro() {
        dataThread = new Thread(new ThreadStart(readData));
        dataThread.IsBackground = true;
        dataThread.Start();
        //sendData ("\"accel_gyro\":\"null\"", ACCEL_GYRO, true);
    }

    void OnApplicationQuit() {
		stopThread();
    }

    public void stopThread() {
        if (dataThread != null && dataThread.IsAlive) {
            dataThread.Abort();
        }
        if (dataClient != null) {
            dataClient.Close();
        }
    }

    public void readData() {
        dataClient = new UdpClient(8000);
        while (true) {
            try {
                IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 8000);
                byte[] data = dataClient.Receive(ref anyIP);
                string returnData = Encoding.ASCII.GetString(data);
                JSONNode node = SimpleJSON.JSON.Parse(returnData);
                int timestamp = node["timestamp"].AsInt;
                if (UserData.timestamp < timestamp) {
                    if (node["accel_gyro"] != null) {
                        UserData.accel_gyro = new double[]{node["accel_gyro"]["x"].AsDouble, node[ "accel_gyro"]["y"].AsDouble};
                    }
                    if (node["servo"] != null) {
                        UserData.servo = new double[]{node["servo"][0].AsDouble, node["servo"][1].AsDouble};
                    }
                    UserData.timestamp = timestamp;
                }

                Debug.Log (returnData);
            } catch(Exception e) {
                Debug.Log("Unable to receive data");
            }
        }
    }
}
