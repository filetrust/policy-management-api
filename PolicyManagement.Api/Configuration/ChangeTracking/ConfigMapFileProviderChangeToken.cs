using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using Microsoft.Extensions.Primitives;

namespace Glasswall.PolicyManagement.Api.Configuration.ChangeTracking
{
    [ExcludeFromCodeCoverage]
    public sealed class ConfigMapFileProviderChangeToken : IChangeToken, IDisposable
    {
        class CallbackRegistration : IDisposable
        {
            Action<object> _callback;
            object _state;
            Action<CallbackRegistration> _unregister;


            public CallbackRegistration(Action<object> callback, object state, Action<CallbackRegistration> unregister)
            {
                this._callback = callback;
                this._state = state;
                this._unregister = unregister;
            }

            public void Notify()
            {
                var localState = this._state;
                var localCallback = this._callback;
                if (localCallback != null)
                {
                    localCallback.Invoke(localState);
                }
            }


            public void Dispose()
            {
                var localUnregister = Interlocked.Exchange(ref _unregister, null);
                if (localUnregister != null)
                {
                    localUnregister(this);
                    this._callback = null;
                    this._state = null;
                }
            }
        }

        List<CallbackRegistration> _registeredCallbacks;
        private readonly string _rootPath;
        private readonly string _filter;
        private readonly int _detectChangeIntervalMs;
        private Timer _timer;
        private string _lastChecksum;
        readonly object _timerLock = new object();

        public ConfigMapFileProviderChangeToken(string rootPath, string filter, int detectChangeIntervalMs = 30_000)
        {
            _registeredCallbacks = new List<CallbackRegistration>();
            this._rootPath = rootPath;
            this._filter = filter;
            this._detectChangeIntervalMs = detectChangeIntervalMs;
        }

        internal void EnsureStarted()
        {
            lock (_timerLock)
            {
                if (_timer == null)
                {
                    var fullPath = Path.Combine(_rootPath, _filter);
                    if (File.Exists(fullPath))
                    {
                        this._timer = new Timer(CheckForChanges);
                        this._timer.Change(0, _detectChangeIntervalMs);
                    }
                }
            }
        }

        private void CheckForChanges(object state)
        {
            var fullPath = Path.Combine(_rootPath, _filter);
            var newCheckSum = GetFileChecksum(fullPath);
            var newHasChangesValue = false;
            if (this._lastChecksum != null && this._lastChecksum != newCheckSum)
            {
                NotifyChanges();

                newHasChangesValue = true;

                Console.WriteLine($"config/appsettings.json has changed");
            }

            this.HasChanged = newHasChangesValue;

            this._lastChecksum = newCheckSum;

        }

        private void NotifyChanges()
        {
            var localRegisteredCallbacks = _registeredCallbacks;
            if (localRegisteredCallbacks != null)
            {
                var count = localRegisteredCallbacks.Count;
                for (int i = 0; i < count; i++)
                {
                    localRegisteredCallbacks[i].Notify();
                }
            }
        }

        string GetFileChecksum(string filename)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(filename))
                {
                    return BitConverter.ToString(md5.ComputeHash(stream));
                }
            }
        }

        public bool HasChanged { get; private set; }

        public bool ActiveChangeCallbacks => true;

        public IDisposable RegisterChangeCallback(Action<object> callback, object state)
        {
            var localRegisteredCallbacks = _registeredCallbacks;
            if (localRegisteredCallbacks == null)
                throw new ObjectDisposedException(nameof(_registeredCallbacks));

            var cbRegistration = new CallbackRegistration(callback, state, (cb) => localRegisteredCallbacks.Remove(cb));
            localRegisteredCallbacks.Add(cbRegistration);

            return cbRegistration;
        }

        public void Dispose()
        {
            Interlocked.Exchange(ref _registeredCallbacks, null);

            Timer localTimer = null;
            lock (_timerLock)
            {
                localTimer = Interlocked.Exchange(ref _timer, null);
            }

            localTimer?.Dispose();
        }
    }
}