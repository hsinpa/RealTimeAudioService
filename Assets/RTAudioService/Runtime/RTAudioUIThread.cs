using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Hsinpa.RTAudioService {
    public class RTAudioUIThread : MonoBehaviour
    {
        #region Inspector
        [SerializeField]
        private AudioSource microAudioSource;


        public const int FREQUENCY = 11025; //Default => 44100 * 0.25, cause i only care about speed

        private bool m_is_playing = false;

        private const int STREAMING_SAMPLE = 2205;
        private float[] m_sample_rates = new float[STREAMING_SAMPLE];

        private const float m_second_length  = 0.2f;
        private float m_record_time;
        private string m_device_name;

        public System.Action<float[]> OnAudioDataReceived;
        #endregion

        #region Public API

        public string[] GetDevices() {
            return Microphone.devices;
        }

        public void PlayDevice(string device_name) {
            this.m_device_name = device_name;
            if (microAudioSource == null) return;

            microAudioSource.clip = Microphone.Start(device_name, true, 2, FREQUENCY);
            microAudioSource.loop = true;
            microAudioSource.Play();

            Debug.Log("Mic clip samples " + microAudioSource.clip.samples);

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
        private void FixedUpdate()
        {
            if (this.gameObject == null || microAudioSource == null || microAudioSource.clip == null || !m_is_playing) return;

            if (m_record_time <= Time.time) {

                //Debug.Log(Microphone.GetPosition(this.m_device_name));

                int micPosition = Microphone.GetPosition(this.m_device_name) - STREAMING_SAMPLE; // null means the first microphone

                if (micPosition < 0) return;

                microAudioSource.clip.GetData(m_sample_rates, micPosition);

                if (OnAudioDataReceived != null)
                    OnAudioDataReceived(this.m_sample_rates);

                m_record_time = m_second_length + Time.time;
            }
        }
        #endregion

    }

}
