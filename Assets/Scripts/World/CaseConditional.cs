using UnityEngine;
using HollowSaint.Core;
using HollowSaint.Investigation;

namespace HollowSaint.World
{
    /// <summary>
    /// Shows some objects and hides others while a particular case is active. Put it on an
    /// always-active parent; the targets can be inactive.
    /// </summary>
    public class CaseConditional : MonoBehaviour
    {
        [SerializeField] CaseData whileCase;
        [SerializeField] GameObject[] showDuring = new GameObject[0];
        [SerializeField] GameObject[] hideDuring = new GameObject[0];

        void Awake() => Refresh();

        void OnEnable()
        {
            if (Game.Instance != null) Game.Instance.Changed += Refresh;
            Refresh();
        }

        void OnDisable()
        {
            if (Game.Instance != null) Game.Instance.Changed -= Refresh;
        }

        void Refresh()
        {
            bool active = Game.Instance != null && whileCase != null && Game.Instance.ActiveCase == whileCase;
            foreach (var go in showDuring) if (go != null) go.SetActive(active);
            foreach (var go in hideDuring) if (go != null) go.SetActive(!active);
        }
    }
}
