////
// Copyright (c) .NET Foundation and Contributors
// See LICENSE file in the project root for full license information.
////

using System.Device.Adc;

namespace nanoFramework.GiantGecko.Adc
{
    /// <summary>
    /// ADC Channel configuration, common for single conversion and scan sequence.
    /// </summary>
    public class AdcChannelConfiguration
    {
        private PrsSampleTrigger _prsSampleTrigger = PrsSampleTrigger.Disabled;
        private AquisitionTime _aquisitionTime = AquisitionTime._8Cyles;
        private ReferenceVoltage _referenceVoltage = ReferenceVoltage.Internal1_25V;
        private SampleResolution _sampleResolution = SampleResolution._12bits;
        private AdcChannelMode _channelMode = AdcChannelMode.SingleEnded;

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
        /// Gets or sets the channel mode for the <see cref="AdcChannel"/>.
        /// </summary>
        /// <value>
        /// The mode for the <see cref="AdcChannel"/>.
        /// </value>
        public AdcChannelMode ChannelMode { get => _channelMode; set => _channelMode = value; }

        /// <summary>
        /// Peripheral reflex system trigger enable.
        /// </summary>
        public bool IsPrsEnabled => _prsSampleTrigger > PrsSampleTrigger.Disabled;
    }
}
