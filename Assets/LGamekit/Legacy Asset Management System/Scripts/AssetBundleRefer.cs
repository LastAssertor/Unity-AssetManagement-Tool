using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace LGamekit.LegacyAssetManagementSystem {

    public class AssetBundleRefer : IReferCounter {

        private static ObjectPool<AssetBundleRefer> m_Pool;
        private static ObjectPool<AssetBundleRefer> Pool {
            get {
                if (m_Pool == null) {
                    m_Pool = ObjectPool<AssetBundleRefer>.Create(Factory, 128);
                }
                return m_Pool;
            }
        }

        public int Count {
            get;
            private set;
        }

        public AssetBundle Target {
            get;
            private set;
        }

        private AssetBundleRefer() {

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
                Target.Unload(false);
                Target = null;
            } else {
                if (Count > 0) {
                    if (--Count == 0) {
                        Target.Unload(false);
                        Target = null;
                    }
                }
            }
            return Count;
        }

        public static AssetBundleRefer Spawn(AssetBundle assetBundle) {
            if (assetBundle == null) {
                throw new ArgumentException("assetBundle == null.");
            }
            AssetBundleRefer assetBundleRefer = Pool.Spawn();
            assetBundleRefer.Count = 0;
            assetBundleRefer.Target = assetBundle;
            return assetBundleRefer;
        }

        public static void Unspawn(AssetBundleRefer assetBundleRefer) {
            if (assetBundleRefer.Target != null) {
                assetBundleRefer.Count = 0;
                assetBundleRefer.Target.Unload(false);
                assetBundleRefer.Target = null;
            }
            Pool.Unspawn(assetBundleRefer);
        }

        static AssetBundleRefer Factory() {
            return new AssetBundleRefer();
        }

    }

}