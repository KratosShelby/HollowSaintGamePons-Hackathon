Addendum — visual style swap. Read this as an addition to what you've already built, not a redo. Your systems, data assets, scene structure, and the persistent GameManager are all staying exactly as they are. This only touches how things are rendered.

## What's NOT changing (don't touch any of this)

- All C# scripts, the persistent GameManager, case/Book/tool data assets, the 4 scenes you built, the newspaper/dialogue/Book/tool-select UI logic.
- The hard color-switch rule for the purple accent (reserved for clue glow, creature tells, and Dutch's reflection — no fades). This is a narrative/gameplay rule, not a pixel-art rule — it works identically with painted art. Keep it exactly as built.
- The data-driven art pipeline you already have (sprite swaps via the Inspector without touching colliders) — this is exactly the mechanism we're about to use, so nothing structural changes there either.

## What IS changing: the look

Drop the pixel-art rendering entirely — no more 16 PPU / Point filter / flat 8-color-swatch sprites. Target look is hand-painted noir illustration, semi-realistic: references **#20** (typewriter/desk illustration — soft painted shading, visible brush texture), **#22** (detective at a desk, venetian-blind light — this is the direct template for the hotel room's lighting), and **#25** (stairwell silhouette, dramatic directional light). Not references #12/#15 — those are pixel art, wrong technique for this pass.

Concretely:

1. **Import settings:** new art imports with normal filtering (not Point/nearest-neighbor), no forced 16 PPU, no "no compression" pixel-art requirement. Standard sprite import is fine.
2. **Camera:** skip the Pixel Perfect package and the 320×180 native-resolution camera entirely — that was a pixel-art-specific requirement, drop it along with the pixel look. Use a normal camera setup sized for however the painted art reads best.
3. **Font:** also drop the "pixel font" plan from the original Step 5 — instead use a clean serif or typewriter-style font for Book/case-file text (fits the noir case-file look directly, e.g. matching reference #24's document style), and make sure it anti-aliases normally rather than forcing crisp pixel edges.
4. **Color family stays the anchor, not a hard swatch:** cool navy/near-black shadow, warm amber/orange light, cream/off-white paper and highlights, reserved purple only for the supernatural tells above. Within painted art you can use real gradients and tonal range inside that family — you're not locked to the 8 exact hex values anymore, those were a pixel-art constraint.
5. **Lighting technique:** one directional light source per room casting real shadow across the scene — venetian blinds in the hotel room (ref #22), whatever reads best per scene elsewhere. This is the technique that makes it look like the references instead of generic flat art — most of the visual effort should go here.
6. **Character:** Dutch's face stays shadowed under his hat brim — no detailed features, that's a deliberate identity-undefined-until-the-twist choice, not a fidelity shortcut. Everything else about him (coat, posture, lighting) can be as rendered as the references.

## How to actually produce the art

This is the real work of this pass — sourcing/painting art, not code. Whatever's fastest for you: hand-paint it, use an AI image tool and then hold the result to the same bar as everything else (no generic/off-model/mismatched-style results — reject and redo anything that looks obviously generated rather than shipping it to hit time), or a mix. Needed, roughly in priority order:
1. Dutch (idle + walk, replacing the current pixel blockout)
2. Hotel room 414 background (the highest-visibility scene — this is what a judge sees first)
3. The other three scene backgrounds (Harbor Street Diner kitchen, Lantern Lane flat, Caravanserai Inn Room 9)
4. Clue objects per scene (can stay simpler/iconic even in the new style — they're small, read-at-a-glance objects, not where the realism budget goes)

Swap pieces in as they're ready — the pipeline already supports that without touching code. No need to block other work waiting on art; keep testing/fixing gameplay in parallel with whatever placeholder is currently in each slot.

Push to the repo (`https://github.com/h4midAslan/hollow-saint`) once you're at a stable point, same as before.
