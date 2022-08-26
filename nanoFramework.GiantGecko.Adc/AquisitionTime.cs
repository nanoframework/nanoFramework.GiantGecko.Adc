////
// Copyright (c) .NET Foundation and Contributors
// See LICENSE file in the project root for full license information.
////

namespace nanoFramework.GiantGecko.Adc
{
    /// <summary>
    /// Acquisition time (in ADC clock cycles).
    /// </summary>
    public enum AquisitionTime
    {
        /// <summary>
        /// 1 clock cycle.
        /// </summary>
        _1Cyle = 1,

        /// <summary>
        /// 2 clock cycles.
        /// </summary>
        _2Cyles,

        /// <summary>
        /// 3 clock cycles.
        /// </summary>
        _3Cyles,

        /// <summary>
        /// 4 clock cycles.
        /// </summary>
        _4Cyles,

        /// <summary>
        /// 8 clock cycless.
        /// </summary>
        _8Cyles,

        /// <summary>
        /// 16 clock cycles.
        /// </summary>
        _16Cyles,

        /// <summary>
        /// 32 clock cycles.
        /// </summary>
        _32Cyles,

        /// <summary>
        /// 64 clock cycles.
        /// </summary>
        _64Cyles,

        /// <summary>
        /// 128 clock cycles.
        /// </summary>
        _128Cyles,

        /// <summary>
        /// 256 clock cycles.
        /// </summary>
        _256Cyles
    }
}
