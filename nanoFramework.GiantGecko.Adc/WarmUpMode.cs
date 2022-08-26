////
// Copyright (c) .NET Foundation and Contributors
// See LICENSE file in the project root for full license information.
////

namespace nanoFramework.GiantGecko.Adc
{
    /// <summary>
    /// Warm-up mode.
    /// </summary>
    public enum WarmUpMode
    {
        /// <summary>
        /// ADC shutdown after each conversion.
        /// </summary>
        Normal,

        /// <summary>
        /// Do not warm up bandgap references.
        /// </summary>
        WarmupFastBG,

        /// <summary>
        /// Reference selected for scan mode kept warm.
        /// </summary>
        KeepScanRefWarm,

        /// <summary>
        /// ADC and reference selected for scan mode kept at warm-up allowing continuous conversion.
        /// </summary>
        KeepAdcWarm
    }
}
