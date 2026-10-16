using UnityEngine;
using UnityEngine.Rendering.Universal;
using HollowSaint.Core;

namespace HollowSaint.World
{
    /// <summary>
    /// Hard-switches a sprite between two colours (or on/off) at random intervals. No fades:
    /// the supernatural accent is always either fully on or off.
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
        [Tooltip("Optional light that switches with the sprite (a flickering lamp that actually flickers the room).")]
        [SerializeField] Light2D linkedLight;
        [SerializeField, Range(0f, 1f)] float lightOffFraction = 0.15f;

        SpriteRenderer sprite;
        float lightOnIntensity;
        bool lit = true;
        float nextSwitch;

        void Awake()
        {
            sprite = GetComponent<SpriteRenderer>();
            if (linkedLight != null) lightOnIntensity = linkedLight.intensity;
        }

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
            if (linkedLight != null) linkedLight.intensity = lit ? lightOnIntensity : lightOnIntensity * lightOffFraction;
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
