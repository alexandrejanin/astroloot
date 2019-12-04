using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Unity;
using UnityEngine;

namespace BeardedManStudios.Forge.Networking.Generated {
    [GeneratedRPC("{\"types\":[[][\"int\"][\"Vector2\"][\"uint\", \"Vector2\", \"Vector2\"][\"uint\"][][][\"int\"]]")]
    [GeneratedRPCVariableNames(
        "{\"types\":[[][\"damage\"][\"direction\"][\"bulletId\", \"originPosition\", \"targetPosition\"][\"bulletId\"][][][\"index\"]]")]
    public abstract partial class PlayerBehavior : NetworkBehavior {
        public const byte RPC_LOCAL_PLAYER_SPAWNED = 0 + 5;
        public const byte RPC_DAMAGE = 1 + 5;
        public const byte RPC_KNOCKBACK = 2 + 5;
        public const byte RPC_SHOOT = 3 + 5;
        public const byte RPC_DESTROY_BULLET = 4 + 5;
        public const byte RPC_ON_DEATH = 5 + 5;
        public const byte RPC_ON_RESPAWN = 6 + 5;
        public const byte RPC_SET_WEAPON = 7 + 5;

        public PlayerNetworkObject networkObject = null;

        public override void Initialize(NetworkObject obj) {
            // We have already initialized this object
            if (networkObject != null && networkObject.AttachedBehavior != null)
                return;

            networkObject = (PlayerNetworkObject) obj;
            networkObject.AttachedBehavior = this;

            base.SetupHelperRpcs(networkObject);
            networkObject.RegisterRpc("LocalPlayerSpawned", LocalPlayerSpawned);
            networkObject.RegisterRpc("Damage", Damage, typeof(int));
            networkObject.RegisterRpc("Knockback", Knockback, typeof(Vector2));
            networkObject.RegisterRpc("Shoot", Shoot, typeof(uint), typeof(Vector2), typeof(Vector2));
            networkObject.RegisterRpc("DestroyBullet", DestroyBullet, typeof(uint));
            networkObject.RegisterRpc("OnDeath", OnDeath);
            networkObject.RegisterRpc("OnRespawn", OnRespawn);
            networkObject.RegisterRpc("SetWeapon", SetWeapon, typeof(int));

            networkObject.onDestroy += DestroyGameObject;

            if (!obj.IsOwner) {
                if (!skipAttachIds.ContainsKey(obj.NetworkId)) {
                    uint newId = obj.NetworkId + 1;
                    ProcessOthers(gameObject.transform, ref newId);
                } else
                    skipAttachIds.Remove(obj.NetworkId);
            }

            if (obj.Metadata != null) {
                byte transformFlags = obj.Metadata[0];

                if (transformFlags != 0) {
                    BMSByte metadataTransform = new BMSByte();
                    metadataTransform.Clone(obj.Metadata);
                    metadataTransform.MoveStartIndex(1);

                    if ((transformFlags & 0x01) != 0 && (transformFlags & 0x02) != 0) {
                        MainThreadManager.Run(() => {
                            transform.position = ObjectMapper.Instance.Map<Vector3>(metadataTransform);
                            transform.rotation = ObjectMapper.Instance.Map<Quaternion>(metadataTransform);
                        });
                    } else if ((transformFlags & 0x01) != 0) {
                        MainThreadManager.Run(() => {
                            transform.position = ObjectMapper.Instance.Map<Vector3>(metadataTransform);
                        });
                    } else if ((transformFlags & 0x02) != 0) {
                        MainThreadManager.Run(() => {
                            transform.rotation = ObjectMapper.Instance.Map<Quaternion>(metadataTransform);
                        });
                    }
                }
            }

            MainThreadManager.Run(() => {
                NetworkStart();
                networkObject.Networker.FlushCreateActions(networkObject);
            });
        }

        protected override void CompleteRegistration() {
            base.CompleteRegistration();
            networkObject.ReleaseCreateBuffer();
        }

        public override void Initialize(NetWorker networker, byte[] metadata = null) {
            Initialize(new PlayerNetworkObject(networker, createCode: TempAttachCode, metadata: metadata));
        }

        private void DestroyGameObject(NetWorker sender) {
            MainThreadManager.Run(() => {
                try {
                    Destroy(gameObject);
                } catch { }
            });
            networkObject.onDestroy -= DestroyGameObject;
        }

        public override NetworkObject CreateNetworkObject(NetWorker networker, int createCode, byte[] metadata = null) {
            return new PlayerNetworkObject(networker, this, createCode, metadata);
        }

        protected override void InitializedTransform() {
            networkObject.SnapInterpolations();
        }

        /// <summary>
        /// Arguments:
        /// </summary>
        public abstract void LocalPlayerSpawned(RpcArgs args);

        /// <summary>
        /// Arguments:
        /// </summary>
        public abstract void Damage(RpcArgs args);

        /// <summary>
        /// Arguments:
        /// </summary>
        public abstract void Knockback(RpcArgs args);

        /// <summary>
        /// Arguments:
        /// </summary>
        public abstract void Shoot(RpcArgs args);

        /// <summary>
        /// Arguments:
        /// </summary>
        public abstract void DestroyBullet(RpcArgs args);

        /// <summary>
        /// Arguments:
        /// </summary>
        public abstract void OnDeath(RpcArgs args);

        /// <summary>
        /// Arguments:
        /// </summary>
        public abstract void OnRespawn(RpcArgs args);

        /// <summary>
        /// Arguments:
        /// </summary>
        public abstract void SetWeapon(RpcArgs args);

        // DO NOT TOUCH, THIS GETS GENERATED PLEASE EXTEND THIS CLASS IF YOU WISH TO HAVE CUSTOM CODE ADDITIONS
    }
}