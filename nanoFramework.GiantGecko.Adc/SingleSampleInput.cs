////
// Copyright (c) .NET Foundation and Contributors
// See LICENSE file in the project root for full license information.
////

using System;

namespace nanoFramework.GiantGecko.Adc
{
    /// <summary>
    /// Single sample input selection.
    /// </summary>
    public enum SingleSampleInput
    {
        /// <summary>
        /// Channel 0.
        /// </summary>
        Channel0,

        /// <summary>
        /// Channel 1.
        /// </summary>
        Channel1,

        /// <summary>
        /// Channel 2.
        /// </summary>
        Channel2,

        /// <summary>
        /// Channel 3.
        /// </summary>
        Channel3,

        /// <summary>
        /// Channel 4.
        /// </summary>
        Channel4,

        /// <summary>
        /// Channel 5.
        /// </summary>
        Channel5,

        /// <summary>
        /// Channel 6.
        /// </summary>
        Channel6,

        /// <summary>
        /// Channel 7.
        /// </summary>
        Channel7,

        /// <summary>
        /// Temperature reference.
        /// </summary>
        TemperatureReference,

        /// <summary>
        /// VDD divided by 3.
        /// </summary>
        VddDiv3,

        /// <summary>
        /// VDD.
        /// </summary>
        Vdd,

        /// <summary>
        /// VSS.
        /// </summary>
        Vss,

        /// <summary>
        /// Vref divided by 2.
        /// </summary>
        VrefDiv2,

        /// <summary>
        /// DAC output 0.
        /// </summary>
        DacOutput0,

        /// <summary>
        /// DAC output 1.
        /// </summary>
        DacOutput1,

        /// <summary>
        /// ATEST.
        /// </summary>
        Atest,

        /// <summary>
        /// Positive Ch0, negative Ch1.
        /// </summary>
        Positive0Negative1,

        /// <summary>
        /// Positive Ch2, negative Ch3.
        /// </summary>
        Positive2Negative3,

        /// <summary>
        /// Positive Ch4, negative Ch5.
        /// </summary>
        Positive4Negative5,

        /// <summary>
        /// Positive Ch6, negative Ch7.
        /// </summary>
        Positive6Negative7,

        /// <summary>
        /// Differential 0.
        /// </summary>
        Differential0,
    }
}
