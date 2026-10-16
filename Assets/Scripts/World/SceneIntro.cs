using UnityEngine;
using HollowSaint.Core;
using HollowSaint.Investigation;

namespace HollowSaint.World
{
    /// <summary>Tells the Game which scene just loaded so it can play the right opening beat.</summary>
    public class SceneIntro : MonoBehaviour
    {
        [Tooltip("The case this scene belongs to. Empty for the hotel.")]
        [SerializeField] CaseData sceneCase;
        [SerializeField] bool isHotel;

        void Start()
        {
            if (Game.Instance != null) Game.Instance.BeginScene(sceneCase, isHotel);
        }
    }
}
