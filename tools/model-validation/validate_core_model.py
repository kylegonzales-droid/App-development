#!/usr/bin/env python3
"""
Reference implementation of Manor.Core's numeric models, ported line-for-line from
the C# in unity/Manor/Assets/Manor/Core/.

WHY THIS EXISTS
    The container this project is developed in has no .NET SDK (the network policy
    blocks Microsoft's distribution host) and no Unity, so the C# cannot be compiled
    or run here. The *arithmetic*, however, can be: this script mirrors it exactly
    and asserts the same properties the NUnit tests assert.

    A pass here means the MODEL is right. It does not mean the C# compiles.
    Run the real NUnit suite in the Unity Editor for that.

    If you change the C#, change this too, or delete it. A reference model that has
    silently drifted from the code is worse than no reference model.
"""
import math
import random
import sys
from dataclasses import dataclass

# ---------------------------------------------------------------- surfaces

REFERENCE_EVAPORATION_PER_SECOND = 0.0045
MIN_EXPOSURE_FACTOR = 0.25
MIN_DRAINAGE_FACTOR = 0.30


@dataclass(frozen=True)
class Material:
    name: str
    porosity: float
    absorption_rate: float
    drying_rate: float
    snow_affinity: float


ASPHALT      = Material("Asphalt",      0.75, 0.035, 1.00, 0.55)
WORN_ASPHALT = Material("WornAsphalt",  0.80, 0.042, 0.85, 0.60)
CONCRETE     = Material("Concrete",     0.70, 0.030, 1.05, 0.65)
BRICK        = Material("Brick",        0.85, 0.022, 0.55, 0.35)
GLASS        = Material("Glass",        0.02, 0.060, 2.40, 0.10)


@dataclass
class Patch:
    material: Material
    sky_exposure: float
    drainage: float
    wetness: float = 0.0


@dataclass(frozen=True)
class Drying:
    temperature_c: float
    wind_speed: float
    relative_humidity: float


TEMPERATE = Drying(15.0, 2.0, 0.6)


def clamp(v, lo, hi):
    return lo if v < lo else (hi if v > hi else v)


def evaporation_factor(c: Drying) -> float:
    temperature = clamp((c.temperature_c + 5.0) / 30.0, 0.05, 1.5)
    wind = 1.0 + c.wind_speed * 0.08
    humidity = clamp(1.0 - c.relative_humidity * 0.7, 0.2, 1.0)
    return temperature * wind * humidity


def wetting_delta(p: Patch, rain: float, dt: float) -> float:
    if rain <= 0.0 or dt <= 0.0:
        return 0.0
    return rain * p.sky_exposure * p.material.absorption_rate * dt


def drying_delta(p: Patch, c: Drying, dt: float) -> float:
    if dt <= 0.0:
        return 0.0
    exposure = MIN_EXPOSURE_FACTOR + (1.0 - MIN_EXPOSURE_FACTOR) * p.sky_exposure
    drainage = MIN_DRAINAGE_FACTOR + (1.0 - MIN_DRAINAGE_FACTOR) * p.drainage
    return (REFERENCE_EVAPORATION_PER_SECOND * p.material.drying_rate
            * exposure * drainage * evaporation_factor(c) * dt)


def advance(p: Patch, rain: float, c: Drying, dt: float) -> Patch:
    p.wetness = clamp(p.wetness + wetting_delta(p, rain, dt) - drying_delta(p, c, dt), 0.0, 1.0)
    return p


def seconds_to_dry(p: Patch, target: float, c: Drying) -> float:
    if p.wetness <= target:
        return 0.0
    per_second = drying_delta(p, c, 1.0)
    return math.inf if per_second <= 0 else (p.wetness - target) / per_second


def simulate(p: Patch, rain: float, seconds: float, step: float = 0.1) -> Patch:
    t = 0.0
    while t < seconds:
        advance(p, rain, TEMPERATE, step)
        t += step
    return p


def crown():  return Patch(ASPHALT, 1.00, 1.00)
def gutter(): return Patch(WORN_ASPHALT, 1.00, 0.05)
def arch():   return Patch(CONCRETE, 0.05, 0.20)


def soaked(p: Patch) -> Patch:
    p.wetness = 1.0
    return p


# ---------------------------------------------------------------- weather

PROFILES = {
    # name:          precip snow  cloud  vis    wind  humid dwell
    "Clear":        (0.00, 0.00, 0.10, 2000.0,  2.0, 0.45, 300.0),
    "Cloudy":       (0.00, 0.00, 0.55, 1600.0,  3.0, 0.60, 420.0),
    "Overcast":     (0.00, 0.00, 0.95, 1100.0,  3.5, 0.72, 600.0),
    "LightRain":    (0.30, 0.00, 0.95,  800.0,  4.0, 0.85, 300.0),
    "HeavyRain":    (0.85, 0.00, 1.00,  420.0,  6.5, 0.95, 240.0),
    "Storm":        (1.00, 0.00, 1.00,  260.0, 12.0, 0.97, 180.0),
    "Fog":          (0.00, 0.00, 0.80,   90.0,  0.8, 0.98, 360.0),
    "LightSnow":    (0.05, 0.35, 0.95,  500.0,  3.0, 0.88, 300.0),
    "HeavySnow":    (0.10, 0.90, 1.00,  220.0,  5.5, 0.92, 240.0),
}

TRANSITIONS = {
    "Clear":     [("Cloudy", .60), ("Overcast", .25), ("Clear", .15)],
    "Cloudy":    [("Overcast", .45), ("Clear", .25), ("LightRain", .20), ("Cloudy", .10)],
    "Overcast":  [("LightRain", .34), ("Cloudy", .30), ("Overcast", .20), ("Fog", .10), ("LightSnow", .06)],
    "LightRain": [("Overcast", .44), ("HeavyRain", .26), ("LightRain", .18), ("Cloudy", .12)],
    "HeavyRain": [("LightRain", .50), ("Overcast", .26), ("Storm", .14), ("HeavyRain", .10)],
    "Storm":     [("HeavyRain", .62), ("LightRain", .28), ("Overcast", .10)],
    "Fog":       [("Overcast", .62), ("Cloudy", .22), ("Fog", .16)],
    "LightSnow": [("Overcast", .48), ("HeavySnow", .22), ("LightSnow", .18), ("Cloudy", .12)],
    "HeavySnow": [("LightSnow", .58), ("Overcast", .30), ("HeavySnow", .12)],
}


def seasonal_weight(cond: str, season: str) -> float:
    if cond == "Clear":
        return {"Summer": 2.0, "Spring": 1.2, "Autumn": 0.7}.get(season, 0.5)
    if cond in ("LightRain", "HeavyRain"):
        return {"Autumn": 1.4, "Winter": 1.2, "Spring": 1.1}.get(season, 0.7)
    if cond == "Storm":
        return {"Autumn": 1.5, "Winter": 1.2}.get(season, 0.8)
    if cond == "Fog":
        return {"Winter": 2.0, "Autumn": 1.6}.get(season, 0.4)
    if cond in ("LightSnow", "HeavySnow"):
        return 1.0 if season == "Winter" else 0.0
    return 1.0


TRANSITION_SECONDS = 90.0
MIN_DWELL_SECONDS = 60.0


class WeatherSim:
    """Mirrors WeatherSimulation.cs. RNG differs from System.Random, so only
    statistical properties are compared, never exact sequences."""

    def __init__(self, seed, initial="Overcast", season="Autumn"):
        self.rng = random.Random(seed)
        self.frm = initial
        self.to = initial
        self.transition_elapsed = TRANSITION_SECONDS
        self.dwell_elapsed = 0.0
        self.season = season
        self.scripted = False
        self.dwell_target = self._roll_dwell(initial)

    def _roll_dwell(self, cond):
        mean = PROFILES[cond][6]
        dwell = mean * (0.6 + self.rng.random() * 0.8)
        return max(dwell, MIN_DWELL_SECONDS)

    def _pick_next(self, current):
        cands = TRANSITIONS[current]
        total = sum(w * seasonal_weight(c, self.season) for c, w in cands)
        if total <= 0:
            return current
        roll = self.rng.random() * total
        running = 0.0
        for c, w in cands:
            running += w * seasonal_weight(c, self.season)
            if roll <= running:
                return c
        return cands[-1][0]

    def _begin(self, nxt):
        self.frm, self.to = self.to, nxt
        self.transition_elapsed = 0.0
        self.dwell_elapsed = 0.0
        self.dwell_target = self._roll_dwell(nxt)

    def force(self, cond):
        self.scripted = True
        if cond != self.to:
            self._begin(cond)

    def advance(self, dt):
        if dt < 0:
            raise ValueError("Time does not run backwards.")
        if self.transition_elapsed < TRANSITION_SECONDS:
            self.transition_elapsed = min(self.transition_elapsed + dt, TRANSITION_SECONDS)
            if self.transition_elapsed >= TRANSITION_SECONDS:
                self.frm = self.to
            return
        if self.scripted:
            return
        self.dwell_elapsed += dt
        if self.dwell_elapsed < self.dwell_target:
            return
        self.dwell_elapsed = 0.0
        nxt = self._pick_next(self.to)
        if nxt == self.to:
            self.dwell_target = self._roll_dwell(self.to)
        else:
            self._begin(nxt)

    @property
    def t(self):
        return min(self.transition_elapsed / TRANSITION_SECONDS, 1.0)

    @property
    def precipitation(self):
        return PROFILES[self.frm][0] + (PROFILES[self.to][0] - PROFILES[self.frm][0]) * self.t

    @property
    def dominant(self):
        return self.to if self.t >= 0.5 else self.frm


def time_by_condition(season, seed, total_seconds, step=1.0):
    sim = WeatherSim(seed, "Overcast", season)
    totals = {k: 0.0 for k in PROFILES}
    t = 0.0
    while t < total_seconds:
        sim.advance(step)
        totals[sim.dominant] += step
        t += step
    return totals



# ---------------------------------------------------------------- missions

THE_FIRST_DAY = [
    # (beat_id, kind, target, next, state_changes)
    ("explore_centre",        "GoTo", "town_centre",    "find_river",           []),
    ("find_river",            "GoTo", "riverside",      "speak_local",          []),
    ("speak_local",           "Talk", "local_resident", "visit_high_street",    [("met.first_local", 1, True)]),
    ("visit_high_street",     "GoTo", "high_street",    "return_meeting_point", []),
    ("return_meeting_point",  "GoTo", "meeting_point",  None,
     [("mission.the_first_day.complete", 1, True), ("standing.quayside", 5, False)]),
]


class MissionRunner:
    """Mirrors MissionRunner.cs."""

    def __init__(self, beats):
        self.beats = {b[0]: b for b in beats}
        if len(self.beats) != len(beats):
            raise ValueError("duplicate beat id")
        for b in beats:
            if b[3] is not None and b[3] not in self.beats:
                raise ValueError(f"dangling transition from {b[0]} to {b[3]}")
        self.first = beats[0][0]
        self.current = None
        self.status = "NotStarted"
        self.world = {}
        self.completed_events = 0

    def begin(self):
        self.current = self.first
        self.status = "Running"

    def _apply(self, beat):
        for key, value, absolute in beat[4]:
            self.world[key] = value if absolute else self.world.get(key, 0) + value

    def complete(self):
        if self.status != "Running" or self.current is None:
            return
        beat = self.beats[self.current]
        self._apply(beat)
        if beat[3] is None:
            self.current = None
            self.status = "Completed"
            self.completed_events += 1
        else:
            self.current = beat[3]

    def try_complete(self, kind, target):
        if self.status != "Running" or self.current is None:
            return False
        beat = self.beats[self.current]
        if beat[1] != kind or beat[2].lower() != target.lower():
            return False
        self.complete()
        return True


def check_missions():
    print("\nMissions")
    r = MissionRunner(THE_FIRST_DAY)
    r.begin()
    check("begins at the first beat", r.current == "explore_centre")

    check("wrong target does not advance",
          r.try_complete("GoTo", "riverside") is False and r.current == "explore_centre")
    check("wrong kind does not advance", r.try_complete("Talk", "town_centre") is False)

    r.try_complete("GoTo", "town_centre")
    r.try_complete("GoTo", "town_centre")
    check("repeated trigger does not double-advance", r.current == "find_river")

    r.try_complete("GoTo", "riverside")
    check("state change not applied early", "met.first_local" not in r.world)
    r.try_complete("Talk", "local_resident")
    check("state change applied on completion", r.world.get("met.first_local") == 1)

    r.try_complete("GoTo", "high_street")
    r.try_complete("GoTo", "meeting_point")
    check("mission reaches completion", r.status == "Completed")
    check("completion flag written", r.world.get("mission.the_first_day.complete") == 1)
    check("standing awarded", r.world.get("standing.quayside") == 5)

    r.complete()
    check("completion fires exactly once", r.completed_events == 1)

    # graph integrity
    reached, node = set(), THE_FIRST_DAY[0][0]
    beats = {b[0]: b for b in THE_FIRST_DAY}
    while node and node not in reached:
        reached.add(node)
        node = beats[node][3]
    check("every beat is reachable", len(reached) == len(THE_FIRST_DAY),
          f"reached {len(reached)} of {len(THE_FIRST_DAY)}")

    try:
        MissionRunner([("a", "GoTo", "x", "nope", [])])
        check("dangling transition rejected", False)
    except ValueError:
        check("dangling transition rejected", True)

    try:
        MissionRunner([("a", "GoTo", "x", None, []), ("a", "GoTo", "y", None, [])])
        check("duplicate beat id rejected", False)
    except ValueError:
        check("duplicate beat id rejected", True)


# ---------------------------------------------------------------- assertions

FAILURES = []


def check(name, condition, detail=""):
    if condition:
        print(f"  PASS  {name}")
    else:
        print(f"  FAIL  {name}  {detail}")
        FAILURES.append(name)


def main():
    print("Surfaces")
    p = simulate(crown(), 1.0, 30.0)
    check("exposed surface wets in heavy rain", p.wetness > 0.5, f"wetness={p.wetness:.3f}")

    p = simulate(arch(), 1.0, 120.0)
    check("sheltered surface stays dry in rain", p.wetness < 0.15, f"wetness={p.wetness:.3f}")

    p = simulate(crown(), 1.0, 600.0)
    check("wetness saturates at 1", abs(p.wetness - 1.0) < 1e-4, f"wetness={p.wetness:.6f}")

    c = crown()
    wet_s, dry_s = wetting_delta(c, 1.0, 1.0), drying_delta(c, TEMPERATE, 1.0)
    check("wetting is >4x faster than drying", wet_s > dry_s * 4,
          f"wet={wet_s:.5f}/s dry={dry_s:.5f}/s ratio={wet_s/dry_s:.1f}x")

    p = simulate(crown(), 0.0, 3600.0)
    check("wetness floors at 0", p.wetness == 0.0, f"wetness={p.wetness}")

    # THE key acceptance criterion
    cr = simulate(soaked(crown()), 0.0, 300.0)
    gu = simulate(soaked(gutter()), 0.0, 300.0)
    ar = simulate(soaked(arch()), 0.0, 300.0)
    print(f"        after 5 min: crown={cr.wetness:.3f} gutter={gu.wetness:.3f} arch={ar.wetness:.3f}")
    check("crown dries before gutter", cr.wetness < gu.wetness)
    check("gutter dries before arch", gu.wetness < ar.wetness)
    check("spread is visible on screen (>0.2)", ar.wetness - cr.wetness > 0.2,
          f"spread={ar.wetness - cr.wetness:.3f}")

    b = seconds_to_dry(soaked(Patch(BRICK, 1, 1)), 0.0, TEMPERATE)
    g = seconds_to_dry(soaked(Patch(GLASS, 1, 1)), 0.0, TEMPERATE)
    check("brick holds water >3x longer than glass", b > g * 3, f"brick={b:.0f}s glass={g:.0f}s")

    w = seconds_to_dry(soaked(crown()), 0.0, Drying(1, 1, 0.97))
    s = seconds_to_dry(soaked(crown()), 0.0, Drying(26, 4, 0.35))
    check("cold saturated air dries >3x slower", w > s * 3, f"winter={w:.0f}s summer={s:.0f}s")

    fine = simulate(soaked(crown()), 0.0, 120.0, step=1/120)
    coarse = simulate(soaked(crown()), 0.0, 120.0, step=1/15)
    check("frame rate does not change drying", abs(fine.wetness - coarse.wetness) < 0.02,
          f"fine={fine.wetness:.4f} coarse={coarse.wetness:.4f}")

    print("\nWeather")
    sim = WeatherSim(7, "Clear", "Autumn")
    prev, worst = sim.precipitation, 0.0
    for _ in range(40000):
        sim.advance(0.5)
        worst = max(worst, abs(sim.precipitation - prev))
        prev = sim.precipitation
    check("precipitation never snaps", worst < 0.05, f"largest step={worst:.4f}")

    totals = time_by_condition("Autumn", 99, 400000.0)
    tot = sum(totals.values())
    grey = (totals["Overcast"] + totals["Cloudy"] + totals["LightRain"]) / tot
    storm = totals["Storm"] / tot
    print(f"        autumn mix: grey={grey:.1%} storm={storm:.1%} clear={totals['Clear']/tot:.1%} fog={totals['Fog']/tot:.1%}")
    check("grey weather dominates (>60%)", grey > 0.60, f"grey={grey:.1%}")
    check("storms stay rare (<10%)", storm < 0.10, f"storm={storm:.1%}")

    for season in ("Spring", "Summer", "Autumn"):
        t = time_by_condition(season, 5, 300000.0)
        check(f"no snow in {season}", t["LightSnow"] == 0 and t["HeavySnow"] == 0)

    su = time_by_condition("Summer", 11, 300000.0)
    wi = time_by_condition("Winter", 11, 300000.0)
    check("summer clearer than winter", su["Clear"] > wi["Clear"],
          f"summer={su['Clear']:.0f}s winter={wi['Clear']:.0f}s")
    su2 = time_by_condition("Summer", 21, 300000.0)
    wi2 = time_by_condition("Winter", 21, 300000.0)
    check("winter foggier than summer", wi2["Fog"] > su2["Fog"],
          f"winter={wi2['Fog']:.0f}s summer={su2['Fog']:.0f}s")

    sim = WeatherSim(3, "Clear", "Autumn")
    sim.force("Fog")
    for _ in range(20000):
        sim.advance(1.0)
    check("scripted weather holds", sim.dominant == "Fog", f"got {sim.dominant}")

    check_missions()

    print()
    if FAILURES:
        print(f"{len(FAILURES)} FAILED: {', '.join(FAILURES)}")
        return 1
    print("All model checks passed.")
    return 0


if __name__ == "__main__":
    sys.exit(main())
