using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System;

public class Worker
{
    public delegate void WorkProc(int id, bool quit);
    private Thread _thread = null;
    private WorkProc _proc = null;
    private bool _quit = false;
    private int _id = -1;

    public bool Start(int id, WorkProc proc)
    {
        if (id == -1)
            return false;
        _id = id;
        _quit = false;
        _proc = proc;
        _thread = new Thread(new ThreadStart(ThreadProc));
        _thread.Start();
        return true;
    }

    public void Stop()
    {
        if (_thread != null)
        {
            _quit = false;
            _thread.Join();
            _thread.Abort();
        }
    }

    public bool IsWorking()
    {
        return (_id != -1);
    }

    private void ThreadProc()
    {
        _proc?.Invoke(_id, _quit);
        _id = -1;
    }
}
