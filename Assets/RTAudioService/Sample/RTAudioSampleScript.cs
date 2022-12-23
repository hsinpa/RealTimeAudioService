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

        void Start()
        {
            _simulateSelfAudioSource.clip = AudioClip.Create("MySinusoid", RTAudioUIThread.STREAMING_SAMPLE * 2, 1, RTAudioUIThread.STREAMING_SAMPLE, true, OnAudioRead, OnAudioSetPosition);
            _simulateSelfAudioSource.loop = true;
            _simulateSelfAudioSource.Play();

            m_RTAudioProcessor = new RTAudioProcessor();
            m_RTAudioProcessor.SetAudioClip(_simulateSelfAudioSource.clip);

            rtAudioUIThread.OnAudioDataReceived += this.OnAudioDataRead;
            var devices = rtAudioUIThread.GetDevices();

            foreach (string d in devices) {
                Debug.Log(d);
            }

            if (devices.Length > 0)
                rtAudioUIThread.PlayDevice(devices[0]);
        }

        private void OnAudioDataRead(float[] sample_rates) {
            int lens = sample_rates.Length;
            int mic_position = Mathf.RoundToInt(sample_rates[0]);

            if (lens > 1)
                sample_rates[0] = sample_rates[1];

            //bool is_audio_updated = m_RTAudioProcessor.AppendData(sample_rates, mic_position);

            //if (is_audio_updated)
            //    Debug.Log($"OnAudioDataRead MicPos { mic_position }");


            //_simulateSelfAudioSource.clip.SetData(sample_rates);

            //for (int i = 0; i < lens; i += step) {
            //    Debug.Log("OnAudioData " + sample_rates[i]);
            //}

        }


        void OnAudioRead(float[] data)
        {
            int count = 0;
            Debug.Log("OnAudioRead " + data.Length);
        }

        void OnAudioSetPosition(int newPosition)
        {
            //position = newPosition;
        }

    }
}