using BeardedManStudios.Forge.Networking.Frame;
using BeardedManStudios.Forge.Networking.Unity;
using System;
using UnityEngine;

namespace BeardedManStudios.Forge.Networking.Generated {
    [GeneratedInterpol("{\"inter\":[0]")]
    public partial class WeaponPickupNetworkObject : NetworkObject {
        public const int IDENTITY = 2;

        private byte[] _dirtyFields = new byte[1];

#pragma warning disable 0067
        public event FieldChangedEvent fieldAltered;
#pragma warning restore 0067
        [ForgeGeneratedField] private int _weaponIndex;
        public event FieldEvent<int> weaponIndexChanged;
        public Interpolated<int> weaponIndexInterpolation = new Interpolated<int>() {LerpT = 0f, Enabled = false};

        public int weaponIndex {
            get { return _weaponIndex; }
            set {
                // Don't do anything if the value is the same
                if (_weaponIndex == value)
                    return;

                // Mark the field as dirty for the network to transmit
                _dirtyFields[0] |= 0x1;
                _weaponIndex = value;
                hasDirtyFields = true;
            }
        }

        public void SetweaponIndexDirty() {
            _dirtyFields[0] |= 0x1;
            hasDirtyFields = true;
        }

        private void RunChange_weaponIndex(ulong timestep) {
            if (weaponIndexChanged != null) weaponIndexChanged(_weaponIndex, timestep);
            if (fieldAltered != null) fieldAltered("weaponIndex", _weaponIndex, timestep);
        }

        protected override void OwnershipChanged() {
            base.OwnershipChanged();
            SnapInterpolations();
        }

        public void SnapInterpolations() {
            weaponIndexInterpolation.current = weaponIndexInterpolation.target;
        }

        public override int UniqueIdentity {
            get { return IDENTITY; }
        }

        protected override BMSByte WritePayload(BMSByte data) {
            UnityObjectMapper.Instance.MapBytes(data, _weaponIndex);

            return data;
        }

        protected override void ReadPayload(BMSByte payload, ulong timestep) {
            _weaponIndex = UnityObjectMapper.Instance.Map<int>(payload);
            weaponIndexInterpolation.current = _weaponIndex;
            weaponIndexInterpolation.target = _weaponIndex;
            RunChange_weaponIndex(timestep);
        }

        protected override BMSByte SerializeDirtyFields() {
            dirtyFieldsData.Clear();
            dirtyFieldsData.Append(_dirtyFields);

            if ((0x1 & _dirtyFields[0]) != 0)
                UnityObjectMapper.Instance.MapBytes(dirtyFieldsData, _weaponIndex);

            // Reset all the dirty fields
            for (int i = 0; i < _dirtyFields.Length; i++)
                _dirtyFields[i] = 0;

            return dirtyFieldsData;
        }

        protected override void ReadDirtyFields(BMSByte data, ulong timestep) {
            if (readDirtyFlags == null)
                Initialize();

            Buffer.BlockCopy(data.byteArr, data.StartIndex(), readDirtyFlags, 0, readDirtyFlags.Length);
            data.MoveStartIndex(readDirtyFlags.Length);

            if ((0x1 & readDirtyFlags[0]) != 0) {
                if (weaponIndexInterpolation.Enabled) {
                    weaponIndexInterpolation.target = UnityObjectMapper.Instance.Map<int>(data);
                    weaponIndexInterpolation.Timestep = timestep;
                } else {
                    _weaponIndex = UnityObjectMapper.Instance.Map<int>(data);
                    RunChange_weaponIndex(timestep);
                }
            }
        }

        public override void InterpolateUpdate() {
            if (IsOwner)
                return;

            if (weaponIndexInterpolation.Enabled &&
                !weaponIndexInterpolation.current.UnityNear(weaponIndexInterpolation.target, 0.0015f)) {
                _weaponIndex = (int) weaponIndexInterpolation.Interpolate();
                //RunChange_weaponIndex(weaponIndexInterpolation.Timestep);
            }
        }

        private void Initialize() {
            if (readDirtyFlags == null)
                readDirtyFlags = new byte[1];
        }

        public WeaponPickupNetworkObject() : base() {
            Initialize();
        }

        public WeaponPickupNetworkObject(NetWorker networker, INetworkBehavior networkBehavior = null,
            int createCode = 0, byte[] metadata = null) : base(networker, networkBehavior, createCode, metadata) {
            Initialize();
        }

        public WeaponPickupNetworkObject(NetWorker networker, uint serverId, FrameStream frame) : base(networker,
            serverId, frame) {
            Initialize();
        }

        // DO NOT TOUCH, THIS GETS GENERATED PLEASE EXTEND THIS CLASS IF YOU WISH TO HAVE CUSTOM CODE ADDITIONS
    }
}