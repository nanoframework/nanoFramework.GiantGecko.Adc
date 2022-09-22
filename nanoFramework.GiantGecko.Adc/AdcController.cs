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
        internal bool _continuousSamplingStarted;

        private readonly AdcConfiguration _adcConfiguration;
        private AdcChannelConfiguration _adcChannelConfiguration;

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
        public AdcConfiguration AcdConfiguration => _adcConfiguration;

        /// <summary>
        /// Gets an array with the last samples from an ongoing scan operation.
        /// </summary>
        /// <exception cref="InvalidOperationException">The ADC is not performing a scan operation. This has to be started with a call to <see cref="StartContinuousSampling"/> or <see cref="StartAveragedContinuousSampling"/>.</exception>
        /// <remarks>
        /// <para>
        /// The values are either the last sample value (if started with <see cref="StartContinuousSampling"/>) or the average of the last samples count (if the averaged continuous scan was started with <see cref="StartAveragedContinuousSampling"/>). 
        /// The array is indexed by the position of the channel in the array passed to <see cref="StartContinuousSampling"/> or <see cref="StartAveragedContinuousSampling"/>. For example, if <see cref="StartContinuousSampling"/>([channel_idda, channel_idd3_3]) was called, then <see cref="LastContinuousSamples">[0]</see> would have value for channel_idda and <see cref="LastContinuousSamples"/>[1] would have value for channel_idd3_3. The last continuous sample for a channel may also be retrieved from <see cref="AdcChannel.LastContinuousValue"/>.
        /// </para>
        /// <para>
        /// Please see remarks on <see cref="StartContinuousSampling"/> and <see cref="StartAveragedContinuousSampling"/> for more information on continuous sampling.
        /// </para>
        /// </remarks>
        public int[] LastContinuousSamples
        {
            get
            {
                CheckIfContinuousSamplingIsStarted();

                return NativeGetLastContinuousSamples();
            }
        }

        /// <summary>
        /// Returns <see langword="true"/> if the ADC is currently running in scan mode, started via <see cref="StartContinuousSampling"/> or <see cref="StartAveragedContinuousSampling"/>. <see langword="false"/> otherwise.
        /// </summary>
        public bool IsContinuousSamplingRunning => _continuousSamplingStarted;

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
                _adcConfiguration = acdInitialization;

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
        /// Starts continuous sampling on the specified channels using the default <see cref="AdcChannelConfiguration"/>.
        /// </summary>
        /// <param name="channels">Array of channels indexes to scan performing continuous sampling.</param>
        /// <exception cref="InvalidOperationException">If a previous continuous sampling operation has been started previously without being stopped.</exception>
        /// <exception cref="ArgumentException">If the specified channel index does not exist.</exception>
        public void StartContinuousSampling(int[] channels)
        {
            StartContinuousSampling(
                channels,
                new AdcChannelConfiguration());
        }

        /// <summary>
        /// Starts continuous sampling on the specified channels using the specified <see cref="AdcChannelConfiguration"/>.
        /// </summary>
        /// <param name="channels">Array of channels indexes to scan performing continuous sampling.</param>
        /// <param name="configuration">Initial configuration for the various ADC channels.</param>
        /// <exception cref="InvalidOperationException">If a previous continuous sampling operation has been started previously without being stopped.</exception>
        /// <exception cref="ArgumentException">If the specified channel index does not exist.</exception>
        public void StartContinuousSampling(
            int[] channels,
            AdcChannelConfiguration configuration)
        {
            CheckIfContinuousSamplingIsStarted();

            _adcChannelConfiguration = configuration;

            // set average count to 1 for single sample
            // flag is updated in native code upon successful start
            NativeStartContinuousConversion(
                            channels,
                            1);
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
        /// <exception cref="ArgumentException">If the specified channel index does not exist.</exception>
        public void StartAveragedContinuousSampling(
            int[] channels,
            int count)
        {
            StartAveragedContinuousSampling(
                channels,
                new AdcChannelConfiguration(),
                count);
        }

        /// <summary>
        /// Starts continuous sampling and average the digital representation of <paramref name="count"/> analog values read from the ADC.
        /// </summary>
        /// <remarks>
        /// In this mode, the last count samples are averaged and made available in LastScanConversion[0].
        /// </remarks>
        /// <param name="channels">Array of channels to scan performing continuous sampling.</param>
        /// <param name="configuration">Initial configuration for the various ADC channels.</param>
        /// <param name="count">Number of samples to take for averaging.</param>
        /// <returns><see langword="true"/> if the continuous sampling was successfully started. <see langword="false"/> otherwise.</returns>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="ArgumentException">If the specified channel index does not exist.</exception>
        public void StartAveragedContinuousSampling(
            int[] channels,
            AdcChannelConfiguration configuration,
            int count)
        {
            CheckIfContinuousSamplingIsStarted();

            _adcChannelConfiguration = configuration;

            // set average count to 1 for single sample
            // flag is updated in native code upon successful start
            NativeStartContinuousConversion(
                channels,
                count);
        }

        /// <summary>
        /// Stops an ongoing continuous sampling operation.
        /// </summary>
        /// <exception cref="InvalidOperationException">If there is no ongoing continuous sampling operation.</exception>
        public void StopContinuousSampling()
        {
            CheckIfContinuousSamplingIsStarted(true);

            // flag is updated in native code upon successful start
            NativeStopContinuousConversion();
        }

        private void CheckIfContinuousSamplingIsStarted(bool invertCheck = false)
        {
            if (invertCheck ? !_continuousSamplingStarted : _continuousSamplingStarted)
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
        private extern void NativeStartContinuousConversion(
            int[] channels,
            int count);

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void NativeStopContinuousConversion();

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern int[] NativeGetLastContinuousSamples();

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern int NativeGetLastScanSampleForChannel(int channel);

        #endregion
    }
}
