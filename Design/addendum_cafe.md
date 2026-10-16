# Addendum — Cafe location (read after the main build prompt)

Small scope addition, not a redesign. No new mechanics — the newspaper-hub interaction you've already built just moves to a new room.

**What's changing:**
- The hotel room stays exactly as specced: home base, mirror, opening beat. Remove the newspaper interactable from the hotel room if you've already placed it there.
- Add one new scene: **the cafe**. Dutch gets a drink, sits at a table, and the newspaper hub interaction (the one that was on the hotel desk) lives here instead. Same newspaper data/UI, same "click the real lead to load that case scene" logic — only the location and surrounding room art changes.
- Small, cramped room again — don't scope up. A table, a chair, 1-2 background silhouette props (other patrons, static, no AI/dialogue needed) for atmosphere, one light source (a hanging bulb works well, keeps it cheap — no venetian-blind rig needed here, that's the hotel room's signature look, keep the cafe visually distinct).
- Flow: Hotel room (wake, mirror) → transition/load → Cafe (read paper, pick lead) → transition/load → that day's case scene. A simple scene-load trigger (walk to a door, or a straight scene transition on leaving the hotel) is enough — no overworld/map needed.

**Build order impact:** slot this in alongside your existing Step 3 (newspaper hub + opening scene) — same step, just two rooms instead of one. Nothing else in the build order changes.

If you're already past this point and the newspaper's wired into the hotel desk, don't rip it out under time pressure — moving its *scene placement* later is a 10-minute job once the core loop works. Prioritize whatever's currently in progress first.
