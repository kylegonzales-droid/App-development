# Mobile controls

**Pillar:** the thumb is the whole interface. Designed for one or two hands on a
phone on a bus — never a shrunken console pad.

## The core idea: one context button

Most mobile open-world games fail by putting eight buttons on the screen. This
game has **one action button**, on the right, whose meaning is set by context and
whose label always says what it will do.

| Context | Button reads |
|---|---|
| Near a vehicle | `GET IN` |
| Driving | `GET OUT` (hold) |
| Near a person | `TALK` |
| Near a door | `ENTER` |
| Near an item | `TAKE` |
| Near a shutter | `PULL DOWN` |
| Threat in range | `HIT` |
| Nothing nearby | Hidden |

Rules: exactly one primary action at a time, chosen by a scored priority
(distance × facing × story relevance). It **fades in** when available rather than
popping. A second, smaller button appears only when a genuine simultaneous
alternative exists — never more than two.

## On foot

```
┌──────────────────────────────────────────────┐
│                                              │
│                                              │
│                                              │
│   ╭─╮                                        │
│   │◉│  floating stick                        │   right half (blank area):
│   ╰─╯  appears where thumb lands             │   drag = camera peek
│                                      ( HIT ) │
│  [minimap]                            ⌄stam  │
└──────────────────────────────────────────────┘
```

- **Left thumb — floating stick.** Invisible until touched; origin is wherever
  the thumb lands inside the left zone. Never a fixed pad the thumb has to find.
- Walk / run by stick magnitude. No separate run button.
- **Right half — drag to peek**, springs back. Tap-through to the action button.
- **Vault, climb, squeeze** are automatic on contact. No button.

## Driving

**Do not use a stick for driving.** Recommended model:

- **Auto-accelerate.** The car goes forward; the player steers.
- **Arc steering** — a wide thumb zone across the bottom-left; horizontal
  position maps to steering angle with speed-scaled sensitivity.
- **Brake / reverse** — right-side button, hold.
- **Handbrake** — swipe up anywhere on the right. Tight turns, and the primary
  skill expression in driving.
- **Assist (default on)** — subtle steering assist to hold a lane, disengaging
  the instant the player fights it. Off in Settings for players who want it raw.

Auto-accelerate + arc steering is what makes mobile driving feel good; twin-stick
driving on a touchscreen does not work and should not be attempted.

## Combat

`Tap` strike · `Swipe` dodge · `Hold` guard. No combos to memorise. See
`07-combat.md`.

## Gestures reserved

Two-finger tap → phone. Pinch → minimap zoom. Edge swipe is **left to the
system** — never intercepted.

## Customisation (Settings, Zone 1)

- Stick: floating (default) / fixed / position and size
- **Left-handed mirror** — full layout flip
- Control opacity, and hide-when-idle
- Steering assist on/off; steering sensitivity
- Auto-accelerate on/off
- Haptic intensity: Off / Light / Full
- Hold-to-press duration (accessibility)
- Tap-instead-of-hold for every hold action (accessibility)

## Accessibility

Every control target ≥ 44×44 pt, with hit areas larger than the visuals. **Every
hold has a tap alternative.** No gesture requires more than one finger to
complete a critical path. No time-critical double-tap. Full Reduce Motion support
in the camera, which is a control surface as much as a visual one.

## Validation before it is "done"

Feel cannot be reviewed in a diff. Per `.claude/rules/verification.md`, the
control scheme is not done until it has been played **on a physical device, one
handed, standing up, on a moving bus, by someone who is not the developer.** That
test is the acceptance criterion for VS0.
