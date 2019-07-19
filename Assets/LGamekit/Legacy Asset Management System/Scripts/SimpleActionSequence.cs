using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace LGamekit.LegacyAssetManagementSystem {

	public sealed class SimpleActionSequence {

		Queue<Action> m_Actions = new Queue<Action>();
		Func<bool> m_CanContinue;

        public SimpleActionSequence(Func<bool> canContinue) {
            m_CanContinue = canContinue;
        }

		/// <summary>
		/// record an action
		/// </summary>
        public void Enqueue(Action action, bool checkContinue = false) {
            if (checkContinue && m_CanContinue.Invoke()) {
                action.Invoke();
                return;
            }
            m_Actions.Enqueue(action);
        }

		/// <summary>
		/// Execute next action until can not continue.
		/// </summary>
        public void Dequeue() {
            while (m_Actions.Count > 0) {
                m_Actions.Dequeue().Invoke();
                if (!m_CanContinue.Invoke()) {
                    break;
                }
            }
        }

		/// <summary>
		/// Clear all actions.
		/// </summary>
        public void Clear() {
            m_Actions.Clear();
        }

    }

}