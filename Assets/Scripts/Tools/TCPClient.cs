using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System;

public class TCPClient
{
    private Socket _socket;

    public bool Open()
    {
        if (_socket != null)
            return false;

        _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        return true;
    }

    public void Close()
    {
        try
        {
            if (_socket != null)
            {
                if (_socket.Connected)
                    _socket.Disconnect(false);
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

    public void Connect(string ip, int port)
    {
        try
        {
            if (_socket != null)
            {

                _socket.Connect(new IPEndPoint(IPAddress.Parse(ip), port));
            }
        }
        catch (Exception e)
        {
#if UNITY_EDITOR
             Debug.LogError(e.StackTrace);
#endif
        }
    }

    public void Disconnect()
    {
        try
        {
            if (_socket.Connected)
                _socket.Disconnect(true);
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
        if (_socket == null)
            return 0;

        int nLen = -1;
        try
        {
            if (_socket.Connected)
                nLen = _socket.Send(data);
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
        if (_socket == null)
            return 0;

        int nLen = -1;
        try
        {
            if (_socket.Connected)
                nLen = _socket.Receive(data);
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
