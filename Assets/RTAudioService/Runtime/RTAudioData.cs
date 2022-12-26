using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Hsinpa.RTAudioService
{
    public class RTAudioData
    {
        private List<float> m_data;
        private int m_data_length;

        private int m_grab_index;
        private int m_insert_index;

        private int m_sample_rate;
        private int m_total_sample_lens;

        private int m_round_append = 0;
        private int m_round_grab = 0;

        private long m_mic_full_index => (m_total_sample_lens * m_round_append) + this.m_insert_index;
        private long m_grab_full_index => (m_total_sample_lens * m_round_grab) + this.m_grab_index;

        public RTAudioData(int sample_rate, int segment_lens) {
            this.m_total_sample_lens = sample_rate * segment_lens;

            this.m_data = new List<float>(Enumerable.Repeat(0f, m_total_sample_lens));
            Debug.Log($"Data Length : {this.m_data.Count}");
        }

        public void AppendData(float[] raw_data) {
            int data_lens = raw_data.Length;
            int insert_index = this.m_insert_index;

            for (int i = 0; i < data_lens; i++) {
                int index = i + insert_index;


                if (index >= this.m_total_sample_lens) {
                    //Debug.Log("Out index " + index);
                    index = index - this.m_total_sample_lens;
                }

                m_data[index] = raw_data[i];
            }

            //m_data.InsertRange(insert_index, raw_data);

            this.m_insert_index = (insert_index + data_lens) % m_total_sample_lens;

           // Debug.Log($"m_insert_index : {m_insert_index}");


            if (insert_index > this.m_insert_index)
                m_round_append++;

            //this.m_grab_index = insert_index;
            //Debug.Log($"OnMicAudioReceive Original Index : {insert_index}, Post Index : {this.m_insert_index}, Count {m_data.Count}");
        }

        public void GrabData(ref float[] mute_array) {
            int data_lens = mute_array.Length;

            if (data_lens > m_data.Count) return;
            //Debug.Log($"GrabData Index : {m_grab_index}, Post Index : {m_grab_index + data_lens}");
            //Debug.Log($"Round data Grab : {m_round_grab}, Append : {m_round_append}");

            if (this.m_grab_index + data_lens > m_total_sample_lens) {
                int first_part_lens = m_total_sample_lens - this.m_grab_index;
                int second_part_lens = (this.m_grab_index + data_lens) - m_total_sample_lens;

                m_data.CopyTo(m_grab_index, mute_array, 0, first_part_lens);
                m_data.CopyTo(0, mute_array, first_part_lens, second_part_lens);

                this.m_grab_index = (this.m_grab_index + data_lens) % m_total_sample_lens;

                //Debug.Log($"GrabData First part : {first_part_lens}, Second part : {second_part_lens}");
                m_round_grab++;

                Debug.Log($"m_grab_full_index : {m_grab_full_index}, grab round : {m_round_grab}, m_mic_full_index : {m_mic_full_index}");
                return;
            } 

            m_data.CopyTo(m_grab_index, mute_array, 0, data_lens);

            this.m_grab_index = (this.m_grab_index + data_lens) % m_total_sample_lens;
        }
    }
}