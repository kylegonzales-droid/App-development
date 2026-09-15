# Vehicles

Vehicles are how you cross the manor, and the one-way system is what makes
crossing it interesting. Driving is **not** a separate minigame bolted on — it is
the primary traversal verb outside the pedestrianised core.

## Design stance

- **Few vehicles, each distinct.** Eight to twelve classes at ship, not eighty
  reskins. Every class should change how you route.
- **Handling is readable, not simulated.** Arcade physics with a strong sense of
  weight. The player must be able to predict a corner from the silhouette.
- **All vehicles are fictional.** No real manufacturer, model, badge, grille or
  silhouette is reproduced. Designs are generic-British-road archetypes.
- **Right-hand drive, left-hand traffic.** Non-negotiable for the setting.

## Classes

| Class | Role | Handling | Where it lives |
|---|---|---|---|
| **Courier moped** | Starting vehicle | Nimble, weak, uses cycle lanes and the towpath | Everywhere |
| **E-bike** | Five Star signature | Silent, fastest through culverts and alleys, no heat from cameras | Greenside, Hogsmill |
| **Hatchback** | The default car | Balanced, forgettable, blends into traffic | The Loop, residential |
| **Estate car** | Load-carrying | Heavy, stable, boot capacity matters for jobs | Back of House |
| **Transit-type van** | Quayside signature | Slow, tough, high seating position sees over traffic | The Reach, yards |
| **Flatbed / recovery truck** | Story and heists | Very slow, pushes things out of the way | Verrall's Motors |
| **Black cab** | Disguise | Mediocre, but police scrutiny drops sharply | Arrivals |
| **Double-decker bus** | Set-piece only | Unwieldy, unstoppable, comedic | The Loop |
| **Performance hatch** | Late-game reward | Fast, twitchy, attracts attention | Earned |
| **Police response car** | Pursuit | Faster than most, drives the Loop correctly | — |

**The cycle-lane/towpath rule is the key idea:** two-wheeled vehicles can use the
towpath, culverts and the pedestrianised core's edges. Four-wheeled cannot. That
single asymmetry makes vehicle choice a *routing* decision rather than a stats
decision, and it is derived directly from real Kingston.

## Handling model

Arcade, tuned per class from a small data table — not a physics rig per vehicle.

```
mass, enginePower, brakeForce, gripFront, gripRear,
steerRateAtSpeed[], turnRadiusMin, handbrakeGripLoss
```

Top-down 2D means handling can be **kinematic with a slip term** rather than a
full rigid-body simulation: far cheaper, far more controllable, and it keeps the
frame budget intact. `SKPhysicsBody` is used for collision response, not for
driving the wheels.

## Damage

Cosmetic and functional, three stages: clean → knocked about → smoking.
Functional damage affects grip and top speed only. No destruction modelling, no
dismemberment of vehicles, no explosions as a routine outcome — a burning car in
Kingston is a *story event*, not Tuesday.

## Heat interaction

Vehicles carry heat, not just the player. A vehicle that has been seen becomes
"known" for a period: police recognise it, and NPCs react to it.

Shedding heat:
- **Respray** at Verrall's Motors (Back of House) — costs money, clears vehicle heat.
- **Park and walk** — abandon it, heat stays with the vehicle.
- **Swap plates** — cheaper, partial, temporary.

This makes Back of House a functional destination rather than set dressing.

## Explicitly deferred

Vehicle customisation beyond respray, vehicle ownership fleets, racing
progression, passenger systems, fuel. The data table above is shaped so these can
be added without rework.
