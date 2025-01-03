using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hsinpa
{
    public class MicUtility 
    {
        
        static public void sample_float_to_short(ref float[] source_samples, ref short[] target_samples)
        {
            int size = source_samples.Length;
            for (int i = 0; i < size; i++)
            {
                // Scale float sample to short range
                float scaled = source_samples[i] * 32767.0f;

                // Clamp before casting
                if (scaled > 32767.0f) scaled = 32767.0f;
                if (scaled < -32768.0f) scaled = -32768.0f;

                // Convert to short
                target_samples[i] = (short)scaled;
            }
        }

        static public void sample_short_to_float(ref short[] source_samples, ref float[] target_samples)
        {
            int size = source_samples.Length;
            for (int i = 0; i < size; i++)
            {
                float sample = source_samples[i] / 32767.0f;
                if (sample < -1.0f) sample = -1.0f;
                if (sample > +1.0f) sample = +1.0f;
                target_samples[i] = sample;
            }
        }

    }
}
