using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;

//deals with all the calls to our servers
public class NetworkController
{
    Socket client;
    IPEndPoint servoServer;
    IPEndPoint peltierServer;

    string PELTIER = "peltier";
    string SERVO = "servo";
    
    int servoPort = 3000;
    int peltierPort = 3001;
    string IP = "169.254.191.81";

    Dictionary<string, IPEndPoint> serverMap;

    bool connected = false;

    //connect to the servers
    public NetworkController() {
        serverMap = new Dictionary<string, IPEndPoint>();

        servoServer = new IPEndPoint (IPAddress.Parse (IP), servoPort);
        peltierServer = new IPEndPoint (IPAddress.Parse (IP), peltierPort);
        client = new Socket (AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        try {
            //TODO: this doesn't work for UDP, how do a check if connected?
            //client.Connect (servoServer);
            //client.Connect (peltierServer);
            //connected = true;
            serverMap["peltier"] = peltierServer;
            serverMap["servo"] = servoServer;
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
        
        client.SendTo (d, d.Length, SocketFlags.None, serverMap[serverName]);
    }

    public void resetServo() {
        sendData ("\"angle\": 0", SERVO);
    }

    public void pullServo() {
        sendData ("\"angle\": 180", SERVO);
    }

    public void resetPeltier() {
        sendData("\"temperature\": 0", PELTIER);
    }

    public void heatPeltier() {
        sendData("\"temperature\": 10", PELTIER);
    }

    public void coolPeltier() {
        sendData("\"temperature\": -10", PELTIER);
    }
}

