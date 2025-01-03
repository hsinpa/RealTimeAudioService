using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Text;
using AudioProcessingModuleCs.Media.Dsp.WebRtc;
using AudioProcessingModuleCs.Media;
using System;
using Adrenak.UniMic;
using System.Collections.Concurrent;

namespace Hsinpa.VSU
{
    public class EchoCancelMain : MonoBehaviour
    {
        [SerializeField]
        private AudioSource audioSource;

        WebRtcFilter _webRtcFilter;
        AudioClip mic_audio_clip;
        int mic_audio_index = 0;
        float[] mic_data_buffer;

        private int recordingFrequency = 8000;
        private int sampleDurationMS = 100;
        private ConcurrentQueue<float> signal_queue = new ConcurrentQueue<float>();
        private int buffer_count = 0;
        private int buffer_min = 4;
        private int buffer_max = 12;
        private bool receive_start = false;

        void Start()
        {
            //_webRtcFilter = new WebRtcFilter(24, 256, new AudioFormat(recordingFrequency), new AudioFormat(recordingFrequency), true, true, true);

            //var mic = Adrenak.UniMic.Mic.Instance;
            //mic.StartRecording(recordingFrequency, sampleDurationMS);
            ////mic.OnTimestampedSampleReady += OnMicTimestampedSampleReady;

            ////mic_audio_clip = AudioClip.Create("clip", mic.AudioClip.frequency * 4, mic.AudioClip.channels, mic.AudioClip.frequency, true, PCMReaderCallback);
            //audioSource.clip = mic.AudioClip;
            //audioSource.loop = true;
            //audioSource.Play();
        }

        //private void OnMicTimestampedSampleReady(long index, float[] segment)
        //{
        //    // Mic Send to Filter
        //    audioSource.timeSamples = mic_audio_index;

        //    var short_array = new short[segment.Length];
        //    for (int i = 0; i < segment.Length; i++)
        //    {
        //        short_array[i] = (short)Math.Round(segment[i] * 32768.0f);
        //    }

        //    var byteArray = new byte[short_array.Length * 2];
        //    Buffer.BlockCopy(short_array, 0, byteArray, 0, byteArray.Length);

        //    _webRtcFilter.Write(byteArray);
        //}

        //void PCMReaderCallback(float[] data)
        //{
        //    bool moreFrames;
        //    short[] cancelBuffer = new short[data.Length]; // contains cancelled audio signal

        //    //do
        //    //{
        //        if (_webRtcFilter.Read(cancelBuffer, out moreFrames))
        //        {
        //            // Debug.Log(cancelBuffer[0] / 32768.0f);
        //            for (int i = 0; i < data.Length; i++)
        //            {
        //                data[i] = (cancelBuffer[i] / 32768.0f);
        //            }
        //        }
        //    //} while (moreFrames);

        //}

        //void OnAudioFilterRead(float[] data, int channels)
        //{
        //    var short_array = new short[data.Length];
        //    for (int i = 0; i < data.Length; i++)
        //    {
        //        short_array[i] = (short)Math.Round(data[i] * 32768.0f);
        //    }

        //    var byteArray = new byte[data.Length * 2];
        //    Buffer.BlockCopy(data, 0, byteArray, 0, byteArray.Length);

        //    _webRtcFilter.RegisterFramePlayed(byteArray);
        //}
    }
}
