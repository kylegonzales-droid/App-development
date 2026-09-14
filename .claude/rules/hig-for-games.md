# Apple HIG for a game — how to apply it without flattening the product

The `apple-hig` skill and the HIG references are installed because a game still
ships inside iOS and must behave like a well-made iOS product at its edges. They
are **not** a mandate to make the game look like Settings.

## Split the product into two zones

**Zone 1 — system-facing surfaces. HIG is authoritative here.**
Onboarding, settings, account, purchases, permission prompts, notifications,
share sheets, error and offline states, legal screens, Game Center surfaces,
anything the App Store reviewer will open first.

These should feel native, predictable, and unsurprising. Standard navigation,
standard controls, standard gestures. Do not reinvent a picker.

**Zone 2 — the gameplay surface. HIG informs, art direction decides.**
The playfield, HUD, characters, VFX, transitions, celebration moments, the game's
typography and palette.

Here a custom visual language is the point. A game is *expected* to have bespoke
buttons, custom fonts, diegetic UI, and motion with personality. Do not replace a
deliberate game aesthetic with system defaults and call it a HIG fix.

## What is non-negotiable in both zones

These are accessibility and platform-contract obligations, not styling opinions:

- **Touch targets** — minimum 44×44 pt for anything tappable, including bespoke
  HUD controls. Hit area may exceed the visual, and often should.
- **VoiceOver** — every interactive control has a meaningful label and correct
  traits. Gameplay that is genuinely visual-motor may be exempt, but menus,
  HUD, settings and results screens are not.
- **Dynamic Type** — all *text* in Zone 1 scales. Zone 2 may use fixed type
  inside the playfield where layout is pixel-critical, but must remain legible
  at default sizes and must not clip.
- **Reduce Motion** — honour it. Replace parallax, screen shake, large-scale
  camera moves and bounce transitions with cuts or short fades. The game must
  stay fully playable with it on.
- **Reduce Transparency / Increased Contrast** — honour both in HUD and menus.
- **Colour is never the only channel.** Pair it with shape, icon, position or
  text — this covers colour-blind players and it is also good game design.
- **Safe areas, notch, Dynamic Island, home indicator** — nothing interactive
  or informational underneath them.
- **Interruptions** — calls, notifications, backgrounding, low power. The game
  pauses cleanly and resumes without losing state.
- **Audio** — respect the silent switch and other audio sessions. Never assume
  sound is on; the game must be fully playable muted.
- **Haptics** — additive only, never load-bearing, and respect system settings.

## How to use the skill

Run `apple-hig` and `ui-review` against Zone 1 and against HUD/menu work in
Zone 2. When a HIG recommendation collides with a deliberate game-design choice
in Zone 2, state the tension and let the user decide. Do not silently apply the
system default, and do not silently ignore the guideline.
