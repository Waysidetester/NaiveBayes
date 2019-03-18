using System;
using System.Collections.Generic;
using System.Linq;

namespace NaiveBayes
{
    class Program
    {
        static void Main(string[] args)
        {
            // Check https://en.wikipedia.org/wiki/Naive_Bayes_classifier for explanation

            List<Tuple<string, double, double, double>> info = new List<Tuple<string, double, double, double>>();

            info.Add(new Tuple<string, double, double, double>("male", 6, 180, 12));
            info.Add(new Tuple<string, double, double, double>("male", 5.92, 190, 11));
            info.Add(new Tuple<string, double, double, double>("male", 5.58, 170, 12));
            info.Add(new Tuple<string, double, double, double>("male", 5.92, 165, 10));
            info.Add(new Tuple<string, double, double, double>("female", 5, 100, 6));
            info.Add(new Tuple<string, double, double, double>("female", 5.5, 150, 8));
            info.Add(new Tuple<string, double, double, double>("female", 5.4, 130, 7));
            info.Add(new Tuple<string, double, double, double>("female", 5.75, 150, 9));

            double maleCount = 0;
            List<double> maleHeight = new List<double>();
            List<double> maleWeight = new List<double>();
            List<double> maleFoot = new List<double>();
            double femaleCount = 0;
            List<double> femaleHeight = new List<double>();
            List<double> femaleWeight = new List<double>();
            List<double> femaleFoot = new List<double>();


            for (int i = 0; i < info.Count; i++)
            {
                var data = info[i];
                if (data.Item1 == "male")
                {
                    maleHeight.Add(data.Item2);
                    maleWeight.Add(data.Item3);
                    maleFoot.Add(data.Item4);
                    maleCount++;
                }
                else if (data.Item1 == "female")
                {
                    femaleHeight.Add(data.Item2);
                    femaleWeight.Add(data.Item3);
                    femaleFoot.Add(data.Item4);
                    femaleCount++;
                }
            }

            double Variance(double average, List<double>listOfMeasurement)
            {
                return (listOfMeasurement.Select(x => Math.Pow(x - average, 2)).Sum() / (listOfMeasurement.Count - 1));
            }

            var maleHeightAve = maleHeight.Average();
            var maleHeightVari = Variance(maleHeightAve, maleHeight);
            var maleWeightAve = maleWeight.Average();
            var maleWeightVari = Variance(maleWeightAve, maleWeight);
            var maleFootAve = maleFoot.Average();
            var maleFootVari = Variance(maleFootAve, maleFoot);

            Console.WriteLine($"{maleHeightAve} + {maleWeightAve} + {maleFootAve}");
            Console.WriteLine($"{maleHeightVari} + {maleWeightVari} + {maleFootVari}");

            double SingleProbability(double newMeasure, double testMeasureAverage, double testMeasureVariance)
            {
                return ((1/(Math.Sqrt(2*Math.PI*testMeasureVariance))) * Math.Pow(Math.E, ((-1 * Math.Pow(newMeasure - testMeasureAverage, 2)) / (2 * testMeasureVariance))));
            }

            var maleProbHeight = SingleProbability(6, maleHeightAve, maleHeightVari);
            var maleProbWeight = SingleProbability(130, maleWeightAve, maleWeightVari);
            var maleProbFoot = SingleProbability(8, maleFootAve, maleFootVari);
            var maleProbGender = (maleCount / (maleCount + femaleCount));

            Console.WriteLine(maleProbFoot * maleProbGender * maleProbHeight * maleProbWeight);

            Console.ReadKey();
        }
    }
}
