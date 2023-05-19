using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System;

public class Client
{
    private Socket _socket;
    private IPEndPoint _ipEndPoint;
    IPEndPoint _endPoint;
    private bool _udp;

    public void Open(bool udp, string ip, int port)
    {
        _udp = udp;
        _ipEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);

        try
        {
            if (udp)
            {
                _endPoint = new IPEndPoint(IPAddress.Any, 0);
                _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                _socket.Bind(_endPoint);
            }
            else
            {
                _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _socket.Connect(_ipEndPoint);
            }
        }
        catch (Exception e)
        {
#if UNITY_EDITOR
            Debug.LogError(e.StackTrace);
#endif
        }
    }

    public void Close()
    {
        try
        {
            if (_socket != null)
            {
                _socket.Close();
                _socket = null;
            }
        }
        catch (Exception e)
        {
#if UNITY_EDITOR
            Debug.LogError(e.StackTrace);
#endif
        }
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

    public void Connect()
    {
        try
        {
            if (_udp == false)
            {
                _socket.Disconnect(true);
                _socket.Close();
                _socket.Connect(_ipEndPoint);
            }
        }
        catch (Exception e)
        {
#if UNITY_EDITOR
            Debug.LogError(e.StackTrace);
#endif
        }
    }

    public int Send(byte[] data)
    {
        int nLen = -1;
        try
        {
            if (_udp)
            {
                nLen = _socket.SendTo(data, _ipEndPoint);
            }
            else
            {
                if (_socket.Connected)
                    nLen = _socket.Send(data);
            }
        }
        catch (Exception e)
        {
#if UNITY_EDITOR
            Debug.LogError(e.StackTrace);
#endif
        }
        return nLen;
    }

    public int Receive(byte[] data)
    {
        int nLen = -1;
        try
        {
            if (_udp)
            {
                EndPoint ep = (EndPoint)_endPoint;
                nLen = _socket.ReceiveFrom(data, ref ep);
            }
            else
            {
                if (_socket.Connected)
                    nLen = _socket.Receive(data);
            }
        }
        catch (Exception e)
        {
#if UNITY_EDITOR
            Debug.LogError(e.StackTrace);
#endif
        }
        return nLen;
    }
}
