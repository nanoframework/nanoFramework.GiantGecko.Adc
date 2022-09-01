////
// Copyright (c) .NET Foundation and Contributors
// See LICENSE file in the project root for full license information.
////

namespace nanoFramework.GiantGecko.Adc
{
    /// <summary>
    /// Sample resolution.
    /// </summary>
    [System.Flags]
    public enum SampleResolution
    {
        /// <summary>
        /// 12 bit sampling.
        /// </summary>
        _12bits,

        /// <summary>
        /// 8 bit sampling.
        /// </summary>
        _8bit,

        /// <summary>
        /// 6 bit sampling.
        /// </summary>
        _6bit,

        /// <summary>
        /// Oversampling.
        /// </summary>
        Oversampling
    }
}
