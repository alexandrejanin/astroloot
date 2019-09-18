using BeardedManStudios.Forge.Networking.Frame;
using BeardedManStudios.Forge.Networking.Unity;
using System;
using UnityEngine;

namespace BeardedManStudios.Forge.Networking.Generated
{
	[GeneratedInterpol("{\"inter\":[0.15]")]
	public partial class PlayerCombatNetworkObject : NetworkObject
	{
		public const int IDENTITY = 5;

		private byte[] _dirtyFields = new byte[1];

		#pragma warning disable 0067
		public event FieldChangedEvent fieldAltered;
		#pragma warning restore 0067
		[ForgeGeneratedField]
		private Vector2 _aimingPosition;
		public event FieldEvent<Vector2> aimingPositionChanged;
		public InterpolateVector2 aimingPositionInterpolation = new InterpolateVector2() { LerpT = 0.15f, Enabled = true };
		public Vector2 aimingPosition
		{
			get { return _aimingPosition; }
			set
			{
				// Don't do anything if the value is the same
				if (_aimingPosition == value)
					return;

				// Mark the field as dirty for the network to transmit
				_dirtyFields[0] |= 0x1;
				_aimingPosition = value;
				hasDirtyFields = true;
			}
		}

		public void SetaimingPositionDirty()
		{
			_dirtyFields[0] |= 0x1;
			hasDirtyFields = true;
		}

		private void RunChange_aimingPosition(ulong timestep)
		{
			if (aimingPositionChanged != null) aimingPositionChanged(_aimingPosition, timestep);
			if (fieldAltered != null) fieldAltered("aimingPosition", _aimingPosition, timestep);
		}

		protected override void OwnershipChanged()
		{
			base.OwnershipChanged();
			SnapInterpolations();
		}
		
		public void SnapInterpolations()
		{
			aimingPositionInterpolation.current = aimingPositionInterpolation.target;
		}

		public override int UniqueIdentity { get { return IDENTITY; } }

		protected override BMSByte WritePayload(BMSByte data)
		{
			UnityObjectMapper.Instance.MapBytes(data, _aimingPosition);

			return data;
		}

		protected override void ReadPayload(BMSByte payload, ulong timestep)
		{
			_aimingPosition = UnityObjectMapper.Instance.Map<Vector2>(payload);
			aimingPositionInterpolation.current = _aimingPosition;
			aimingPositionInterpolation.target = _aimingPosition;
			RunChange_aimingPosition(timestep);
		}

		protected override BMSByte SerializeDirtyFields()
		{
			dirtyFieldsData.Clear();
			dirtyFieldsData.Append(_dirtyFields);

			if ((0x1 & _dirtyFields[0]) != 0)
				UnityObjectMapper.Instance.MapBytes(dirtyFieldsData, _aimingPosition);

			// Reset all the dirty fields
			for (int i = 0; i < _dirtyFields.Length; i++)
				_dirtyFields[i] = 0;

			return dirtyFieldsData;
		}

		protected override void ReadDirtyFields(BMSByte data, ulong timestep)
		{
			if (readDirtyFlags == null)
				Initialize();

			Buffer.BlockCopy(data.byteArr, data.StartIndex(), readDirtyFlags, 0, readDirtyFlags.Length);
			data.MoveStartIndex(readDirtyFlags.Length);

			if ((0x1 & readDirtyFlags[0]) != 0)
			{
				if (aimingPositionInterpolation.Enabled)
				{
					aimingPositionInterpolation.target = UnityObjectMapper.Instance.Map<Vector2>(data);
					aimingPositionInterpolation.Timestep = timestep;
				}
				else
				{
					_aimingPosition = UnityObjectMapper.Instance.Map<Vector2>(data);
					RunChange_aimingPosition(timestep);
				}
			}
		}

		public override void InterpolateUpdate()
		{
			if (IsOwner)
				return;

			if (aimingPositionInterpolation.Enabled && !aimingPositionInterpolation.current.UnityNear(aimingPositionInterpolation.target, 0.0015f))
			{
				_aimingPosition = (Vector2)aimingPositionInterpolation.Interpolate();
				//RunChange_aimingPosition(aimingPositionInterpolation.Timestep);
			}
		}

		private void Initialize()
		{
			if (readDirtyFlags == null)
				readDirtyFlags = new byte[1];

		}

		public PlayerCombatNetworkObject() : base() { Initialize(); }
		public PlayerCombatNetworkObject(NetWorker networker, INetworkBehavior networkBehavior = null, int createCode = 0, byte[] metadata = null) : base(networker, networkBehavior, createCode, metadata) { Initialize(); }
		public PlayerCombatNetworkObject(NetWorker networker, uint serverId, FrameStream frame) : base(networker, serverId, frame) { Initialize(); }

		// DO NOT TOUCH, THIS GETS GENERATED PLEASE EXTEND THIS CLASS IF YOU WISH TO HAVE CUSTOM CODE ADDITIONS
	}
}
