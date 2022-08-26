////
// Copyright (c) .NET Foundation and Contributors
// See LICENSE file in the project root for full license information.
////

namespace nanoFramework.GiantGecko.Adc
{
    /// <summary>
    /// Scan sequence initialization.
    /// </summary>
    public class ScanSequenceInitialization
    {
        private PrsSampleTrigger _prsSampleTrigger;
        private AquisitionTime _aquisitionTime;
        private ReferenceVoltage _referenceVoltage;
        private SampleResolution _sampleResolution;
        private uint _scanInput;
        private InputOption _inputOption;
        private bool _prsEnable;

        /// <summary>
        /// Peripheral reflex system trigger selection.
        /// </summary>
        /// <remarks>
        /// Only applicable if prsEnable is enabled.
        /// </remarks>
        public PrsSampleTrigger PrsSampleTrigger { get => _prsSampleTrigger; set => _prsSampleTrigger = value; }

        /// <summary>
        /// Acquisition time (in ADC clock cycles).
        /// </summary>
        public AquisitionTime AquisitionTime { get => _aquisitionTime; set => _aquisitionTime = value; }

        /// <summary>
        /// Sample reference selection.
        /// </summary>
        /// <remarks>
        /// Note that, for external references, the ADC calibration register must be set explicitly.
        /// </remarks>
        public ReferenceVoltage ReferenceVoltage { get => _referenceVoltage; set => _referenceVoltage = value; }

        /// <summary>
        /// Sample resolution.
        /// </summary>
        public SampleResolution SampleResolution { get => _sampleResolution; set => _sampleResolution = value; }

        /// <summary>
        /// Scan input selection.
        /// </summary>
        /// <remarks>
        /// <para>If <see cref="InputOption"/> is <see cref="InputOption.SingleEnded"/>, use logical combination of ADC_SCANCTRL_INPUTMASK_CHx defines.</para>
        /// <para>If <see cref="InputOption"/> is <see cref="InputOption.Differential"/>, use logical combination of ADC_SCANCTRL_INPUTMASK_CHxCHy defines.</para>
        /// </remarks>
        public uint ScanInput { get => _scanInput; set => _scanInput = value; }

        /// <summary>
        /// Select if single-ended or differential input.
        /// </summary>
        public InputOption InputOption { get => _inputOption; set => _inputOption = value; }

        /// <summary>
        /// Peripheral reflex system trigger enable.
        /// </summary>
        public bool PrsEnable { get => _prsEnable; set => _prsEnable = value; }

        /// <summary>
        /// Select if left adjustment should be done.
        /// </summary>
        public bool LedfAdjust;

        /// <summary>
        /// Select if continuous conversion until explicit stop.
        /// </summary>
        public bool ContinuousConversion;
    }
}
