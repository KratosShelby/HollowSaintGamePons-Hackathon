Quick clarification on the art direction already sent (unity_dev_final.md) — no new work, just sharpening the bar on the painted-art pass you're already doing.

"Looks like the references" specifically means it should read as dimensional/3D-ish even though it's 2D painted art — not flat. Two concrete things get you there:

1. **Volumetric shading, not flat color fills.** Every surface — Dutch's coat, a chair, a doorframe — should be shaded like it actually has form: light wrapping around curves, a core shadow, a reflected-light edge where it meets another surface. Refs #20/#22/#25 are all 2D paintings, not 3D renders, but they model light this way, which is exactly why they read as "real" instead of flat.
2. **Real perspective in the backgrounds.** Rooms shouldn't be flat orthographic boxes — give them actual vanishing-point perspective (ref #22's desk-and-blinds composition is a clean example: the blinds, the desk edges, all converging, not drawn flat-on). This alone does a lot of the "3D-ish" work.

Same scope, same priority order as before (Dutch → hotel room background → other scenes → clue objects). This is just the quality bar for that pass, not an additional task.
