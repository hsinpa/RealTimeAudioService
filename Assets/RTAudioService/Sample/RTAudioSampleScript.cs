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

        [SerializeField]
        private AudioSource _simulateSelfAudioSource;

        private RTAudioProcessor m_RTAudioProcessor;
        private RTAudioData m_RTAudioData;
        private RTAudioSocket m_RTAudioSocket;

        async void Start()
        {
            m_RTAudioSocket = new RTAudioSocket();
            m_RTAudioSocket.Connect("ws://localhost:8080");

            m_RTAudioSocket.OnStringMessage += OnSocketMessage;
            m_RTAudioSocket.OnBinaryMessage += OnSocketBinary;

            m_RTAudioData = new RTAudioData(RTAudioUIThread.FREQUENCY, 2);

            _simulateSelfAudioSource.clip = AudioClip.Create("MySinusoid", RTAudioUIThread.FREQUENCY * 2, 1, RTAudioUIThread.FREQUENCY, true, OnAudioRead, OnAudioSetPosition);
            _simulateSelfAudioSource.loop = true;

            m_RTAudioProcessor = new RTAudioProcessor();
            m_RTAudioProcessor.SetAudioClip(_simulateSelfAudioSource.clip);

            var devices = rtAudioUIThread.GetDevices();
            rtAudioUIThread.OnAudioDataReceived += OnMicAudioReceive;


            foreach (string d in devices) {
                Debug.Log(d);
            }

            if (devices.Length > 0)
                rtAudioUIThread.PlayDevice(devices[0]);

            await System.Threading.Tasks.Task.Delay(500);
            _simulateSelfAudioSource.Play();
        }

        void OnAudioRead(float[] data)
        {
            int count = 0;
            //Debug.Log("OnAudioRead " + data.Length);

            m_RTAudioData.GrabData(ref data);
        }

        void OnAudioSetPosition(int newPosition)
        {
            //position = newPosition;
        }

        void OnMicAudioReceive(float[] mic_audio) {
            //Debug.Log("OnMicAudioReceive " + mic_audio.Length);
            m_RTAudioData.AppendData(mic_audio);
        }

        void OnSocketMessage(string event_id, string json_string) { 
            
        }

        void OnSocketBinary(byte[] binary_data)
        {

        }

        private void OnDestroy()
        {
            if (m_RTAudioSocket != null) {
                m_RTAudioSocket.OnStringMessage -= OnSocketMessage;
                m_RTAudioSocket.OnBinaryMessage -= OnSocketBinary;
            }
        }
    }
}