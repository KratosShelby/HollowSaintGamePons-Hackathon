You are the technical lead and sole engineer on a game project called Hollow Saint. You have full ownership of the implementation. Read this completely before starting — it contains everything you need. Do not stop to ask questions; where something isn't specified, make the reasonable call yourself and keep moving. Report back only when a milestone below is fully working, or if you hit something that actually blocks progress (not just an open design choice).

This is a revised version of the brief you already have — most of it is unchanged. What's new: two locations instead of one (hotel + cafe), concrete lighting/rendering technique pulled from reference art, and a GitHub repo to push into. If you're mid-build on something from the original prompt, finish that first — none of this should interrupt work in progress.

## The game

A short narrative investigation game. **Dutch Marlowe**, an American monster hunter, is stuck in Baku on a layover that keeps getting delayed. Bored, restless, killing time. He carries **The Book** — a bestiary of every creature he's hunted, each entry listing symptoms and a specific, sometimes strange, method to deal with it. Not a generic weapon — something tied to that creature's own history.

## Locations (changed — was one room, now two)

**Hotel room (home base):** where Dutch sleeps, the opening beat plays out, and the mirror lives. Small, cramped, slightly off-center rather than symmetric. One key light source — a venetian-blind window — casting hard directional light/shadow stripes across the room. Two interactables: a **mirror** (short throwaway internal-monologue line, pure flavor, no mechanical weight yet — it matters later, don't forget it exists) and nothing else mechanically; no newspaper here anymore.

**Cafe (hub — new location):** a short walk from the hotel. Dutch gets a drink, sits at a table, reads the day's paper — this is where the newspaper-hub interaction lives now (moved off the hotel desk). Small, cheap room: a table, a chair, 1-2 static background silhouette props (other patrons — no AI, no dialogue, just atmosphere), one light source — a single hanging bulb overhead, not the venetian-blind rig (keep the two rooms visually distinct: hard blind-stripes in the hotel, a soft pool of light under one bulb in the cafe).

**Flow:** Hotel room (wake, mirror) → scene transition → Cafe (read paper, pick the real lead) → scene transition → that day's case scene.

## Core loop, every case

1. **Newspaper (hub, at the cafe):** a page with a few flavor headlines (atmosphere only) plus one bolded real lead, written with a location/time detail baked into the text (e.g. "reported near 2 AM, Harbor Street"). Clicking the real lead sends Dutch to that scene.
2. **Investigate:** walk around, interact with 2–3 clue objects (wounds, residue, disturbed items). Each logs a "symptom" to a persistent case file.
3. **The Book:** open it, cross-reference collected symptoms, find the matching bestiary entry.
4. **Act:** pick the correct method for that entry's specific weakness.
5. **Confront and resolve:** apply it, case closes, counter increments, move to the next.

## The bestiary, build in this order

1. **The Famished** (zombie-type) — symptoms: cold rot smell, flickering electronics, claw marks on the inside of doors. Weakness: a salt line, specifically rock salt from the kitchen where it died.
2. **The Echo** (spirit) — symptoms: repeated whispers, objects slightly moved, cold spots near mirrors. Weakness: bound to a forgotten possession — a silver spoon from its own set, found in the scene.
3. A third case of your own design — pick a creature whose symptoms quietly half-match Dutch's own habits, planted so it reads as coincidence on a first playthrough, not yet a reveal.
4. **The Mirror-Born** (final case, the twist): Dutch opens The Book expecting a new entry, and it's already written — in his own handwriting, describing him. None of the usual tools work. The listed weakness is just: *look in the mirror.* The deduction system the player has trusted for three cases has been building a file on him the whole time. This must land as something the player *does* — reuse the same Book/tool-select interaction one more time, but subvert what "choosing the weakness" means here.

**Scope discipline:** one scene per case, keep it small. Matching symptoms to a bestiary entry should feel satisfying and clear, not a separate hard puzzle. No day/night simulation — the newspaper's bolded headline is the only "time" device, just text. Add one recurring NPC only after the core loop is fully proven, not before.

## Art direction (locked — follow exactly, don't improvise a different style)

Duotone pixel noir. Palette (exact hex, use only these 8 — add tints/shades by mixing these via dithering, not new colors):
- `#0d0d1a` near-black / deepest shadow
- `#1b1b2f` base navy (primary silhouette color)
- `#2e2e52` lighter navy / shadow midtone
- `#7a4a2b` coat brown, shadow side
- `#c97d3f` coat brown, lit side / rim-light
- `#f2a65a` amber light glow (every light source in the game uses this)
- `#f4e9d8` cream highlight / paper / newspaper / Book pages
- `#9d4edd` reserved supernatural accent — **never used in the normal world.** Only for clue glow, entity tells, and Dutch's reflection in the final mirror moment (the first and only time it appears on him). This is a gameplay signal as much as a color rule.

**Lighting technique (new — concrete, not just a mood word):** the realism gap between a flat-lit room and the reference art comes from one technique: a single directional light source casting **hard-edged dithered shadows** across the whole scene, not flat-colored props on a flat floor. Concretely:
- Pick one light source per room (hotel: venetian-blind window: cafe: hanging bulb) and let it cast actual shadow stripes/pools across the floor and walls using a 1-2px dither pattern (checkerboard/Bayer-style) at the shadow edge instead of a hard flat color boundary.
- Spend this technique's budget on the **room backgrounds**, not every small prop — clue objects and icons stay simple/flat, that's correct and intentional, not a shortcut.
- If you have a 2D lighting package available (URP 2D Lights, or similar) and it's not a time sink to wire up, real-time directional lighting with a dithered shadow shader is even better than hand-painted static shadows — use your judgment on which is faster given what's already built.

Technical setup: native render resolution ~320×180, Unity's **2D Pixel Perfect** package on the main camera, texture import settings **Point (no filter)** / **Compression: None**, Pixels Per Unit locked to one number project-wide (use 16). Character/prop sprites read as silhouette first — Dutch's face stays shadowed under his hat brim, no facial detail needed or wanted.

A first-pass blockout of Dutch (32×32, rough silhouette) and three Famished clue-object sprites (floor stain, flickering radio, clawed door — each already using the violet "something's off" accent) exist in `Art/` in the repo, meant to be replaced by hand-refined art later. Keep sprites data-driven (SpriteRenderer references via Inspector, not hardcoded) so placeholder art swaps in without code changes.

## Build order

1. **Kernel loop:** 2D project, player with 4-directional top-down movement, an interact system (proximity + keypress), one clue object that logs a symptom on interact, a minimal Book UI showing one bestiary entry, a tool-select UI, confront/resolve logic, a case counter. Fully playable end to end before anything else. Ugly/placeholder art is correct at this stage.
2. Reskin the kernel for **The Famished** and **The Echo** using the real content above.
3. Build the **cafe hub scene** (newspaper interaction + background silhouette props) and the **hotel room opening scene** (mirror interactable, venetian-blind lighting), connected by a scene transition.
4. Build the **third case**, then **The Mirror-Born** twist scene last — it depends on the pattern the first three cases establish.
5. Only after the full loop (open → investigate → deduce → act → resolve, across all 4 cases) works end to end: style pass using the exact palette and dithered-lighting technique above, pixel-perfect camera setup, and swap in real art as it arrives.

Use whatever Unity CLI/MCP tooling you have available to drive the Editor directly — create objects, wire components, build, test — rather than leaving it as manual GUI work wherever that tooling supports it. Compile-check and actually playtest each milestone yourself before calling it done; don't assume something works because it compiles.

## Repo

A GitHub repo now exists for this project: `https://github.com/h4midAslan/hollow-saint` (private). It has the design docs, art direction, the four sprites in `Art/`, and the starter C# scripts as the initial commit. Connect your project to it:

```
git init                                              (only if not already a repo)
git remote add origin https://github.com/h4midAslan/hollow-saint.git
git pull origin master --allow-unrelated-histories    (merges what's already pushed)
git add -A
git commit -m "Unity project setup + progress"
git push -u origin master
```

Push at a stable point once you're there — don't interrupt a milestone to do this mid-build.

Start now. Work through the build order without pausing for confirmation between milestones unless you're genuinely blocked.
