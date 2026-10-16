This is a clean, consolidated update — read it in place of the other addendum files in Design/ (unity_dev_prompt_v2.md, v3.md, unity_dev_art_swap.md — ignore those, this supersedes them). Nothing here asks you to redo anything. Two things: a visual style change, and ready-to-paste text content. Keep moving on whatever you're currently doing first; apply this when you hit a natural break.

## 1. Visual style: drop pixel art, go painted noir — additive only

Everything you've built stays exactly as-is: all C# scripts, the persistent GameManager, case/Book/tool data assets, the 4 scenes, the newspaper/dialogue/Book/tool-select UI logic, and the hard color-switch rule for the purple accent (that's a narrative rule, not a pixel-art rule — it works unchanged with painted art). The art pipeline you already built for swapping sprites via the Inspector is exactly the mechanism this uses — nothing structural changes.

What changes is only the rendering look:

- **Drop:** 16 PPU / Point filter / flat 8-swatch pixel sprites, the Pixel Perfect package, the 320×180 native camera, the pixel-font plan.
- **Target:** hand-painted noir illustration, semi-realistic — references **#20** (soft painted shading, visible brush texture), **#22** (detective at a desk, venetian-blind light — direct template for the hotel room), **#25** (dramatic directional silhouette light). Not #12/#15 (pixel art — wrong technique for this pass).
- **Camera:** swap to a normal camera setup sized for however the painted art reads best — no pixel snapping needed.
- **Font:** a clean serif or typewriter-style font for Book/case-file text instead, normal anti-aliasing.
- **Color:** same family as an anchor (navy/near-black shadow, amber/orange light, cream paper, reserved purple for supernatural tells only) but not locked to 8 exact hex values anymore — real gradients and tonal range within that family now.
- **Lighting:** one directional light source per room, real cast shadow across the scene — this is what makes it read like the references, put most of the effort here.
- **Dutch:** face stays shadowed under the hat brim (deliberate — identity undefined until the twist), everything else can be as rendered as the references.
- **The old pixel sprites in `Art/` (Dutch blockout + 3 clue sprites) are superseded** — don't bother importing or swapping those in, new painted versions are coming. Not worth your time right now.

Priority order for new art, swap in as it's ready (don't block other work waiting on it): Dutch → hotel room 414 background → the other 3 scene backgrounds → clue objects (these can stay simple/iconic even in the new style, small objects don't need the realism budget).

## 2. Book entries — ready to paste into the Inspector now

Journal-voice, in Dutch's hand. Drop these straight into the Book-page text fields for the 4 real cases — no rewriting needed on your end, just paste:

**THE FAMISHED** (Harbor Street Diner, kitchen)
Signs: cold rot under the cold-storage smell. Lights that won't hold steady — it pulls power like it pulls everything else, more than it needs, like it forgot how to stop. Claw marks on the inside of the pantry door. Inside. Something that wants in claws at the outside. Something that already got what it wanted claws to get out — and didn't make it.
Fix: A line of rock salt. Has to be from that kitchen specifically — whatever was in the shaker the night it died hungry. Store-bought doesn't hold. I don't know why. I've stopped asking why about half of what's in this book.
*Don't ask the owner before you take the salt. Just don't.*

**THE ECHO** (Lantern Lane, third-floor flat)
Signs: whispers that repeat the same four words, never more. Things moved an inch from where they were left — never thrown, never broken, just wrong, like someone tidied a room they used to live in. Cold spots that gather near glass. Mirrors, windows, a picture frame. Anything that used to hold a reflection it recognized.
Fix: Give it back what's its. A spoon from its own set — not silver in general, not a replacement, its spoon, the one still in a drawer somewhere in that flat. It's not angry. It's confused. Confused is worse. Angry wants to hurt you. Confused just wants you to stay until it figures out why you're not who it's waiting for.

**THE LODGER** (Caravanserai Inn, Room 9)
Signs: clocks stopped at the same minute, all of them, like the room agreed on a time and quit. Ashtrays that fill overnight with nobody smoking. A bed made every morning that nobody's slept in — made better than housekeeping makes it, like someone's still proud of the work.
Fix: *(unfinished — the ink stops here)*
*My watch stopped at 3:12 four nights running. I told myself it was the battery. I didn't check the battery.*

**[untitled]** (Room 414 — his own room, the Mirror-Born page)
This page was already written when I opened to it. Not my handwriting — my hand, moving the way it moves, but I didn't write this. Five of the six signs are already checked. I didn't check them. They checked themselves, one case at a time, every night I told myself it was nothing: a watch stuck at 3:12, an ashtray I don't remember filling, a bed I don't remember not sleeping in.
Fix: Every tool in the bag. Tried them all, in order, out of habit more than hope. Each one fails for its own reason — not because the page is wrong. Because I'm reading it wrong. Because I keep looking at the room for the thing that's supposed to be in it.
The sixth line, when everything else is crossed out, is one word: **Look.**

*(The two decoy pages, including Salt Widow, are already built with their own partial-overlap symptoms — if you send over their symptom lists I'll write matching journal-voice entries, same length/tone as above. Not blocking anything, just say the word.)*

## 3. Everything else from your last status report, quick answers

- **Dialogue stall after pantry-door clue:** your hypothesis (Editor frame starvation, not a bug) is reasonable since the same step passed in isolation. Close memory-heavy apps and rerun the full playthrough like you planned — no code change needed unless it stalls again clean.
- **3.6 GB free on C:** worth clearing more before the next full run — low disk can cause exactly this kind of instability.
- **Once a clean full playthrough passes:** move on to wiring in the art above as it comes in, and pasting in the Book text above. No other open item blocks you — go.

Push to the repo (`https://github.com/h4midAslan/hollow-saint`) at your next stable point.
