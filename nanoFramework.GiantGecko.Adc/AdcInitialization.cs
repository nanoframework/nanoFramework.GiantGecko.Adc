////
// Copyright (c) .NET Foundation and Contributors
// See LICENSE file in the project root for full license information.
////

namespace nanoFramework.GiantGecko.Adc
{
    /// <summary>
    /// ADC initialization configuration, common for single conversion and scan sequence.
    /// </summary>
    public class AdcInitialization
    {
        private OversampleRate _oversampleRate = OversampleRate._2Samples;
        private LowpassFilterMode _lowpassFilterMode = LowpassFilterMode.Bypass;
        private WarmUpMode _warmUpMode = WarmUpMode.Normal;
        private uint _warnupTimeBase = 1;
        private uint _prescale = 0;
        private bool _tailgating = false;

        /// <summary>
        /// Oversampling rate select.
        /// </summary>
        /// <remarks>
        /// To have any effect, oversampling must be enabled for single/scan mode.
        /// </remarks>
        public OversampleRate OversampleRate { get => _oversampleRate; set => _oversampleRate = value; }

        /// <summary>
        /// Low-pass or decoupling capacitor filter.
        /// </summary>
        public LowpassFilterMode LowpassFilterMode { get => _lowpassFilterMode; set => _lowpassFilterMode = value; }

        /// <summary>
        /// ADC Warm-up mode.
        /// </summary>
        public WarmUpMode WarmUpMode { get => _warmUpMode; set => _warmUpMode = value; }

        /// <summary>
        /// Timebase for ADC warm up.
        /// </summary>
        /// <remarks>
        /// Select N to give (N+1) HFPERCLK / HFPERCCLK cycles. (Additional delay is added for bandgap references. See the reference manual for more information.) 
        /// Normally, N should be selected so that the timebase is at least 1us. See ADC_TimebaseCalc() to obtain a suggested timebase of, at least, 1 us.
        /// </remarks>
        public uint WarnupTimeBase { get => _warnupTimeBase; set => _warnupTimeBase = value; }

        /// <summary>
        /// Clock division factor N, ADC clock = (HFPERCLK or HFPERCCLK) / (N + 1).
        /// </summary>
        public uint Prescale { get => _prescale; set => _prescale = value; }

        /// <summary>
        /// Enable/disable conversion tailgating.
        /// </summary>
        public bool Tailgating { get => _tailgating; set => _tailgating = value; }
    }
}
