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
    /// This class implements specifics of the Silabs Giant Gecko ADC.
    /// It's meant to be used instead of the standard System.Adc.
    /// </remarks>
    public class AdcController : AdcControllerBase
    {
        // this is used as the lock object 
        // a lock is required because multiple threads can access the AdcController
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private readonly object _syncLock;

        private PrsSampleTrigger _prsSampleTrigger;
        private AquisitionTime _aquisitionTime;
        private ReferenceVoltage _referenceVoltage;
        private SampleResolution _sampleResolution;
        private uint _scanInput;
        private AdcChannelMode _channelMode = AdcChannelMode.SingleEnded;
        private bool _prsEnable;
        private readonly AcdInitialization _acdInitialization;
        private bool _continuousConvertionstarted;

        /// <inheritdoc/>
        public override int ChannelCount
        {
            get
            {
                return NativeGetChannelCount();
            }
        }

        /// <inheritdoc/>
        public override AdcChannelMode ChannelMode { get => _channelMode; set => _channelMode = value; }

        /// <inheritdoc/>
        public override int MaxValue
        {
            get
            {
                return NativeGetMaxValue();
            }
        }

        /// <inheritdoc/>
        public override int MinValue
        {
            get
            {
                return NativeGetMinValue();
            }
        }

        /// <inheritdoc/>
        public override int ResolutionInBits
        {
            get
            {
                return NativeGetResolutionInBits();
            }
        }

        /// <inheritdoc/>
        public override bool IsChannelModeSupported(AdcChannelMode channelMode)
        {
            return NativeIsChannelModeSupported((int)channelMode);
        }

        /// <summary>
        /// Peripheral reflex system trigger selection.
        /// </summary>
        /// <remarks>
        /// Only applicable if prsEnable is enabled.
        /// </remarks>
        public PrsSampleTrigger PrsSampleTrigger { get => _prsSampleTrigger; set => _prsSampleTrigger = value; }

        /// <summary>
        /// Acquisition time (in ADC clock cycles).
        /// </summary>
        public AquisitionTime AquisitionTime { get => _aquisitionTime; set => _aquisitionTime = value; }

        /// <summary>
        /// Sample reference selection.
        /// </summary>
        /// <remarks>
        /// Note that, for external references, the ADC calibration register must be set explicitly.
        /// </remarks>
        public ReferenceVoltage ReferenceVoltage { get => _referenceVoltage; set => _referenceVoltage = value; }

        /// <summary>
        /// Sample resolution.
        /// </summary>
        public SampleResolution SampleResolution { get => _sampleResolution; set => _sampleResolution = value; }

        /// <summary>
        /// Scan input selection.
        /// </summary>
        /// <remarks>
        /// <para>If <see cref="ChannelMode"/> is <see cref="AdcChannelMode.SingleEnded"/>, use logical combination of ADC_SCANCTRL_INPUTMASK_CHx defines.</para>
        /// <para>If <see cref="ChannelMode"/> is <see cref="AdcChannelMode.Differential"/>, use logical combination of ADC_SCANCTRL_INPUTMASK_CHxCHy defines.</para>
        /// </remarks>
        public uint ScanInput { get => _scanInput; set => _scanInput = value; }

        /// <summary>
        /// Peripheral reflex system trigger enable.
        /// </summary>
        public bool PrsEnable { get => _prsEnable; set => _prsEnable = value; }

        /// <summary>
        /// Select if left adjustment should be done.
        /// </summary>
        public bool LedfAdjust;

        /// <summary>
        /// Initialization configuration for <see cref="AdcController"/>.
        /// </summary>
        public AcdInitialization AcdInitialization => _acdInitialization;

        /// <summary>
        /// Gets an array with the last conversions from an ongoing scan operation.
        /// </summary>
        /// <exception cref="InvalidOperationException">The ADC is not performing a scan operation. This as to be started with a call to <see cref="StartContinuousConversion"/> or <see cref="StartAveragedContinuousConversion"/>.</exception>
        /// <remarks>The values are either the last conversion or the average of the last conversion count, if the averaged continuous scan was started with <see cref="StartAveragedContinuousConversion"/>.</remarks>
        public int[] LastConversion
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AdcController"/> class. 
        /// </summary>
        /// <returns>
        /// The <see cref="AdcController"/> for the system.
        /// </returns>
        /// <exception cref="InvalidOperationException">If the <see cref="AdcController"/> has already been instantiated.</exception>
        public AdcController()
        {
            // check if this device is already opened
            if (_syncLock == null)
            {
                _acdInitialization = new AcdInitialization();

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

        /// <summary>
        /// Initializes a new instance of the <see cref="AdcController"/> class. 
        /// </summary>
        /// <param name="acdInitialization">Initialization configuration for the <see cref="AdcController"/>.</param>
        /// <exception cref="InvalidOperationException">If the <see cref="AdcController"/> has already been instantiated.</exception>
        public AdcController(AcdInitialization acdInitialization)
        {
            // check if this device is already opened
            if (_syncLock == null)
            {
                _acdInitialization = acdInitialization;

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
        public override AdcChannel OpenChannel(Int32 channelNumber)
        {
            NativeOpenChannel(channelNumber);

            return new AdcChannel(this, channelNumber);
        }


        /// <summary>
        /// Starts continuous conversions on the specified channels.
        /// </summary>
        /// <returns><see langword="true"/> if the operation was successful. <see langword="false"/> otherwise.</returns>
        /// <exception cref="">If a previous continuous conversion operation has been started previously without being stopped.</exception>
        public bool StartContinuousConversion()
        {
            if(_continuousConvertionstarted)
            {
                throw new InvalidOperationException();
            }

            // update flag
            _continuousConvertionstarted = NativeStartContinuousConversion();

            return _continuousConvertionstarted;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern bool StartAveragedContinuousConversion(int count);

        /// <summary>
        /// Stops an ongoing continuous conversion scan operation.
        /// </summary>
        /// <exception cref="InvalidOperationException">If there is no ongoing continuous conversion operation.</exception>
        public void StoptContinuousConversion()
        {
            if(!_continuousConvertionstarted)
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
        private extern void NativeOpenChannel(Int32 channelNumber);

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern int NativeGetChannelCount();

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern int NativeGetMaxValue();

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern int NativeGetMinValue();

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern bool NativeIsChannelModeSupported(int mode);

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern int NativeGetResolutionInBits();

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern bool NativeStartContinuousConversion();

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern bool NativeStoptContinuousConversion();

        #endregion
    }
}
