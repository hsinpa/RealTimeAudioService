using NumSharp;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEditor.PackageManager;
using UnityEngine;

namespace Hsinpa.AdaptiveFilter
{
    public class LMSAdaptiveFilter
    {
        private float _step_size;
        private int _filter_size;

        private Vector<float> _filter_coeffs;

        public LMSAdaptiveFilter(float step_size, int filter_coeffs_size)
        {
            this._step_size = step_size;
            this._filter_size = filter_coeffs_size;
            this._filter_coeffs = new Vector<float>(new float[filter_coeffs_size]);
        }

        public Vector<float> PointSignalFilter(ref float[] input_signal, Vector<float> reference_signal)
        {               
            for (int i = 0; i < _filter_size; i++)
            {
                var filter = Vector.Dot(this._filter_coeffs, reference_signal);

                input_signal[i] = (input_signal[i] - filter);

                _filter_coeffs += _step_size * input_signal[i] * reference_signal;
            }

            return this._filter_coeffs;
        }
    }
}
