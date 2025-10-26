using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class AsyncAwait_Task : MonoBehaviour
{
    int TaskA(int initialValue)
    {
        Debug.Log($"Task A ID: {Thread.CurrentThread.ManagedThreadId}");
        Debug.Log("Task A Start");
        Thread.Sleep(5000);

        int result = initialValue + 1;

        Debug.Log($"Task A ID: {Thread.CurrentThread.ManagedThreadId}");
        Debug.Log($"Task A End with result: {result}");

        return result;
    }

    int TaskB(int resultA)
    {
        Debug.Log($"Task B ID: {Thread.CurrentThread.ManagedThreadId}");
        Debug.Log("Task B Start");
        Thread.Sleep(5000);

        int result = resultA * 2 + 2;
        Debug.Log($"Task B ID: {Thread.CurrentThread.ManagedThreadId}");
        Debug.Log($"Task B End with result: {result}");
        return result;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log($"Main Thread ID: {Thread.CurrentThread.ManagedThreadId}");

        int initialValue = 100;

        // TaskA をスレッドプール上の任意のスレッドで実行してもらう
        Task<int> taskA = Task.Run(() => TaskA(initialValue));

        // TaskA が完了したら ContinueWith で与えたコールバック関数を実行してもらう、ということを指示する
        Task<int> taskB = taskA.ContinueWith(tA =>
        {
            if (tA.IsFaulted)
            {
                Debug.Log("Task A Failed");
                // return 0; // エラー処理が面倒
                return Task.FromException<int>(tA.Exception.InnerException);
            }

            Debug.Log("Task A Succeeded"); 
            int resultA = tA.Result;
            
            // TaskB を起動する
            return Task.Run(() => TaskB(resultA));
        }).Unwrap();

        Task finalTask = taskB.ContinueWith(tB =>
        {
            if (tB.IsFaulted)
            {
                Debug.Log("Task B Failed");
                return;
            }

            Debug.Log("Final Task");
            Debug.Log($"Final callback id: {Thread.CurrentThread.ManagedThreadId}");
            Debug.Log($"Final result: {tB.Result}");
            Debug.Log("Start method is done");
        });
        // finalTask は裏のスレッドプールで走らせる
        // finalTask.Wait(); を実行してしまうと、Unity Start メソッドが終わらず１０ｓゲームが遊べない

        Debug.Log("Calling Start method");
    }
}