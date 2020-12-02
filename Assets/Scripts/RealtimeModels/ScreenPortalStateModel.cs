using Normal.Realtime;
using Normal.Realtime.Serialization;

namespace Assets.Scripts
{
    [RealtimeModel()]
    public partial class ScreenPortalStateModel
    {
        [RealtimeProperty(1, true, true)] private int _screenId;
        [RealtimeProperty(2, true, true)] private bool _isPortal;
        [RealtimeProperty(3, true, true)] private int _destinationSceneId;
    }
}

/* ----- Begin Normal Autogenerated Code ----- */
namespace Assets.Scripts {
    public partial class ScreenPortalStateModel : RealtimeModel {
        public int screenId {
            get {
                return _cache.LookForValueInCache(_screenId, entry => entry.screenIdSet, entry => entry.screenId);
            }
            set {
                if (this.screenId == value) return;
                _cache.UpdateLocalCache(entry => { entry.screenIdSet = true; entry.screenId = value; return entry; });
                InvalidateReliableLength();
                FireScreenIdDidChange(value);
            }
        }
        
        public bool isPortal {
            get {
                return _cache.LookForValueInCache(_isPortal, entry => entry.isPortalSet, entry => entry.isPortal);
            }
            set {
                if (this.isPortal == value) return;
                _cache.UpdateLocalCache(entry => { entry.isPortalSet = true; entry.isPortal = value; return entry; });
                InvalidateReliableLength();
                FireIsPortalDidChange(value);
            }
        }
        
        public int destinationSceneId {
            get {
                return _cache.LookForValueInCache(_destinationSceneId, entry => entry.destinationSceneIdSet, entry => entry.destinationSceneId);
            }
            set {
                if (this.destinationSceneId == value) return;
                _cache.UpdateLocalCache(entry => { entry.destinationSceneIdSet = true; entry.destinationSceneId = value; return entry; });
                InvalidateReliableLength();
                FireDestinationSceneIdDidChange(value);
            }
        }
        
        public delegate void PropertyChangedHandler<in T>(ScreenPortalStateModel model, T value);
        public event PropertyChangedHandler<int> screenIdDidChange;
        public event PropertyChangedHandler<bool> isPortalDidChange;
        public event PropertyChangedHandler<int> destinationSceneIdDidChange;
        
        private struct LocalCacheEntry {
            public bool screenIdSet;
            public int screenId;
            public bool isPortalSet;
            public bool isPortal;
            public bool destinationSceneIdSet;
            public int destinationSceneId;
        }
        
        private LocalChangeCache<LocalCacheEntry> _cache = new LocalChangeCache<LocalCacheEntry>();
        
        public enum PropertyID : uint {
            ScreenId = 1,
            IsPortal = 2,
            DestinationSceneId = 3,
        }
        
        public ScreenPortalStateModel() : this(null) {
        }
        
        public ScreenPortalStateModel(RealtimeModel parent) : base(null, parent) {
        }
        
        protected override void OnParentReplaced(RealtimeModel previousParent, RealtimeModel currentParent) {
            UnsubscribeClearCacheCallback();
        }
        
        private void FireScreenIdDidChange(int value) {
            try {
                screenIdDidChange?.Invoke(this, value);
            } catch (System.Exception exception) {
                UnityEngine.Debug.LogException(exception);
            }
        }
        
        private void FireIsPortalDidChange(bool value) {
            try {
                isPortalDidChange?.Invoke(this, value);
            } catch (System.Exception exception) {
                UnityEngine.Debug.LogException(exception);
            }
        }
        
        private void FireDestinationSceneIdDidChange(int value) {
            try {
                destinationSceneIdDidChange?.Invoke(this, value);
            } catch (System.Exception exception) {
                UnityEngine.Debug.LogException(exception);
            }
        }
        
        protected override int WriteLength(StreamContext context) {
            int length = 0;
            if (context.fullModel) {
                FlattenCache();
                length += WriteStream.WriteVarint32Length((uint)PropertyID.ScreenId, (uint)_screenId);
                length += WriteStream.WriteVarint32Length((uint)PropertyID.IsPortal, _isPortal ? 1u : 0u);
                length += WriteStream.WriteVarint32Length((uint)PropertyID.DestinationSceneId, (uint)_destinationSceneId);
            } else if (context.reliableChannel) {
                LocalCacheEntry entry = _cache.localCache;
                if (entry.screenIdSet) {
                    length += WriteStream.WriteVarint32Length((uint)PropertyID.ScreenId, (uint)entry.screenId);
                }
                if (entry.isPortalSet) {
                    length += WriteStream.WriteVarint32Length((uint)PropertyID.IsPortal, entry.isPortal ? 1u : 0u);
                }
                if (entry.destinationSceneIdSet) {
                    length += WriteStream.WriteVarint32Length((uint)PropertyID.DestinationSceneId, (uint)entry.destinationSceneId);
                }
            }
            return length;
        }
        
        protected override void Write(WriteStream stream, StreamContext context) {
            var didWriteProperties = false;
            
            if (context.fullModel) {
                stream.WriteVarint32((uint)PropertyID.ScreenId, (uint)_screenId);
                stream.WriteVarint32((uint)PropertyID.IsPortal, _isPortal ? 1u : 0u);
                stream.WriteVarint32((uint)PropertyID.DestinationSceneId, (uint)_destinationSceneId);
            } else if (context.reliableChannel) {
                LocalCacheEntry entry = _cache.localCache;
                if (entry.screenIdSet || entry.isPortalSet || entry.destinationSceneIdSet) {
                    _cache.PushLocalCacheToInflight(context.updateID);
                    ClearCacheOnStreamCallback(context);
                }
                if (entry.screenIdSet) {
                    stream.WriteVarint32((uint)PropertyID.ScreenId, (uint)entry.screenId);
                    didWriteProperties = true;
                }
                if (entry.isPortalSet) {
                    stream.WriteVarint32((uint)PropertyID.IsPortal, entry.isPortal ? 1u : 0u);
                    didWriteProperties = true;
                }
                if (entry.destinationSceneIdSet) {
                    stream.WriteVarint32((uint)PropertyID.DestinationSceneId, (uint)entry.destinationSceneId);
                    didWriteProperties = true;
                }
                
                if (didWriteProperties) InvalidateReliableLength();
            }
        }
        
        protected override void Read(ReadStream stream, StreamContext context) {
            while (stream.ReadNextPropertyID(out uint propertyID)) {
                switch (propertyID) {
                    case (uint)PropertyID.ScreenId: {
                        int previousValue = _screenId;
                        _screenId = (int)stream.ReadVarint32();
                        bool screenIdExistsInChangeCache = _cache.ValueExistsInCache(entry => entry.screenIdSet);
                        if (!screenIdExistsInChangeCache && _screenId != previousValue) {
                            FireScreenIdDidChange(_screenId);
                        }
                        break;
                    }
                    case (uint)PropertyID.IsPortal: {
                        bool previousValue = _isPortal;
                        _isPortal = (stream.ReadVarint32() != 0);
                        bool isPortalExistsInChangeCache = _cache.ValueExistsInCache(entry => entry.isPortalSet);
                        if (!isPortalExistsInChangeCache && _isPortal != previousValue) {
                            FireIsPortalDidChange(_isPortal);
                        }
                        break;
                    }
                    case (uint)PropertyID.DestinationSceneId: {
                        int previousValue = _destinationSceneId;
                        _destinationSceneId = (int)stream.ReadVarint32();
                        bool destinationSceneIdExistsInChangeCache = _cache.ValueExistsInCache(entry => entry.destinationSceneIdSet);
                        if (!destinationSceneIdExistsInChangeCache && _destinationSceneId != previousValue) {
                            FireDestinationSceneIdDidChange(_destinationSceneId);
                        }
                        break;
                    }
                    default: {
                        stream.SkipProperty();
                        break;
                    }
                }
            }
        }
        
        #region Cache Operations
        
        private StreamEventDispatcher _streamEventDispatcher;
        
        private void FlattenCache() {
            _screenId = screenId;
            _isPortal = isPortal;
            _destinationSceneId = destinationSceneId;
            _cache.Clear();
        }
        
        private void ClearCache(uint updateID) {
            _cache.RemoveUpdateFromInflight(updateID);
        }
        
        private void ClearCacheOnStreamCallback(StreamContext context) {
            if (_streamEventDispatcher != context.dispatcher) {
                UnsubscribeClearCacheCallback(); // unsub from previous dispatcher
            }
            _streamEventDispatcher = context.dispatcher;
            _streamEventDispatcher.AddStreamCallback(context.updateID, ClearCache);
        }
        
        private void UnsubscribeClearCacheCallback() {
            if (_streamEventDispatcher != null) {
                _streamEventDispatcher.RemoveStreamCallback(ClearCache);
                _streamEventDispatcher = null;
            }
        }
        
        #endregion
    }
}
/* ----- End Normal Autogenerated Code ----- */
