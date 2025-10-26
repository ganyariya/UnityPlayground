using System;
using System.Threading;
using UnityEngine;

public class AsyncAwait_Thread : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log($"Main Thread ID: {System.Threading.Thread.CurrentThread.ManagedThreadId}");

        Action finalCallback = () =>
        {
            Debug.Log($"Final Callback ID: {System.Threading.Thread.CurrentThread.ManagedThreadId}");
            Debug.Log("Final callback");
            Debug.Log("Start method is done");
        };

        Action threadBCallback = () =>
        {
            Debug.Log($"Thread B ID: {System.Threading.Thread.CurrentThread.ManagedThreadId}");
            Debug.Log("Thread B Start");
            Thread.Sleep(5000);
            Debug.Log($"Thread B ID: {System.Threading.Thread.CurrentThread.ManagedThreadId}");
            Debug.Log("Thread B End");

            finalCallback();
        };

        Action threadACallback = () =>
        {
            Debug.Log($"Thread A ID: {System.Threading.Thread.CurrentThread.ManagedThreadId}");
            Debug.Log("Thread A Start");
            Thread.Sleep(5000);
            Debug.Log($"Thread A ID: {System.Threading.Thread.CurrentThread.ManagedThreadId}");
            Debug.Log("Thread A End");

            var threadB = new Thread(new ThreadStart(threadBCallback));
            threadB.Start();
            threadB.Join();

            Debug.Log("Thread b end in Thread a");
        };

        var threadA = new Thread(new ThreadStart(threadACallback));
        threadA.Start();

        Debug.Log("Calling Start method");
    }

// Update is called once per frame
    void Update()
    {
    }
}