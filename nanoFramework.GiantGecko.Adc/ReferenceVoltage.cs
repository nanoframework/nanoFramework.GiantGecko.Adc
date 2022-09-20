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
        Internal1_25V = 0,

        /// <summary>
        /// Internal 2.5 V reference.
        /// </summary>
        Internal2_5V = 1,

        /// <summary>
        /// Buffered VDD.
        /// </summary>
        BufferedVdd = 2,

        /// <summary>
        /// Internal differential 5 V reference.
        /// </summary>
        InternalDifferencial_5V = 3,

        /// <summary>
        /// Single-ended external reference from pin 6.
        /// </summary>
        SingleEndedExternalPin6 = 4,

        /// <summary>
        /// Differential external reference from pin 6 and 7.
        /// </summary>
        DiffExternalPin6And7 = 5,

        /// <summary>
        /// Unbuffered 2xVDD.
        /// </summary>
        Unbuffered2Vdd = 6,

        /// <summary>
        /// Internal Bandgap reference. Custom VFS.
        /// </summary>
        InternalBandgap = 128,

        /// <summary>
        /// Scaled AVDD: AVDD * VREFATT.
        /// </summary>
        ScaledAvdd = 129,

        /// <summary>
        /// Scaled singled ended external reference from pin 6: VREFP * VREFATT.
        /// </summary>
        ScaledSingleEndedExternalPin6 = 130,

        /// <summary>
        /// Raw single-ended external reference from pin 6. 
        /// </summary>
        RawSingleEndedExternalPin6 = 131,

        /// <summary>
        /// Special mode for entropy generation. 
        /// </summary>
        EntropyGeneration = 132,

        /// <summary>
        /// Scaled differential external Vref from pin 6 and 7: (VREFP - VREFN) * VREFATT.
        /// </summary>
        ScaledExternalPin6And7 = 133,

        /// <summary>
        /// Raw differential external Vref from pin 6 and 7: VREFP - VREFN.
        /// </summary>
        RawExternalPin6And7 = 134
    }
}
