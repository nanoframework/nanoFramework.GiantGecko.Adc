//
// Copyright (c) .NET Foundation and Contributors
// See LICENSE file in the project root for full license information.
//

using System;
using System.Device.Adc;
using System.Runtime.CompilerServices;

namespace nanoFramework.GiantGecko.Adc
{
    /// <summary>
    /// Represents a single ADC channel.
    /// </summary>
    public class AdcChannel : AdcChannelBase, IDisposable
    {
        // this is used as the lock object 
        // a lock is required because multiple threads can access the channel
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private readonly object _syncLock;

        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private bool _disposed;

#pragma warning disable IDE0052 // Need to be declared to become accessible from native code
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private readonly int _channelNumber;
        private readonly AdcChannelConfiguration _adcChannelConfiguration;
        private int _averageCount;
#pragma warning restore IDE0052 // Remove unread private members

        /// <summary>
        /// Initialization configuration for <see cref="AdcChannel"/>.
        /// </summary>
        public AdcChannelConfiguration AdcChannelConfiguration => _adcChannelConfiguration;

        internal AdcChannel(
            AdcController controller,
            int channelNumber)
        {
            _adcController = controller;
            _channelNumber = channelNumber;
            _adcChannelConfiguration = default;

            _syncLock = new object();
        }

        internal AdcChannel(
            AdcController controller,
            int channelNumber,
            AdcChannelConfiguration acdInitialization)
        {
            _adcController = controller;
            _channelNumber = channelNumber;
            _adcChannelConfiguration = acdInitialization;

            _syncLock = new object();
        }

        /// <inheritdoc/>
        public override int ReadValue()
        {
            lock (_syncLock)
            {
                // check if pin has been disposed
                if (_disposed)
                {
                    throw new ObjectDisposedException();
                }

                // set average count to 1 for single sample
                _averageCount = 1;

                return NativeReadValue();
            }
        }

        /// <inheritdoc/>
        public override int ReadValueAveraged(int count)
        {
            lock (_syncLock)
            {
                // check if pin has been disposed
                if (_disposed)
                {
                    throw new ObjectDisposedException();
                }

                _averageCount = count;

                return NativeReadValue();
            }
        }

        #region IDisposable Support

        private void Dispose(bool disposing)
        {
            if (_adcController != null)
            {
                if (disposing)
                {
                    NativeDisposeChannel();
                    _adcController = null;

                }

                _disposed = true;
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            lock (_syncLock)
            {
                if (!_disposed)
                {
                    Dispose(true);
                    GC.SuppressFinalize(this);
                }
            }
        }

#pragma warning disable 1591
        ~AdcChannel()
        {
            Dispose(false);
        }
#pragma warning restore 1591

        #endregion

        #region Native Calls

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern int NativeReadValue();

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void NativeDisposeChannel();

        #endregion

    }
}
