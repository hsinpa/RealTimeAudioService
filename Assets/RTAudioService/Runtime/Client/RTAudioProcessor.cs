using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RTAudioProcessor : System.IDisposable
{
    private int m_mic_position;

    private AudioClip m_audoclip;

    public void SetAudioClip(AudioClip audioClip) {
        this.m_audoclip = audioClip;
        this.m_mic_position = 0;

        string info = $"RTAudioProcessor Frequency {audioClip.frequency}, Channel {audioClip.channels}, Sample {audioClip.samples}";
        Debug.Log(info);
    }

    public bool AppendData(float[] data, int mic_position) {

        if (this.m_mic_position == mic_position) return false;
        int mic_position_offset = mic_position - this.m_mic_position;

        //Data circular Loop occur
        //if (mic_position < m_mic_position)
        //{
        //    Debug.Log("Mic Position " + m_mic_position);
        //    m_audoclip.SetData(data, this.m_mic_position);
        //}

        m_audoclip.SetData(data, 0);

        this.m_mic_position = mic_position;

        return true;
    }

    public void Dispose() {
        this.m_mic_position = 0;
        this.m_audoclip = null;
    }

    

}
