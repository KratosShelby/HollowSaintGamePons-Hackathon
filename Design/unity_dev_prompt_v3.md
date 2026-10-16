You are the technical lead and sole engineer on a game project called Hollow Saint. You have full ownership of the implementation. Read this completely before starting — it contains everything you need. Do not stop to ask questions; where something isn't specified, make the reasonable call yourself and keep moving. Report back only when a milestone below is fully working, or if you hit something that actually blocks progress (not just an open design choice).

This continues the brief you already have — story, mechanics, and build order are unchanged. What's different in this version: the art direction. Drop the "strict low-res pixel grid" framing entirely — go for a hand-crafted noir illustration look instead, closer to the reference art than to retro pixel-game nostalgia. If you're mid-build on code/systems, none of this should interrupt that — this only changes how things are drawn and rendered, not what you're building.

## The game

A short narrative investigation game. **Dutch Marlowe**, an American monster hunter, is stuck in Baku on a layover that keeps getting delayed. Bored, restless, killing time. He carries **The Book** — a bestiary of every creature he's hunted, each entry listing symptoms and a specific, sometimes strange, method to deal with it. Not a generic weapon — something tied to that creature's own history.

## Locations

**Hotel room (home base):** where Dutch sleeps, the opening beat plays out, and the mirror lives. Small, cramped, slightly off-center rather than symmetric. One key light source — a venetian-blind window — casting hard directional light/shadow stripes across the room. Two interactables: a **mirror** (short throwaway internal-monologue line, pure flavor, no mechanical weight yet — it matters later) and nothing else mechanically; no newspaper here.

**Cafe (hub):** a short walk from the hotel. Dutch gets a drink, sits at a table, reads the day's paper — this is where the newspaper-hub interaction lives. Small, cheap room: a table, a chair, 1-2 static background silhouette props (other patrons — no AI, no dialogue, just atmosphere), one light source — a single hanging bulb overhead, visually distinct from the hotel's hard blind-stripes (soft pool of light instead).

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

## Art direction — revised (read carefully, this replaces the old pixel-grid spec)

**No pixel art.** Drop sprite-sheet/retro-pixel styling entirely — no blocky silhouettes, no hard pixel edges, no nearest-neighbor scaling. The target is **hand-painted noir illustration, semi-realistic** — the look of references #20, #22, #25 (soft digital painting, visible brush/texture strokes, naturalistic proportions and lighting), not references #12/#15 (which are pixel-art and are *not* the technique to follow — fine for palette/mood only, ignore their blocky rendering). Think illustrated graphic novel / noir concept art, not a sprite sheet.

**Color family (anchor, not a hard swatch rule):** cool navy/near-black for shadow and base tone, warm amber/orange for every light source, cream/off-white for paper and highlights, and one reserved color — sickly violet — that appears *only* in the supernatural world: clue glow, entity tells, and Dutch's reflection in the final mirror moment (first and only time it appears on him). Stay inside this family; vary tone, saturation, and gradient freely within it.

**Rendering approach:** render at whatever resolution and fidelity it takes to look like the references — full-color digital painting with real brushwork, soft directional light, and textured/dithered or soft-edged shadow (not a hard flat color boundary). One light source per room, casting real directional shadow across the whole scene (ref #22's venetian-blind stripes, ref #20's single-lamp pool). This lighting technique is what separates "looks like the references" from "looks like generic placeholder art" — spend the most effort here.

**Characters stay silhouette/shadow-first where it matters:** Dutch's face stays shadowed under his hat brim — no detailed facial features needed, it's a deliberate design choice (quietly on-theme: his identity is undefined until the twist), not a fidelity shortcut. Everything else about him — coat, posture, lighting on the fabric — can be as rendered/realistic as the references.

**What this does NOT mean:** no generic asset-store packs, no mismatched styles between scenes, nothing that reads as obviously AI-generated or off-model (inconsistent anatomy, mismatched lighting logic, generic "stock art" composition). Every piece should look like it belongs to the same hand, same palette, same light logic. If you're generating art rather than painting it by hand, hold it to that bar and reject/redo anything that doesn't clear it rather than shipping it to hit a deadline.

The rough blockout of Dutch and the three Famished clue sprites currently in `Art/` in the repo are flat-color pixel placeholders made to unblock early development — they predate this direction change and are not the target look. Treat them as palette/layout reference only; replace them as real art comes in. Keep sprites/art data-driven (SpriteRenderer references via Inspector, not hardcoded) so art keeps swapping in without code changes.

## Build order

1. **Kernel loop:** 2D project, player with 4-directional top-down movement, an interact system (proximity + keypress), one clue object that logs a symptom on interact, a minimal Book UI showing one bestiary entry, a tool-select UI, confront/resolve logic, a case counter. Fully playable end to end before anything else. Ugly/placeholder art is correct at this stage.
2. Reskin the kernel for **The Famished** and **The Echo** using the real content above.
3. Build the **cafe hub scene** (newspaper interaction + background silhouette props) and the **hotel room opening scene** (mirror interactable, venetian-blind lighting), connected by a scene transition.
4. Build the **third case**, then **The Mirror-Born** twist scene last — it depends on the pattern the first three cases establish.
5. Only after the full loop (open → investigate → deduce → act → resolve, across all 4 cases) works end to end: style pass using the art direction above — directional dithered lighting per room, palette consistency check across every scene, swap in refined art as it arrives.

Use whatever Unity CLI/MCP tooling you have available to drive the Editor directly — create objects, wire components, build, test — rather than leaving it as manual GUI work wherever that tooling supports it. Compile-check and actually playtest each milestone yourself before calling it done; don't assume something works because it compiles.

## Repo

`https://github.com/h4midAslan/hollow-saint` (private) — has design docs, art direction, starter sprites/scripts as the initial commit. Connect your project:

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
