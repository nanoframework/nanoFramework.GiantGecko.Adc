﻿////
// Copyright (c) .NET Foundation and Contributors
// See LICENSE file in the project root for full license information.
////

using System;
using System.Device.Adc;
using System.Runtime.CompilerServices;

namespace nanoFramework.GiantGecko.Adc
{
    /// <summary>
    /// Represents an <see cref="AdcController"/> on the system.
    /// </summary>
    /// <remarks>
    /// This class implements specifics of the Silabs Giant Gecko EFM32 ADC.
    /// It's meant to be used instead of the standard System.Adc.
    /// </remarks>
    public class AdcController : AdcControllerBase
    {
        // this is used as the lock object 
        // a lock is required because multiple threads can access the AdcController
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private readonly object _syncLock;
        private AdcChannel[] _scanChannels;
        private int _averageCount;
        private bool _continuousConvertionstarted;

        private readonly AdcConfiguration _acdConfiguration;

        /// <inheritdoc/>
        public override int ChannelCount
        {
            get
            {
                return NativeGetChannelCount();
            }
        }

        /// <inheritdoc/>
        public override SampleResolution[] SupportedResolutionsInBits
        {
            get
            {
                return NativeGetSupportedResolutionsInBits();
            }
        }

        /// <summary>
        /// Initialization configuration for <see cref="AdcController"/>.
        /// </summary>
        public AdcConfiguration AcdConfiguration => _acdConfiguration;

        /// <summary>
        /// Gets an array with the last samples from an ongoing scan operation.
        /// </summary>
        /// <exception cref="InvalidOperationException">The ADC is not performing a scan operation. This has to be started with a call to <see cref="StartContinuousSampling"/> or <see cref="StartAveragedContinuousSampling"/>.</exception>
        /// <remarks>The values are either the last sample value (if started with <see cref="StartContinuousSampling"/>) or the average of the last samples count (if the averaged continuous scan was started with <see cref="StartAveragedContinuousSampling"/>).</remarks>
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
        }

        /// <summary>
        /// Returns <see langword="true"/> if the ADC is currently running in scan mode, started via <see cref="StartContinuousSampling"/> or <see cref="StartAveragedContinuousSampling"/>. <see langword="false"/> otherwise.
        /// </summary>
        public bool IsScanRunning => _continuousConvertionstarted;

        /// <summary>
        /// Initializes a new instance of the <see cref="AdcController"/> class. 
        /// A default <see cref="AdcConfiguration"/> is used.
        /// </summary>
        /// <returns>
        /// The <see cref="AdcController"/> for the system.
        /// </returns>
        /// <exception cref="InvalidOperationException">If the <see cref="AdcController"/> has already been instantiated.</exception>
        public AdcController() : this(new AdcConfiguration())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AdcController"/> class. 
        /// </summary>
        /// <param name="acdInitialization">Initialization configuration for the <see cref="AdcController"/>.</param>
        /// <exception cref="InvalidOperationException">If the <see cref="AdcController"/> has already been instantiated.</exception>
        public AdcController(AdcConfiguration acdInitialization)
        {
            // check if this device is already opened
            if (_syncLock == null)
            {
                _acdConfiguration = acdInitialization;

                // call native init to allow HAL/PAL inits related with ADC hardware
                // this is also used to check if the requested ADC actually exists
                NativeInit();

                _syncLock = new object();
            }
            else
            {
                // this controller already exists: throw an exception
                throw new InvalidOperationException();
            }
        }

        /// <inheritdoc/>   
        public override AdcChannel OpenChannel(int channelNumber)
        {
            return OpenChannel(channelNumber, new AdcChannelConfiguration());
        }

        /// <inheritdoc/>
        public override AdcChannel OpenChannel(
            int channelNumber,
            AdcChannelConfiguration configuration)
        {
            NativeOpenChannel(channelNumber);

            return new AdcChannel(this, channelNumber, configuration);
        }

        /// <summary>
        /// Starts continuous sampling on the specified channels.
        /// </summary>
        /// <param name="channels">Array of channels to scan performing continuous sampling.</param>
        /// <returns><see langword="true"/> if the operation was successful. <see langword="false"/> otherwise.</returns>
        /// <exception cref="InvalidOperationException">If a previous continuous sampling operation has been started previously without being stopped.</exception>
        public bool StartContinuousSampling(AdcChannel[] channels)
        {
            CheckIfContinuousConversionIsStarted();

            _scanChannels = channels;
            // set average count to 1 for single sample
            _averageCount = 1;

            // update flag
            _continuousConvertionstarted = NativeStartContinuousConversion();

            return _continuousConvertionstarted;
        }

        /// <summary>
        /// Starts continuous sampling and average the digital representation of <paramref name="count"/> analog values read from the ADC.
        /// </summary>
        /// <remarks>
        /// In this mode, the last count samples are averaged and made available in LastScanConversion[0].
        /// </remarks>
        /// <param name="channels">Array of channels to scan performing continuous sampling.</param>
        /// <param name="count">Number of samples to take for averaging.</param>
        /// <returns><see langword="true"/> if the continuous sampling was successfully started. <see langword="false"/> otherwise.</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public bool StartAveragedContinuousSampling(AdcChannel[] channels, int count)
        {
            CheckIfContinuousConversionIsStarted();

            _scanChannels = channels;
            _averageCount = count;

            // update flag
            _continuousConvertionstarted = NativeStartContinuousConversion();

            return _continuousConvertionstarted;
        }

        /// <summary>
        /// Stops an ongoing continuous sampling operation.
        /// </summary>
        /// <exception cref="InvalidOperationException">If there is no ongoing continuous sampling operation.</exception>
        public void StopContinuousSampling()
        {
            CheckIfContinuousConversionIsStarted(true);

            NativeStoptContinuousConversion();

            // update flag
            _continuousConvertionstarted = false;

        }


        private void CheckIfContinuousConversionIsStarted(bool invertCheck = false)
        {
            if (invertCheck ? !_continuousConvertionstarted : _continuousConvertionstarted)
            {
                throw new InvalidOperationException();
            }
        }

        #region Native Calls

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void NativeInit();

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void NativeOpenChannel(int channelNumber);

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern int NativeGetChannelCount();

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern bool NativeIsChannelModeSupported(int mode);

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern SampleResolution[] NativeGetSupportedResolutionsInBits();

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern bool NativeStartContinuousConversion();

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern bool NativeStoptContinuousConversion();

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern int[] NativeGetLastScanSamples();

        #endregion
    }
}
