using System;
using System.Collections.Generic;
using System.Text;

namespace NaiveBayes.TrainingData
{
    class Data
    {
        public string Gender { get; }
        public double Height { get; }
        public double Weight { get; }
        public double Foot { get; }

        public Data(string gender, double height, double weight, double foot)
        {
            Gender = gender;
            Height = height;
            Weight = weight;
            Foot = foot;
        }
    }
}
