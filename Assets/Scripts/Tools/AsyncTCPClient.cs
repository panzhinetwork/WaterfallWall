using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class AsyncTCPClient
{
    private Socket _socket = null;

    public bool Open()
    {
        if (_socket != null)
            return false;

        _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        return true;
    }

    public void Close()
    {
        if (_socket.Connected)
        {
            _socket.BeginDisconnect(false, (IAsyncResult ar) => { }, null);
        }
        if (_socket != null)
        {
            _socket.Close();
            _socket = null;
        }
    }

    public void Connect(string ip, int port, Action<bool> act)
    {
        try
        {
            if (_socket.Connected)
            {
                act?.Invoke(true);
                return;
            }

            _socket.BeginConnect(new IPEndPoint(IPAddress.Parse(ip), port), asyncResult =>
            {
                _socket.EndConnect(asyncResult);
                act?.Invoke(true);
            }, null);
        }
        catch (Exception ex)
        {
            act?.Invoke(false);
            Debug.Log(ex);
        }
    }

    public void Disconnect(Action<bool> act)
    {
        try
        {
            if (!_socket.Connected)
            {
                act?.Invoke(true);
                return;
            }

            _socket.BeginDisconnect(true, asyncResult =>
            {
                _socket.EndDisconnect(asyncResult);
                act?.Invoke(true);
            }, null);
        }
        catch (Exception ex)
        {
            act?.Invoke(false);
            Debug.Log(ex);
        }
    }

    public bool Send(byte[] data, Action<int> act)
    {
        try
        {
            if (!_socket.Connected)
                return false;
            _socket.BeginSend(data, 0, data.Length, SocketFlags.None, asyncResult =>
            {
                act?.Invoke(_socket.EndSend(asyncResult));
            }, null);
        }
        catch (Exception ex)
        {
            act?.Invoke(-1);
            Debug.Log(ex);
        }

        return true;
    }

    public bool Recive(byte[] data, Action<int> act)
    {
        try
        {
            if (!_socket.Connected)
                return false;
            _socket.BeginReceive(data, 0, data.Length, SocketFlags.None, asyncResult =>
            {
                act?.Invoke(_socket.EndReceive(asyncResult));
            }, null);
        }
        catch (Exception ex)
        {
            act?.Invoke(-1);
            Debug.Log(ex);
        }

        return true;
    }
}