using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThreadedDataRequester : MonoBehaviour {

    static ThreadedDataRequester instance;
    Queue<ThreadInfo> dataQueue = new Queue<ThreadInfo>();

    void Awake() {
        instance = FindObjectOfType<ThreadedDataRequester>();
    }

    public static void RequestData(Func<object> generateData, Action<object> callback) {
        // Telling the threadstart where to begin
        ThreadStart threadStart = delegate {
            // Calling our data thread func as our main threaded func
            instance.DataThread(generateData, callback);
        };

        // Start the thread
        new Thread(threadStart).Start();
    }

    private void DataThread(Func<object> generateData, Action<object> callback) {
        // We are calling our function and assigning data to whatever it returns
        object data = generateData();
        // Make sure that we are not enqueueing stuff at the same time
        lock (dataQueue) {
            dataQueue.Enqueue(new ThreadInfo(callback, data));
        }
    }

    void Update() {
        // If we have stuff in our data queue then we will update
        if (dataQueue.Count > 0) {
            // We loop through everything we have queued up currently
            for (int i = 0; i < dataQueue.Count; i++) {
                // We Deuque our thread we have finnished
                ThreadInfo threadInfo = dataQueue.Dequeue();
                // Calling our callback with the data we have recieved to let us know when the functions done
                threadInfo.callback(threadInfo.parameter);
            }
        }
    }

    struct ThreadInfo {
        public readonly Action<object> callback;
        public readonly object parameter;

        public ThreadInfo (Action<object> callback, object parameter) {
            this.callback = callback;
            this.parameter = parameter;
        }
    }
}
