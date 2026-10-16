using UnityEngine;
using HollowSaint.Core;

namespace HollowSaint.World
{
    /// <summary>
    /// Hard-switches a sprite between two palette colours (or on/off) at random intervals.
    /// No fades, so nothing ever renders an off-palette in-between colour.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class ColorBlink : MonoBehaviour
    {
        [SerializeField] Color onColor = Palette.Amber;
        [SerializeField] Color offColor = Palette.NavyLight;
        [Tooltip("Hide the sprite instead of switching to offColor.")]
        [SerializeField] bool hideWhenOff;
        [SerializeField] Vector2 onSeconds = new Vector2(0.05f, 0.6f);
        [SerializeField] Vector2 offSeconds = new Vector2(0.03f, 0.15f);

        SpriteRenderer sprite;
        bool lit = true;
        float nextSwitch;

        void Awake() => sprite = GetComponent<SpriteRenderer>();

        void OnEnable()
        {
            lit = true;
            Apply();
            nextSwitch = Time.time + Random.Range(onSeconds.x, onSeconds.y);
        }

        void Update()
        {
            if (Time.time < nextSwitch) return;
            lit = !lit;
            Apply();
            var range = lit ? onSeconds : offSeconds;
            nextSwitch = Time.time + Random.Range(range.x, range.y);
        }

        void Apply()
        {
            if (hideWhenOff)
            {
                sprite.enabled = lit;
                sprite.color = onColor;
            }
            else
            {
                sprite.enabled = true;
                sprite.color = lit ? onColor : offColor;
            }
        }
    }
}
