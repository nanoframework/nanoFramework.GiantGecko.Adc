//
// Copyright (c) .NET Foundation and Contributors
// See LICENSE file in the project root for full license information.
//

using nanoFramework.GiantGecko.Adc;

namespace System.Device.Adc
{
    /// <summary>
    /// Base class for <see cref="AdcChannel"/>.
    /// </summary>
    public abstract class AdcChannelBase
    {
        [Diagnostics.DebuggerBrowsable(Diagnostics.DebuggerBrowsableState.Never)]
        internal AdcController _adcController;

        /// <summary>
        /// Reads the digital representation of the analog value from the ADC.
        /// </summary>
        /// <returns>
        /// The digital value.
        /// </returns>
        public abstract int ReadValue();

        /// <summary>
        /// Reads the averaged digital representation of <paramref name="count"/> analog values read from the ADC.
        /// </summary>
        /// <param name="count">Number of samples to take for averaging.</param>
        /// <returns>
        /// The digital value with the averaged value of <paramref name="count"/> samples.
        /// </returns>
        public abstract int ReadValueAveraged(int count);

        /// <summary>
        /// Gets the AdcController for this channel.
        /// </summary>
        /// <value>
        /// The <see cref="AdcController"/>.
        /// </value>
        public AdcController Controller => _adcController;
    }
}
