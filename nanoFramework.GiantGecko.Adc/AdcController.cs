////
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
        public override SampleResolution SupportedResolutionsInBits
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
        /// Gets an array with the last conversions from an ongoing scan operation.
        /// </summary>
        /// <exception cref="InvalidOperationException">The ADC is not performing a scan operation. This as to be started with a call to <see cref="StartContinuousConversion"/> or <see cref="StartAveragedContinuousConversion"/>.</exception>
        /// <remarks>The values are either the last conversion (if started with <see cref="StartContinuousConversion"/>) or the average of the last conversion count (if the averaged continuous scan was started with <see cref="StartAveragedContinuousConversion"/>).</remarks>
        public int[] LastScanConversions
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
        }

        /// <summary>
        /// Returns true if the ADC is currently running in scan mode, started via <see cref="StartContinuousConversion"/> or <see cref="StartAveragedContinuousConversion"/>.
        /// </summary>
        public bool IsScanRunning => throw new NotImplementedException();

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
        /// Starts continuous conversions on the specified channels.
        /// </summary>
        /// <param name="channels">Array of channels to scan performing continuous conversions.</param>
        /// <returns><see langword="true"/> if the operation was successful. <see langword="false"/> otherwise.</returns>
        /// <exception cref="InvalidOperationException">If a previous continuous conversion operation has been started previously without being stopped.</exception>
        public bool StartContinuousConversion(AdcChannel[] channels)
        {
            if (_continuousConvertionstarted)
            {
                throw new InvalidOperationException();
            }

            _scanChannels = channels;
            // set average count to 1 for single sample
            _averageCount = 1;

            // update flag
            _continuousConvertionstarted = NativeStartContinuousConversion();

            return _continuousConvertionstarted;
        }

        /// <summary>
        /// Starts continuous conversions and average the digital representation of <paramref name="count"/> analog values read from the ADC.
        /// </summary>
        /// <remarks>
        /// In this mode, the last count samples are averaged and made available in LastScanConversion[0].
        /// </remarks>
        /// <param name="channels">Array of channels to scan performing continuous conversions.</param>
        /// <param name="count">Number of samples to take for averaging.</param>
        /// <returns>TODO</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public bool StartAveragedContinuousConversion(AdcChannel[] channels, int count)
        {
            if (_continuousConvertionstarted)
            {
                throw new InvalidOperationException();
            }

            _scanChannels = channels;
            _averageCount = count;

            // update flag
            _continuousConvertionstarted = NativeStartContinuousConversion();

            return _continuousConvertionstarted;
        }

        /// <summary>
        /// Stops an ongoing continuous conversion scan operation.
        /// </summary>
        /// <exception cref="InvalidOperationException">If there is no ongoing continuous conversion operation.</exception>
        public void StoptContinuousConversion()
        {
            if (!_continuousConvertionstarted)
            {
                throw new InvalidOperationException();
            }

            NativeStoptContinuousConversion();

            // update flag
            _continuousConvertionstarted = false;

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
        private extern SampleResolution NativeGetSupportedResolutionsInBits();

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern bool NativeStartContinuousConversion();

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern bool NativeStoptContinuousConversion();

        #endregion
    }
}
