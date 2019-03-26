using System;
using System.Collections.Generic;
using System.Text;

namespace NaiveBayes
{
    class WhatToSolveFor
    {
        public double Height { get; }
        public double Weight { get; }
        public double Foot { get; }

        public WhatToSolveFor(double height, double weight, double foot)
        {
            Height = height;
            Weight = weight;
            Foot = foot;
        }

    }
}
