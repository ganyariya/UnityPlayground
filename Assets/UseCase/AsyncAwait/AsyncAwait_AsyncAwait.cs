using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class AsyncAwait_AsyncAwait : MonoBehaviour
{
    async Task<int> TaskA(int initialValue)
    {
        // ここも 1 になることに注意する（メインスレッド）
        Debug.Log($"Task A ID: {Thread.CurrentThread.ManagedThreadId}");
        Debug.Log("Task A Start");
        
        // ここまで Unity メインスレッドで実行される
        // await が呼び出されると、一時的に処理を中断し Unity メインスレッドを他の GameObject へ返す
        await Task.Delay(5000);

        // 5s 経過すると Unity メインスレッドでそのまま処理が再開される
        int result = initialValue + 1;

        // ここも 1 になることに注意する（メインスレッド）
        Debug.Log($"Task A ID: {Thread.CurrentThread.ManagedThreadId}");
        Debug.Log($"Task A End with result: {result}");

        return result;
    }

    async Task<int> TaskB(int resultA)
    {
        Debug.Log($"Task B ID: {Thread.CurrentThread.ManagedThreadId}");
        Debug.Log("Task B Start");
        await Task.Delay(5000);

        int result = resultA * 2 + 2;
        Debug.Log($"Task B ID: {Thread.CurrentThread.ManagedThreadId}");
        Debug.Log($"Task B End with result: {result}");
        return result;
    }

    async Task CannotCreateGameObjectWithoutMainThread()
    {
        // Unity メインスレッドの場合は実行できる
        new GameObject("hoge");
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    async void Start()
    {
        Debug.Log($"Main Thread ID: {Thread.CurrentThread.ManagedThreadId}");

        int initialValue = 100;

        int resultB = -1;
        try
        {
            // TaskA(initialValue) によって TaskA メソッドの処理が開始される
            // TaskA メソッド内の await が実行されるまで、 TaskA は Unity メインスレッドでそのまま実行される
            // TaskA メソッド内の await が実行されたら一時的に処理を中断し、Start メソッドは一時中断となる
            // → これによって Unity UI が固まることなく TaskA 5s 待機を実現できる
            var resultA = await TaskA(initialValue);

            // 5s 経過して TaskA が完了すると、 Unity メインスレッド が再度次の行の実行を再開する
            resultB = await TaskB(resultA);

            await CannotCreateGameObjectWithoutMainThread();
        }
        // 例外処理が try/catch で書けるため楽
        catch (Exception e)
        {
            throw new Exception($"Error {e.Message}");
        }

        await Task.Run(() =>
        {
            Debug.Log("Final Task");
            Debug.Log($"Final callback id: {Thread.CurrentThread.ManagedThreadId}");
            Debug.Log($"Final result: {resultB}");
            Debug.Log("Start method is done");
        });

        Debug.Log("Calling Start method");
    }
}