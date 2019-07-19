using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Object = UnityEngine.Object;

namespace LGamekit.LegacyAssetManagementSystem {

    public class AssetRefer : IReferCounter {

        private static ObjectPool<AssetRefer> m_Pool;
        private static ObjectPool<AssetRefer> Pool {
            get {
                if (m_Pool == null) {
                    m_Pool = ObjectPool<AssetRefer>.Create(Factory, 256);
                }
                return m_Pool;
            }
        }

        public int Count {
            get;
            private set;
        }

        public Object Target {
            get;
            private set;
        }

        private AssetRefer() {

        }

        public T GetAsset<T>() where T : Object {
            return Target != null ? Target as T : default(T);
        }

        public int Retain() {
            if (Target == null) {
                throw new ObjectDisposedException("this object is already invalid.");
            }
            return ++Count;
        }

        public int Release(bool force = false) {
            if (Target == null) {
                throw new ObjectDisposedException("this object is already invalid.");
            }
            if (force) {
                Count = 0;
                Target = null;
            } else {
                if (Count > 0) {
                    if (--Count == 0) {
                        Target = null;
                    }
                }
            }
            return Count;
        }

        public static AssetRefer Spawn(Object asset) {
            if (asset == null) {
                throw new ArgumentException("asset == null.");
            }
            AssetRefer assetRefer = Pool.Spawn();
            assetRefer.Count = 0;
            assetRefer.Target = asset;
            return assetRefer;
        }

        public static void Unspawn(AssetRefer assetRefer) {
            if (assetRefer.Target != null) {
                assetRefer.Count = 0;
                assetRefer.Target = null;
            }
            Pool.Unspawn(assetRefer);
        }

        static AssetRefer Factory() {
            return new AssetRefer();
        }

    }

}