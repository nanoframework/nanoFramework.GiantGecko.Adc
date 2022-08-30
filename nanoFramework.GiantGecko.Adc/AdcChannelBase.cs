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
        /// Reads the value as a percentage of the max value possible for this controller.
        /// </summary>
        /// <returns>
        /// The value as percentage of the max value.
        /// </returns>
        public double ReadRatio()
        {
            return ReadValue() / (double)Controller.MaxValue;
        }

        /// <summary>
        /// Gets the AdcController for this channel.
        /// </summary>
        /// <value>
        /// The <see cref="AdcController"/>.
        /// </value>
        public AdcController Controller => _adcController;
    }
}
