using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;

namespace Hsinpa.RTAudioService
{
    public class RTAudioSampleScript : MonoBehaviour
    {
        [SerializeField]
        private RTAudioUIThread rtAudioUIThread;

        void Start()
        {
            rtAudioUIThread.OnAudioDataReceived += this.OnAudioDataRead;
            var devices = rtAudioUIThread.GetDevices();

            foreach (string d in devices) {
                Debug.Log(d);
            }

            if (devices.Length > 0)
                rtAudioUIThread.PlayDevice(devices[0]);

            using (var ws = new WebSocket("ws://localhost:8080"))
            {
                ws.OnMessage += (sender, e) =>
                {
                    Debug.Log("Laputa says: " + e.Data);
                };

                ws.Connect();
                ws.Send("BALUS");
            }
        }

        private void OnAudioDataRead(float[] sample_rates) {
            int lens = sample_rates.Length;
            int step = 100;

            //for (int i = 0; i < lens; i += step) {
            //    Debug.Log("OnAudioData " + sample_rates[i]);
            //}

        }

    }
}