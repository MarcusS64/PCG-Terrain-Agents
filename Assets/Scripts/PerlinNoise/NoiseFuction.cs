using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NoiseFunction
{
    /* Function to linearly interpolate between a0 and a1
    * Weight w should be in the range [0.0, 1.0]*/
    static float interpolate(float a0, float a1, float w)
    {
        return (a1 - a0) * w + a0;
    }

    struct vector2
    {
        public float x, y;
    }

    /* Create pseudorandom direction vector*/
    static vector2 randomGradient(int ix, int iy) //Does not work for positive numbers
    {
        float a = ix, b = iy + 1;
        a *= 32841; b += Mathf.Pow(b, a);
        b *= 19115; a += Mathf.Pow(a, b); 
        a *= 20484;
        float random = (float)(a * (3.14159265 / b));
        vector2 v;
        v.x = Mathf.Cos(random); v.y = Mathf.Sin(random);
        return v;
    }

    // Computes the dot product of the distance and gradient vectors.
    static float dotGridGradient(int ix, int iy, float x, float y)
    {
        // Get gradient from integer coordinates
        vector2 gradient = randomGradient(ix, iy);

        // Compute the distance vector
        float dx = x - (float)ix;
        float dy = y - (float)iy;

        // Compute the dot-product
        return (dx * gradient.x + dy * gradient.y);
    }

    // Compute Perlin noise at coordinates x, y
    public static float perlin(float x, float y)
    {
        // Determine grid cell coordinates
        int x0 = (int)Mathf.Floor(x);
        int x1 = x0 + 1;
        int y0 = (int)Mathf.Floor(y);
        int y1 = y0 + 1;

        // Determine interpolation weights
        // Could also use higher order polynomial/s-curve here
        float sx = x - (float)x0;
        float sy = y - (float)y0;

        // Interpolate between grid point gradients
        float n0, n1, ix0, ix1, value;

        n0 = dotGridGradient(x0, y0, x, y);
        n1 = dotGridGradient(x1, y0, x, y);
        ix0 = interpolate(n0, n1, sx);

        n0 = dotGridGradient(x0, y1, x, y);
        n1 = dotGridGradient(x1, y1, x, y);
        ix1 = interpolate(n0, n1, sx);

        value = interpolate(ix0, ix1, sy);
        return value; // Will return in range -1 to 1
    }
}