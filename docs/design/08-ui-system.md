# UI system

Governed by `.claude/rules/hig-for-games.md`. **Zone 1** (settings, onboarding,
purchases, permissions, errors, Game Center) is HIG-authoritative and should feel
native. **Zone 2** (HUD, phone, map, mission UI) is the game's own visual
language — bespoke by intent. The accessibility non-negotiables apply to both.

## HUD — minimal, diegetic, mostly absent

The default state of the screen is **the game**. HUD elements appear when they
have something to say and fade when they don't.

```
┌──────────────────────────────────────────────┐
│ ●  objective line                   £1,240   │  top: objective + money
│                                              │  (fade out when idle)
│                                              │
│                                              │
│                  [ game ]                    │
│                                              │
│                                              │
│  ╭────╮                                      │
│  │ ◔  │  minimap                      ( ⬤ )  │  bottom-L: minimap
│  ╰────╯                             context  │  bottom-R: context action
└──────────────────────────────────────────────┘
      ^ floating stick zone (invisible)
```

**Health** — no bar. Screen edges desaturate and a low pulse enters the audio mix
as damage accumulates. Recovery is visible as colour returning. Readable
peripherally, costs no screen space, and it is far more tense.

*Accessibility override:* a numeric/bar health readout is available in Settings
and is force-enabled when Reduce Transparency or Increased Contrast is on, since
a desaturation cue is unusable for some players. **Colour is never the only channel.**

**Stamina** — a thin arc around the context button, visible only while depleting
or recovering.

**Money** — pounds sterling, `£1,240`. Pence shown on small transactions.
Animates on change, then fades.

**Objective** — one line, top-left, plain language. "Get the van to the yard
before half eleven." Never a checklist.

**Minimap** — bottom-left, circular, rotates with heading, north pip on the rim.
Shows: streets, current objective, faction corner presence (when known), and at
night the **coverage cones of cameras** you have learned about. The minimap is a
progression surface: it starts sparse and fills in as Remi's knowledge becomes
the player's.

**Context action** — one button. See `11-mobile-controls.md`.

## The phone — the meta UI

*(Designed here; **not implemented** in the vertical slice.)*

An in-world smartphone is the container for everything that isn't real-time. It
replaces a conventional pause menu for game content and is strong characterisation.

| App | Contents |
|---|---|
| **Messages** | Job offers, threats, Dami checking in, Mikey posting things he shouldn't. Threaded, with unread badges. Some messages are time-limited. |
| **Contacts** | Known characters, relationship state, who will currently take a call |
| **Map** | Full-screen map, waypoints, discovered shortcuts, camera positions |
| **Jobs** | Available work from both factions, with pay and standing consequence shown |
| **Gallery** | Photos taken as evidence — a mission verb, and a collectible |
| **Feed** | A fictional social app. Mikey's posts are a live tension gauge. Purely narrative. |
| **Wallet** | Money, debts, what's owed to whom |

**Rules:** the phone is *diegetic* — Remi physically holds it, the world does not
hard-pause while it's open (it slows, it does not stop), and being on your phone
in the wrong place is itself a risk. It must be closeable in one gesture.

## Screens

**Zone 2 (game-styled):** loading, mission briefing, mission complete, mission
failed, map, phone, faction standing, character info, inventory.

**Zone 1 (HIG-native):** settings, accessibility, audio, controls, account and
Game Center, privacy, legal, purchase flows (if any), error and offline states.

**Mission complete** — restrained. Payment, standing change, one line of
consequence. No confetti, no rank fanfare. The tone is a receipt, not a trophy.

**Mission failed** — never a wall. States what happened in one line, offers
instant retry from the last beat (not the mission start), and never blames the
player in its wording.

**Pause** — does not fake-pause the world in the phone; the system pause menu is
a real pause and is HIG-standard.

## Notifications

Bottom-centre, one at a time, queued, auto-dismiss. Understated British register:
"Dami's called twice." / "Col wants a word." / "That van's known now."

## Typography

- **Display** (titles, faction marks): a condensed grotesque with strong
  character — the kind of type on a British shopfront or a transport sign.
- **UI** (everything readable): a clean, high-legibility sans. Must scale with
  Dynamic Type in Zone 1 and remain legible at default in Zone 2.
- **Numerals:** tabular everywhere money or time appears, so they stop jittering.

## Accessibility — binding

Per `.claude/rules/hig-for-games.md`, in both zones:

- 44×44 pt minimum for every tappable element **including bespoke HUD controls**;
  hit areas exceed the visual where useful.
- VoiceOver labels and traits on all menus, HUD, settings, results, and the phone.
  Real-time gameplay is the documented exemption; everything around it is not.
- Dynamic Type throughout Zone 1; Zone 2 legible at default and never clipping.
- **Reduce Motion**: no screen shake, no parallax, no camera punch, no bounce
  transitions. Cuts and short fades. **The game stays fully playable.**
- **Reduce Transparency / Increased Contrast**: opaque HUD backplates, and the
  numeric health readout forced on.
- Colour never the sole channel — faction identity is colour **plus** the swan/star
  mark **plus** silhouette.
- Safe areas, Dynamic Island and home indicator respected; nothing interactive
  underneath.
- Full playability **muted** — no information exists only in audio.
- Haptics additive only, and respect system settings.
