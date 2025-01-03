using Hsinpa.AdaptiveFilter;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;
using NumSharp;

namespace Hsinpa.Mic
{
    public class MicrophoneEcho : MonoBehaviour
    {
        int frame_block_index = 0;
        const int frame_block_num = 10;
        const int frame_size = 1024;

        AudioClip mic;
        int lastPos, pos;
        float[] mic_data_buffer;
        float[] frame_history_buffer = new float[frame_size * frame_block_num];
        float[] process_frame_buffer = new float[frame_size];

        ConcurrentQueue<float> mic_data_queue = new ConcurrentQueue<float>();
        LMSAdaptiveFilter _lmsAdaptiveFilter;

        void Start()
        {
            float step_size = 0.001f;

            var devices = Microphone.devices;

            foreach (var device in devices)
            {
                Debug.Log(device);
            }
            
            _lmsAdaptiveFilter = new LMSAdaptiveFilter(step_size: step_size, filter_coeffs_size: frame_size);

            mic_data_buffer = new float[16000];
            mic = Microphone.Start("³Á§J­· (3- WordForum USB)", true, 1, 16000);

            AudioSource audioSource = GetComponent<AudioSource>();
            audioSource.clip = mic;
            audioSource.loop = true;

            while (!(Microphone.GetPosition(null) > 0)) { }

            audioSource.Play();

            NDArray nDArray = new NDArray(shape: (2,3), dtype: typeof(float));

            Debug.Log(nDArray.shape[0] +", "+ nDArray.shape[1]);
        }

        void OnAudioFilterRead(float[] data, int channels)
        {
            //Debug.Log("Channel " + channels + ", Data count " + data.Length);

            // Get the process buffer
            //Array.Copy(data, 0, process_frame_buffer, 0, frame_size);

            //int remaining_count = (frame_block_index) - (process_block_num - 1);

            //if (remaining_count < 0)
            //{
            //    Debug.Log(remaining_count);
            //    // Backward, remaining_count should be a negative number
            //    //Array.Copy(frame_history_buffer, (frame_block_num - 1 + remaining_count) * frame_size,
            //    //           process_frame_buffer, frame_size,
            //    //           length: frame_size * (remaining_count * -1));

            //    //// Onward
            //    //Array.Copy(frame_history_buffer, 0,
            //    //           process_frame_buffer, frame_size,
            //    //           length: frame_size * (process_block_num - 1 + remaining_count));
            //    Array.Copy(frame_history_buffer, 0,
            //               process_frame_buffer, frame_size,
            //               length: frame_size * (process_block_num - 1 + remaining_count));
            //}
            //else
            //{
            //    Array.Copy(sourceArray: frame_history_buffer, sourceIndex: remaining_count * frame_size,
            //               destinationArray: process_frame_buffer, destinationIndex: frame_size,
            //               length: frame_size * (process_block_num - 1));
            //}

            if (frame_block_index == 0)
            {
                Array.Copy(frame_history_buffer, (frame_block_num - 1) * frame_size, process_frame_buffer, 0, frame_size);
            }
            else
            {
                Array.Copy(frame_history_buffer, (frame_block_index - 1) * frame_size, process_frame_buffer, 0, frame_size);
            }

            // Store current frame buffer to history
            Array.Copy(data, 0, frame_history_buffer, frame_block_index * frame_size, frame_size);

            frame_block_index = (frame_block_index + 1) % frame_block_num;

            // LMS Let go
            _lmsAdaptiveFilter.PointSignalFilter(ref data, new System.Numerics.Vector<float>(process_frame_buffer));
        }
    }
}
