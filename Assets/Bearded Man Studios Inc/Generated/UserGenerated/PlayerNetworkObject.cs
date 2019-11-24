using BeardedManStudios.Forge.Networking.Frame;
using BeardedManStudios.Forge.Networking.Unity;
using System;
using UnityEngine;

namespace BeardedManStudios.Forge.Networking.Generated
{
	[GeneratedInterpol("{\"inter\":[0.25,0.25,0,0]")]
	public partial class PlayerNetworkObject : NetworkObject
	{
		public const int IDENTITY = 2;

		private byte[] _dirtyFields = new byte[1];

		#pragma warning disable 0067
		public event FieldChangedEvent fieldAltered;
		#pragma warning restore 0067
		[ForgeGeneratedField]
		private Vector2 _position;
		public event FieldEvent<Vector2> positionChanged;
		public InterpolateVector2 positionInterpolation = new InterpolateVector2() { LerpT = 0.25f, Enabled = true };
		public Vector2 position
		{
			get { return _position; }
			set
			{
				// Don't do anything if the value is the same
				if (_position == value)
					return;

				// Mark the field as dirty for the network to transmit
				_dirtyFields[0] |= 0x1;
				_position = value;
				hasDirtyFields = true;
			}
		}

		public void SetpositionDirty()
		{
			_dirtyFields[0] |= 0x1;
			hasDirtyFields = true;
		}

		private void RunChange_position(ulong timestep)
		{
			if (positionChanged != null) positionChanged(_position, timestep);
			if (fieldAltered != null) fieldAltered("position", _position, timestep);
		}
		[ForgeGeneratedField]
		private Vector2 _aimingPosition;
		public event FieldEvent<Vector2> aimingPositionChanged;
		public InterpolateVector2 aimingPositionInterpolation = new InterpolateVector2() { LerpT = 0.25f, Enabled = true };
		public Vector2 aimingPosition
		{
			get { return _aimingPosition; }
			set
			{
				// Don't do anything if the value is the same
				if (_aimingPosition == value)
					return;

				// Mark the field as dirty for the network to transmit
				_dirtyFields[0] |= 0x2;
				_aimingPosition = value;
				hasDirtyFields = true;
			}
		}

		public void SetaimingPositionDirty()
		{
			_dirtyFields[0] |= 0x2;
			hasDirtyFields = true;
		}

		private void RunChange_aimingPosition(ulong timestep)
		{
			if (aimingPositionChanged != null) aimingPositionChanged(_aimingPosition, timestep);
			if (fieldAltered != null) fieldAltered("aimingPosition", _aimingPosition, timestep);
		}
		[ForgeGeneratedField]
		private int _health;
		public event FieldEvent<int> healthChanged;
		public Interpolated<int> healthInterpolation = new Interpolated<int>() { LerpT = 0f, Enabled = false };
		public int health
		{
			get { return _health; }
			set
			{
				// Don't do anything if the value is the same
				if (_health == value)
					return;

				// Mark the field as dirty for the network to transmit
				_dirtyFields[0] |= 0x4;
				_health = value;
				hasDirtyFields = true;
			}
		}

		public void SethealthDirty()
		{
			_dirtyFields[0] |= 0x4;
			hasDirtyFields = true;
		}

		private void RunChange_health(ulong timestep)
		{
			if (healthChanged != null) healthChanged(_health, timestep);
			if (fieldAltered != null) fieldAltered("health", _health, timestep);
		}
		[ForgeGeneratedField]
		private bool _alive;
		public event FieldEvent<bool> aliveChanged;
		public Interpolated<bool> aliveInterpolation = new Interpolated<bool>() { LerpT = 0f, Enabled = false };
		public bool alive
		{
			get { return _alive; }
			set
			{
				// Don't do anything if the value is the same
				if (_alive == value)
					return;

				// Mark the field as dirty for the network to transmit
				_dirtyFields[0] |= 0x8;
				_alive = value;
				hasDirtyFields = true;
			}
		}

		public void SetaliveDirty()
		{
			_dirtyFields[0] |= 0x8;
			hasDirtyFields = true;
		}

		private void RunChange_alive(ulong timestep)
		{
			if (aliveChanged != null) aliveChanged(_alive, timestep);
			if (fieldAltered != null) fieldAltered("alive", _alive, timestep);
		}

		protected override void OwnershipChanged()
		{
			base.OwnershipChanged();
			SnapInterpolations();
		}
		
		public void SnapInterpolations()
		{
			positionInterpolation.current = positionInterpolation.target;
			aimingPositionInterpolation.current = aimingPositionInterpolation.target;
			healthInterpolation.current = healthInterpolation.target;
			aliveInterpolation.current = aliveInterpolation.target;
		}

		public override int UniqueIdentity { get { return IDENTITY; } }

		protected override BMSByte WritePayload(BMSByte data)
		{
			UnityObjectMapper.Instance.MapBytes(data, _position);
			UnityObjectMapper.Instance.MapBytes(data, _aimingPosition);
			UnityObjectMapper.Instance.MapBytes(data, _health);
			UnityObjectMapper.Instance.MapBytes(data, _alive);

			return data;
		}

		protected override void ReadPayload(BMSByte payload, ulong timestep)
		{
			_position = UnityObjectMapper.Instance.Map<Vector2>(payload);
			positionInterpolation.current = _position;
			positionInterpolation.target = _position;
			RunChange_position(timestep);
			_aimingPosition = UnityObjectMapper.Instance.Map<Vector2>(payload);
			aimingPositionInterpolation.current = _aimingPosition;
			aimingPositionInterpolation.target = _aimingPosition;
			RunChange_aimingPosition(timestep);
			_health = UnityObjectMapper.Instance.Map<int>(payload);
			healthInterpolation.current = _health;
			healthInterpolation.target = _health;
			RunChange_health(timestep);
			_alive = UnityObjectMapper.Instance.Map<bool>(payload);
			aliveInterpolation.current = _alive;
			aliveInterpolation.target = _alive;
			RunChange_alive(timestep);
		}

		protected override BMSByte SerializeDirtyFields()
		{
			dirtyFieldsData.Clear();
			dirtyFieldsData.Append(_dirtyFields);

			if ((0x1 & _dirtyFields[0]) != 0)
				UnityObjectMapper.Instance.MapBytes(dirtyFieldsData, _position);
			if ((0x2 & _dirtyFields[0]) != 0)
				UnityObjectMapper.Instance.MapBytes(dirtyFieldsData, _aimingPosition);
			if ((0x4 & _dirtyFields[0]) != 0)
				UnityObjectMapper.Instance.MapBytes(dirtyFieldsData, _health);
			if ((0x8 & _dirtyFields[0]) != 0)
				UnityObjectMapper.Instance.MapBytes(dirtyFieldsData, _alive);

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
				if (positionInterpolation.Enabled)
				{
					positionInterpolation.target = UnityObjectMapper.Instance.Map<Vector2>(data);
					positionInterpolation.Timestep = timestep;
				}
				else
				{
					_position = UnityObjectMapper.Instance.Map<Vector2>(data);
					RunChange_position(timestep);
				}
			}
			if ((0x2 & readDirtyFlags[0]) != 0)
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
			if ((0x4 & readDirtyFlags[0]) != 0)
			{
				if (healthInterpolation.Enabled)
				{
					healthInterpolation.target = UnityObjectMapper.Instance.Map<int>(data);
					healthInterpolation.Timestep = timestep;
				}
				else
				{
					_health = UnityObjectMapper.Instance.Map<int>(data);
					RunChange_health(timestep);
				}
			}
			if ((0x8 & readDirtyFlags[0]) != 0)
			{
				if (aliveInterpolation.Enabled)
				{
					aliveInterpolation.target = UnityObjectMapper.Instance.Map<bool>(data);
					aliveInterpolation.Timestep = timestep;
				}
				else
				{
					_alive = UnityObjectMapper.Instance.Map<bool>(data);
					RunChange_alive(timestep);
				}
			}
		}

		public override void InterpolateUpdate()
		{
			if (IsOwner)
				return;

			if (positionInterpolation.Enabled && !positionInterpolation.current.UnityNear(positionInterpolation.target, 0.0015f))
			{
				_position = (Vector2)positionInterpolation.Interpolate();
				//RunChange_position(positionInterpolation.Timestep);
			}
			if (aimingPositionInterpolation.Enabled && !aimingPositionInterpolation.current.UnityNear(aimingPositionInterpolation.target, 0.0015f))
			{
				_aimingPosition = (Vector2)aimingPositionInterpolation.Interpolate();
				//RunChange_aimingPosition(aimingPositionInterpolation.Timestep);
			}
			if (healthInterpolation.Enabled && !healthInterpolation.current.UnityNear(healthInterpolation.target, 0.0015f))
			{
				_health = (int)healthInterpolation.Interpolate();
				//RunChange_health(healthInterpolation.Timestep);
			}
			if (aliveInterpolation.Enabled && !aliveInterpolation.current.UnityNear(aliveInterpolation.target, 0.0015f))
			{
				_alive = (bool)aliveInterpolation.Interpolate();
				//RunChange_alive(aliveInterpolation.Timestep);
			}
		}

		private void Initialize()
		{
			if (readDirtyFlags == null)
				readDirtyFlags = new byte[1];

		}

		public PlayerNetworkObject() : base() { Initialize(); }
		public PlayerNetworkObject(NetWorker networker, INetworkBehavior networkBehavior = null, int createCode = 0, byte[] metadata = null) : base(networker, networkBehavior, createCode, metadata) { Initialize(); }
		public PlayerNetworkObject(NetWorker networker, uint serverId, FrameStream frame) : base(networker, serverId, frame) { Initialize(); }

		// DO NOT TOUCH, THIS GETS GENERATED PLEASE EXTEND THIS CLASS IF YOU WISH TO HAVE CUSTOM CODE ADDITIONS
	}
}
