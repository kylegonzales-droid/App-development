namespace Manor.Core.Surfaces
{
    /// <summary>
    /// The authored material set. Numbers are art-direction values, tuned so that
    /// wetting takes tens of seconds and drying takes several minutes — the asymmetry
    /// that makes rain read as weather rather than as a toggle.
    /// </summary>
    public static class SurfaceLibrary
    {
        //                                                      name          poros  absorb  dry   snow
        public static readonly SurfaceMaterialProfile Asphalt      = new("Asphalt",      0.75f, 0.035f, 1.00f, 0.55f);
        public static readonly SurfaceMaterialProfile WornAsphalt  = new("WornAsphalt",  0.80f, 0.042f, 0.85f, 0.60f);
        public static readonly SurfaceMaterialProfile Concrete     = new("Concrete",     0.70f, 0.030f, 1.05f, 0.65f);
        public static readonly SurfaceMaterialProfile PavingSlab   = new("PavingSlab",   0.65f, 0.030f, 1.10f, 0.70f);
        public static readonly SurfaceMaterialProfile Brick        = new("Brick",        0.85f, 0.022f, 0.55f, 0.35f);
        public static readonly SurfaceMaterialProfile Render       = new("Render",       0.70f, 0.024f, 0.70f, 0.40f);
        public static readonly SurfaceMaterialProfile Glass        = new("Glass",        0.02f, 0.060f, 2.40f, 0.10f);
        public static readonly SurfaceMaterialProfile BareMetal    = new("BareMetal",    0.10f, 0.050f, 1.90f, 0.45f);
        public static readonly SurfaceMaterialProfile PaintedMetal = new("PaintedMetal", 0.15f, 0.048f, 1.80f, 0.50f);
        public static readonly SurfaceMaterialProfile Wood         = new("Wood",         0.60f, 0.026f, 0.75f, 0.55f);
        public static readonly SurfaceMaterialProfile Plastic      = new("Plastic",      0.10f, 0.052f, 2.00f, 0.50f);
        public static readonly SurfaceMaterialProfile Grass        = new("Grass",        0.55f, 0.034f, 0.65f, 0.85f);
        public static readonly SurfaceMaterialProfile Soil         = new("Soil",         0.90f, 0.038f, 0.40f, 0.80f);

        public static readonly SurfaceMaterialProfile[] All =
        {
            Asphalt, WornAsphalt, Concrete, PavingSlab, Brick, Render,
            Glass, BareMetal, PaintedMetal, Wood, Plastic, Grass, Soil
        };
    }
}
