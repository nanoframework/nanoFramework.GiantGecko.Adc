//
// Copyright (c) .NET Foundation and Contributors
// See LICENSE file in the project root for full license information.
//

namespace System.Device.Adc
{
    /// <summary>
    /// Describes the channel modes that the <see cref="AdcController"/> can use for input.
    /// </summary>
    public enum AdcChannelMode
    {
        /// <summary>
        /// Simple value of a particular pin.
        /// </summary>
        SingleEnded = 0,
        /// <summary>
        /// Difference between two pins.
        /// </summary>
        Differential
    }
}
