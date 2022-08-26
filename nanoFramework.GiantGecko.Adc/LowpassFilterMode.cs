////
// Copyright (c) .NET Foundation and Contributors
// See LICENSE file in the project root for full license information.
////

namespace nanoFramework.GiantGecko.Adc
{
    /// <summary>
    /// Lowpass filter mode.
    /// </summary>
    public enum LowpassFilterMode
    {
        /// <summary>
        /// No filter or decoupling capacitor.
        /// </summary>
        Bypass,

        /// <summary>
        /// On-chip RC filter.
        /// </summary>
        RcFilter,

        /// <summary>
        /// On-chip decoupling capacitor.
        /// </summary>
        DecouplingCapacitor
    }
}
