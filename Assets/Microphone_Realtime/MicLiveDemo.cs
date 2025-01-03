using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Adrenak.UniMic;
using AudioProcessingModuleCs.Media.Dsp.WebRtc;
using System.Runtime.Remoting.Channels;
using AudioProcessingModuleCs.Media;
using Hsinpa;
using System;
using System.Collections.Concurrent;

namespace Hsinpa
{

    public class MicLiveDemo : MonoBehaviour
    {
        [SerializeField]
        private MicAudioSource micAudioSource;

        [SerializeField]
        private AudioSource speakerAudioSource;

        private int samplingFrequency = 8000;

        WebRtcFilter webRtcFilterEnhancer;

        // Mic Buffer Array
        byte[] micByteArray = null;
        short[] micShortBuffer = null;
        short[] micProcessedBuffer = null;
        float[] micProcessedData = null;

        // Speaker Buffer Array
        byte[] speakerByteArray = null;
        short[] speakerShortBuffer = null;

        ConcurrentQueue<float> mic_process_queue = new ConcurrentQueue<float>();

        // Start is called before the first frame update
        void Start()
        {
            webRtcFilterEnhancer = new WebRtcFilter(450, 250, new AudioFormat(samplingFrequency), new AudioFormat(samplingFrequency), true, true, true);
            micProcessedBuffer = new short[webRtcFilterEnhancer.SamplesPerFrame];
            micProcessedData = new float[webRtcFilterEnhancer.SamplesPerFrame];

            Debug.Log($"OnSourceFrameCollected SamplesPerFrame {webRtcFilterEnhancer.SamplesPerFrame}");

            Mic.Init();

            if (Mic.AvailableDevices.Count > 0)
            {
                Mic.AvailableDevices[0].StartRecording(samplingFrequency: samplingFrequency);

                Mic.AvailableDevices[0].OnFrameCollected += OnSourceFrameCollected;
                micAudioSource.Device = Mic.AvailableDevices[0];

                AudioClip myClip = AudioClip.Create("MySinusoid", samplingFrequency * 2, 1, samplingFrequency, true, PCMReaderCallback);
                speakerAudioSource.clip = myClip;

                speakerAudioSource.loop = true;
                speakerAudioSource.Play();
            }
        }

        // Callback from Mic
        private void OnSourceFrameCollected(int frequency, int channels, float[] samples)
        {
            //Debug.Log($"OnSourceFrameCollected frequency {frequency}, channel {channels}, sample length {samples.Length}");

            // Initiate Array if still null
            if (micByteArray == null || micByteArray.Length != samples.Length)
            {
                micShortBuffer = new short[samples.Length];
                micByteArray = new byte[samples.Length * sizeof(short)];
            }

            MicUtility.sample_float_to_short(ref samples, ref micShortBuffer);
            Buffer.BlockCopy(micShortBuffer, 0, micByteArray, 0, micByteArray.Length);

            // Send to enhancer
            webRtcFilterEnhancer.Write(micByteArray);

            // Get data and insert into queue
            bool moreFrames;
            while (webRtcFilterEnhancer.Read(micProcessedBuffer, out moreFrames))
            {
                MicUtility.sample_short_to_float(ref micProcessedBuffer, ref micProcessedData);

                for (int i = 0; i < micProcessedData.Length; i++)
                {
                    mic_process_queue.Enqueue(micProcessedData[i]);
                }
            }
        }

        // Callback from Speaker clip
        public void PCMReaderCallback(float[] data)
        {

            // If queue length is less than data.length, then ignore
            if (data.Length > mic_process_queue.Count)
            {
                return;
            }

            // Get data from queue
            float data_point = 0;
            for (int i = 0; i < data.Length; i++)
            {
                if (mic_process_queue.TryDequeue(out data_point))
                {
                    data[i] = data_point;
                }
            }

            // Initiate Array if still null or length diff
            if (speakerByteArray == null || speakerByteArray.Length != data.Length)
            {
                speakerShortBuffer = new short[data.Length];
                speakerByteArray = new byte[data.Length * sizeof(short)];
            }

            MicUtility.sample_float_to_short(ref data, ref speakerShortBuffer);
            Buffer.BlockCopy(speakerShortBuffer, 0, speakerByteArray, 0, speakerByteArray.Length);

            // Register frame
            webRtcFilterEnhancer.RegisterFramePlayed(speakerByteArray);
        }
    }

}