using System;

namespace Kott.Utilities
{
    public static class MathUtility
    {
        // Koeffizienten für die zentrale Region (A)
        private const double A0 = 2.506628277459239e+00;
        private const double A1 = -3.06647900018651e+01;
        private const double A2 = 1.383577518672690e+02;
        private const double A3 = -2.759285104469687e+02;
        private const double A4 = 2.209460984245205e+02;
        private const double A5 = -3.969683028665376e+01;

        // Koeffizienten für die zentrale Region (B)
        private const double B1 = -1.328068155288572e+01;
        private const double B2 = 6.680131188771972e+01;
        private const double B3 = -1.556989798598866e+02;
        private const double B4 = 1.615858368580409e+02;
        private const double B5 = -5.447609879822406e+01;

        // Koeffizienten für die Randregionen (C)
        private const double C0 = 2.938163982698783e+00;
        private const double C1 = 4.374641414649687e+00;
        private const double C2 = -2.549732539343734e+00;
        private const double C3 = -2.400758277161838e+00;
        private const double C4 = -3.223964580411365e-01;
        private const double C5 = -7.784894002430293e-03;

        // Koeffizienten für die Randregionen (D)
        private const double D1 = 3.754408661907416e+00;
        private const double D2 = 2.445134137142996e+00;
        private const double D3 = 3.224671290700398e-01;
        private const double D4 = 7.784695709041462e-03;

        /// <summary>
        ///     Berechnet die inverse standardnormale CDF (Beasley-Springer-Moro Approximation).
        /// </summary>
        public static double InverseStandardNormalCDF(double p)
        {
            if (p <= 0.0 || p >= 1.0)
            {
                throw new ArgumentOutOfRangeException(nameof(p), "Muss zwischen 0 und 1 liegen.");
            }

            double x;
            double q = p - 0.5;

            // Zentrale Region
            if (Math.Abs(q) < 0.42)
            {
                double r = q * q;

                // Polynom-Berechnung via Horner-Schema (Zähler)
                double num = ((((A5 * r + A4) * r + A3) * r + A2) * r + A1) * r + A0;

                // Polynom-Berechnung via Horner-Schema (Nenner)
                double den = ((((B5 * r + B4) * r + B3) * r + B2) * r + B1) * r + 1.0;

                x = q * num / den;
            }
            // Randregionen (Extremwerte nah an 0 oder 1)
            else
            {
                double r = p;
                if (q > 0.0)
                {
                    r = 1.0 - p;
                }

                if (r <= 0.0)
                {
                    return q > 0.0 ? double.PositiveInfinity : double.NegativeInfinity;
                }

                double t = Math.Sqrt(-2.0 * Math.Log(r));

                // Polynom-Berechnung via Horner-Schema (Zähler)
                double num = ((((C5 * t + C4) * t + C3) * t + C2) * t + C1) * t + C0;

                // Polynom-Berechnung via Horner-Schema (Nenner)
                double den = (((D4 * t + D3) * t + D2) * t + D1) * t + 1.0;

                x = num / den;

                if (q < 0.0)
                {
                    x = -x;
                }
            }

            return x;
        }

        public static double TransformUniformToNormal(double p, double mean, double stdDev)
        {
            return mean + stdDev * InverseStandardNormalCDF(p);
        }
        
        public static double TransformUniformToNormal01(double p)
        {
            const double mean = 0.5;
            const double stdDev = 0.16666666666666666; // 0.5 / 3

            double normalValue = TransformUniformToNormal(p, 0.5, 0.0);
            return Math.Clamp(normalValue, 0.0, 1.0);
        }
        
        public static double UniformToLogit(double uniformValue, double power = 5.0)
        {
            double k = 0.5 * Math.Exp(power);
            double s = (k - 0.5) / (k + 0.5);
            double g = s * (uniformValue - 0.5) + 0.5;
            return Math.Log(g / (1.0 - g)) / power;
        }
    }
}