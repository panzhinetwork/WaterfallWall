using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System;

public class UDPClient
{
    private Socket _socket;
    private IPEndPoint _ipEndPoint;
    IPEndPoint _endPoint;

    public bool Open(bool broadcast)
    {
        if (_socket != null)
            return false;

        _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        _socket.EnableBroadcast = broadcast;
        return true;
    }

    public bool Bind(int port)
    {
        if (_socket != null)
        {
            _socket.Bind(new IPEndPoint(IPAddress.Any, port));
            return true;
        }

        return false;
    }

    public void Close()
    {
        if (_socket != null)
        {
            _socket.Close();
            _socket = null;
        }
    }

    public int Send(byte[] data, string ip, int port)
    {
        int nLen = -1;
        try
        {
            nLen = _socket.SendTo(data, new IPEndPoint(IPAddress.Parse(ip), port));
        }
        catch (Exception e)
        {
#if UNITY_EDITOR
            Debug.LogError(e.StackTrace);
#endif
        }
        return nLen;
    }

    public int Receive(byte[] data, ref EndPoint ep)
    {
        int nLen = -1;
        try
        {
            nLen = _socket.ReceiveFrom(data, ref ep);
        }
        catch (Exception e)
        {
#if UNITY_EDITOR
            Debug.LogError(e.StackTrace);
#endif
        }
        return nLen;
    }

    public bool SetSendTimeOut(int ms)
    {
        if (_socket != null)
        {
            _socket.SendTimeout = ms;
            return true;
        }

        return false;
    }

    public bool SetRecvTimeOut(int ms)
    {
        if (_socket != null)
        {
            _socket.ReceiveTimeout = ms;
            return true;
        }

        return false;
    }
}
