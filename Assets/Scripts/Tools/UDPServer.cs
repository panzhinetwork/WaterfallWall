using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class UDPServer
{
    public delegate void RecvProc(string data);

    private Worker _worker;
    private Socket _socket;
    private RecvProc _recvProc;
    private static object _locker = new object();

    public void Start(int port, RecvProc recvProc)
    {
        IPEndPoint ip = new IPEndPoint(IPAddress.Any, port);
        _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        _socket.Bind(ip);
        _recvProc = recvProc;
        _worker = new Worker();
        _worker.Start(1, WorkProc);
    }

    public void Stop()
    {
        //lock(_locker)
        {
            if (_socket != null)
            {
                _socket.Close();
                _socket = null;
            }
        }

        if (_worker != null)
        {
            _worker.Stop();
            _worker = null;
        }
    }

    private void WorkProc(int id, bool quit)
    {
        IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
        EndPoint remote = (EndPoint)(sender);
        byte[] data = new byte[1024];
        while (quit == false)
        {
            //lock (_locker)
            {
                if (_socket != null)
                {
                    int len = _socket.ReceiveFrom(data, ref remote);
                    if (len > 0)
                    {
                        _recvProc?.Invoke(Encoding.UTF8.GetString(data, 0, len));
                    }
                }
            }
        }
    }
}
