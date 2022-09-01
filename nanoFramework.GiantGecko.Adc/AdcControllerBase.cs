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
        /// Opens a connection to the specified ADC channel with the specified configurations.
        /// </summary>
        /// <param name="channelNumber">
        /// The channel to connect to.
        /// </param>
        /// <returns>
        /// The ADC channel.
        /// </returns>
        public abstract AdcChannel OpenChannel(int channelNumber, AdcChannelConfiguration configuration);

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
        /// <exception cref="PlatformNotSupportedException">If the platform doesn't have this configuration available.</exception>
        public abstract AdcChannelMode ChannelMode { get; set; }

        /// <summary>
        /// Gets the resolution(s) of the controller as number of bits it has. For example, if we have a 10-bit ADC, that means it can detect 1024 (2^10) discrete levels.
        /// </summary>
        /// <value>
        /// The number of bits the <see cref="AdcController"/> has support for.
        /// </value>
        public abstract SampleResolution SupportedResolutionsInBits { get; }

        /// <summary>
        /// Gets or sets the resolution of the controller as number of bits it has.
        /// </summary>
        /// <value>
        /// The number of bits the <see cref="AdcController"/> will use to perform conversions.
        /// </value>
        /// <exception cref="InvalidOperationException">If trying to change the resolution when a continuous conversion operation has been started.</exception>
        public abstract SampleResolution ResolutionInBits { get; set; }
    }
}
