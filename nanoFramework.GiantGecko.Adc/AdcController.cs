////
// Copyright (c) .NET Foundation and Contributors
// See LICENSE file in the project root for full license information.
////

using System;
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
    public class AdcController
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
        private InputOption _inputOption;
        private bool _prsEnable;
        private readonly AcdInitialization _acdInitialization;

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
        /// <para>If <see cref="InputOption"/> is <see cref="InputOption.SingleEnded"/>, use logical combination of ADC_SCANCTRL_INPUTMASK_CHx defines.</para>
        /// <para>If <see cref="InputOption"/> is <see cref="InputOption.Differential"/>, use logical combination of ADC_SCANCTRL_INPUTMASK_CHxCHy defines.</para>
        /// </remarks>
        public uint ScanInput { get => _scanInput; set => _scanInput = value; }

        /// <summary>
        /// Select if single-ended or differential input.
        /// </summary>
        public InputOption InputOption { get => _inputOption; set => _inputOption = value; }

        /// <summary>
        /// Peripheral reflex system trigger enable.
        /// </summary>
        public bool PrsEnable { get => _prsEnable; set => _prsEnable = value; }

        /// <summary>
        /// Select if left adjustment should be done.
        /// </summary>
        public bool LedfAdjust;

        /// <summary>
        /// Select if continuous conversion until explicit stop.
        /// </summary>
        public bool ContinuousConversion;

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

        /// <summary>
        /// Get a single conversion result from an ADC channel.
        /// </summary>
        /// <returns>Single conversion data.</returns>
        /// <remarks>
        /// <para>
        /// Need to set various configuration properties before getting a conversion. If not, the default values will be used.
        /// </para>
        /// <para>
        /// </para>
        /// <para>
        /// For other references, the calibration is updated with values defined during manufacturing. 
        /// </para>
        /// <para>
        /// For ADC architectures with the ADCn->SCANINPUTSEL register, use ScanSingleEndedInputAdd() to configure single-ended scan inputs or ScanDifferentialInputAdd() to configure differential scan inputs. ADC_ScanInputClear() is also provided for applications that need to update the input configuration.
        /// </para>
        /// </remarks>
        public int GetConversion(int channel)
        {
            return GetAveragedConversion(channel, 1);
        }

        /// <summary>
        /// Get an array of conversion results from an ADC channel.
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="count"></param>
        /// <returns>Average of the number of conversions taken.</returns>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern int GetAveragedConversion(int channel, int count);

        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern bool StartContinuousConversion();

        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern bool StartAveragedContinuousConversion(int count);

        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void StoptContinuousConversion();

        #region Native Calls

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void NativeInit();

        #endregion
    }
}
