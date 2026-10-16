using System;
using UnityEngine;

namespace HollowSaint.Core
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public int CurrentDay { get; private set; } = 1;
        public int CasesResolved { get; private set; } = 0;
        public bool TwistTriggered { get; private set; } = false;

        public event Action OnTwistReady;
        public event Action<int> OnCaseResolved;

        private const int CasesBeforeTwist = 3;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void ResolveCurrentCase()
        {
            CasesResolved++;
            OnCaseResolved?.Invoke(CasesResolved);

            if (CasesResolved >= CasesBeforeTwist && !TwistTriggered)
            {
                TwistTriggered = true;
                OnTwistReady?.Invoke();
            }
        }

        public void AdvanceDay()
        {
            CurrentDay++;
        }
    }
}
