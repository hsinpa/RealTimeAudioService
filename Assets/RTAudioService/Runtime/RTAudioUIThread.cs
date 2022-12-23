using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Hsinpa.RTAudioService {
    public class RTAudioUIThread : MonoBehaviour
    {
        #region Inspector
        [SerializeField]
        private AudioSource microAudioSource;

        int m_frequency = 44100; //Default => 44100 * 0.25, cause i only care about speed
        public int frequency => this.m_frequency;

        private bool m_is_playing = false;

        public const int STREAMING_SAMPLE = 1024;
        private float[] m_sample_rates = new float[STREAMING_SAMPLE];

        private const int m_second_length  = 1;
        private int m_record_time;
        private string m_device_name;

        public System.Action<float[]> OnAudioDataReceived;
        #endregion

        #region Public API
        public void SetFrequency(int frequency) {
            m_frequency = frequency;
        }

        public string[] GetDevices() {
            return Microphone.devices;
        }

        public void PlayDevice(string device_name) {
            this.m_device_name = device_name;
            if (microAudioSource == null) return;

            microAudioSource.clip = Microphone.Start(device_name, true, m_second_length, m_frequency);
            microAudioSource.loop = true;
            microAudioSource.Play();

            //m_sample_rates = new float[m_frequency * microAudioSource.clip.channels];

            string info = $"Frequency {microAudioSource.clip.frequency}, Channel {microAudioSource.clip.channels}, Sample {microAudioSource.clip.samples}";
            Debug.Log(info);

            m_is_playing = true;
        }

        public void Stop() {
            m_is_playing = false;
        }
        #endregion

        #region Monobehavior
        private void Update()
        {
            if (this.gameObject == null || microAudioSource == null || microAudioSource.clip == null || !m_is_playing) return;

            //if (m_record_time < Time.time) {

                //Debug.Log(Microphone.GetPosition(this.m_device_name));

                int micPosition = Microphone.GetPosition(this.m_device_name) - (STREAMING_SAMPLE + 1); // null means the first microphone

                if (micPosition < 0) return;

                microAudioSource.clip.GetData(m_sample_rates, micPosition);

                m_sample_rates[0] = micPosition; 

                if (OnAudioDataReceived != null)
                    OnAudioDataReceived(this.m_sample_rates);

                m_record_time += m_second_length;
           // }
        }
        #endregion

    }

}
