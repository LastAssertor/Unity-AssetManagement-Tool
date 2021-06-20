// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.

// File : ObjectPool.cs
// Date : 2019.7.17
// Modified by : assertorz
// Contact: assertor@qq.com
// Descriptions : the origin source is from the web site http://source.roslyn.codeplex.com/#Microsoft.CodeAnalysis.Workspaces/ObjectPool%25601.cs


using System.Collections;
using System.Collections.Generic;
using System;

namespace UnityAssetManagementTool {

    public sealed class ObjectPool<T> where T : class {

        private struct Element {
            internal T Value;
        }

        public delegate T Func();
        public delegate void Action(T obj);

        private T m_FirstItem;

        private Element[] m_Items;

        private Func m_Factory;
        private Action m_Spawn;
        private Action m_Unspawn;
        private Action m_Release;

        private bool m_Valid;

        private ObjectPool(Func factory, Action spawn, Action unspawn, Action release, int size) {

            if (size <= 0) {
                size = Environment.ProcessorCount * 2;
            }

            if (factory == null) {
                throw new ArgumentException("pool<" + typeof(T).Name + ">.factory == null.");
            }

            m_Factory = factory;
            m_Spawn = spawn;
            m_Unspawn = unspawn;
            m_Release = release;
            m_Items = new Element[size - 1];
            m_Valid = true;
        }

        public T Spawn() {

            if (!m_Valid) {
                throw new ObjectDisposedException("pool<" + typeof(T).Name + "> is already invalid.");
            }

            T inst = m_FirstItem;

            if (inst != null) {

                m_FirstItem = null;

            } else {

                Element[] items = m_Items;
                for (int i = 0; i < items.Length; i++) {
                    inst = items[i].Value;
                    if (inst != null) {
                        items[i].Value = null;
                        break;
                    }
                }

                if (inst == null) {
                    inst = m_Factory();
                }
            }

            if (m_Spawn != null)
                m_Spawn.Invoke(inst);

            return inst;
        }

        public void Unspawn(T obj) {

            if (!m_Valid) {
                throw new ObjectDisposedException("pool<" + typeof(T).Name + "> is already invalid.");
            }

            if (obj == null) {
                throw new ArgumentException("obj == null.");
            }

            if (obj == m_FirstItem) {
                throw new ArgumentException("unspawn twice?");
            }

            Element[] items = m_Items;
            for (int i = 0; i < items.Length; i++) {
                if (items[i].Value == obj) {
                    throw new ArgumentException("unspawn twice?");
                }
            }

            if (m_Unspawn != null)
                m_Unspawn.Invoke(obj);

            if (m_FirstItem == null) {
                m_FirstItem = obj;
                return;
            }

            for (int i = 0; i < items.Length; i++) {
                if (items[i].Value == null) {
                    items[i].Value = obj;
                    return;
                }
            }

            if (m_Release != null)
                m_Release.Invoke(obj);
        }

        private void InternalRelease() {

            if (!m_Valid) {
                throw new ObjectDisposedException("pool<" + typeof(T).Name + "> is already invalid.");
            }

            m_Valid = false;

            Element[] items = m_Items;
            T firstItem = m_FirstItem;
            Action release = m_Release;

            for (int i = 0; i < items.Length; i++) {
                if (items[i].Value != null) {
                    if (release != null)
                        release.Invoke(items[i].Value);
                    items[i].Value = null;
                }
            }

            if (firstItem != null) {
                if (release != null)
                    release.Invoke(firstItem);
            }

            m_Release = null;
            m_Unspawn = null;
            m_Spawn = null;
            m_Factory = null;
            m_FirstItem = null;
            m_Items = null;
        }

        public static void Delete(ObjectPool<T> pool) {
            pool.InternalRelease();
        }

        public static ObjectPool<T> Create(Func factory, int size = 256) {
            return new ObjectPool<T>(factory, null, null, null, size);
        }

        public static ObjectPool<T> Create(Func factory, Action release, int size = 256) {
            return new ObjectPool<T>(factory, null, null, release, size);
        }

        public static ObjectPool<T> Create(Func factory, Action spawn, Action unspawn, Action release, int size = 256) {
            return new ObjectPool<T>(factory, spawn, unspawn, release, size);
        }

    }

}
