////
// Copyright (c) .NET Foundation and Contributors
// See LICENSE file in the project root for full license information.
////

using System;

namespace nanoFramework.GiantGecko.Adc
{
    /// <summary>
    /// Single and scan mode voltage references.
    /// </summary>
    public enum ReferenceVoltage
    {
        /// <summary>
        /// Internal 1.25 V reference.
        /// </summary>
        Internal1_25V,

        /// <summary>
        /// Internal 2.5 V reference.
        /// </summary>
        Internal2_5V,

        /// <summary>
        /// Buffered VDD.
        /// </summary>
        BufferedVdd,

        /// <summary>
        /// Internal differential 5 V reference.
        /// </summary>
        InternalDifferencial_5V,

        /// <summary>
        /// Single-ended external reference from pin 6.
        /// </summary>
        SingleEndedExternalPin6,

        /// <summary>
        /// Differential external reference from pin 6 and 7.
        /// </summary>
        DiffExternalPin6And7,

        /// <summary>
        /// Unbuffered 2xVDD.
        /// </summary>
        Unbuffered2Vdd,
    }
}
