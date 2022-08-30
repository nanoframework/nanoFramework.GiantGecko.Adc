//
// Copyright (c) .NET Foundation and Contributors
// See LICENSE file in the project root for full license information.
//

using nanoFramework.GiantGecko.Adc;

namespace System.Device.Adc
{
    /// <summary>
    /// Base class for <see cref="AdcController"/>.
    /// </summary>
    public abstract class AdcControllerBase
    {
        /// <summary>
        /// Verifies that the specified channel mode is supported by the controller.
        /// </summary>
        /// <param name="channelMode">
        /// The channel mode.
        /// </param>
        /// <returns>
        /// True if the specified channel mode is supported, otherwise false.
        /// </returns>
        public abstract bool IsChannelModeSupported(AdcChannelMode channelMode);

        /// <summary>
        /// Opens a connection to the specified ADC channel.
        /// </summary>
        /// <param name="channelNumber">
        /// The channel to connect to.
        /// </param>
        /// <returns>
        /// The ADC channel.
        /// </returns>
        public abstract AdcChannel OpenChannel(int channelNumber);

        /// <summary>
        /// The number of channels available on the <see cref="AdcController"/>.
        /// </summary>
        /// <value>
        /// Number of channels.
        /// </value>
        public abstract int ChannelCount { get; }

        /// <summary>
        /// Gets or sets the channel mode for the <see cref="AdcController"/>.
        /// </summary>
        /// <value>
        /// The mode for the <see cref="AdcChannel"/>.
        /// </value>
        public abstract AdcChannelMode ChannelMode { get; set; }

        /// <summary>
        /// Gets the maximum value that the <see cref="AdcController"/> can report.
        /// </summary>
        /// <value>
        /// The maximum value.
        /// </value>
        public abstract int MaxValue { get; }

        /// <summary>
        /// The minimum value the <see cref="AdcController"/> can report.
        /// </summary>
        /// <value>
        /// The minimum value.
        /// </value>
        public abstract int MinValue { get; }

        /// <summary>
        /// Gets the resolution of the controller as number of bits it has. For example, if we have a 10-bit ADC, that means it can detect 1024 (2^10) discrete levels.
        /// </summary>
        /// <value>
        /// The number of bits the <see cref="AdcController"/> has.
        /// </value>
        public abstract int ResolutionInBits { get; }
    }
}
