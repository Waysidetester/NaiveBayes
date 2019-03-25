using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NaiveBayes.TrainingData;

namespace NaiveBayes
{
    class Program
    {
        static void Main(string[] args)
        {
            // Check https://en.wikipedia.org/wiki/Naive_Bayes_classifier for explanation

            List<Data> info = new List<Data>();

            info.Add(new Data("male", 6, 180, 12));
            info.Add(new Data("male", 5.92, 190, 11));
            info.Add(new Data("male", 5.58, 170, 12));
            info.Add(new Data("male", 5.92, 165, 10));
            info.Add(new Data("female", 5, 100, 6));
            info.Add(new Data("female", 5.5, 150, 8));
            info.Add(new Data("female", 5.4, 130, 7));
            info.Add(new Data("female", 5.75, 150, 9));

            List<double> TrainData(List<Data> trainingData)
            {
                // separates the training data by what is being solved for
                IEnumerable<IGrouping<string, Data>> groupedTrainingData = WhatToSolveFor(trainingData);

                LoopOverData(groupedTrainingData);

                double firstAvg = trainingData.Select(t => t.Height).ToList().Average();
                double secondAvg = trainingData.Select(t => t.Weight).ToList().Average();
                double thirdAvg = trainingData.Select(t => t.Foot).ToList().Average();

                double firstVari = Variance(firstAvg, trainingData.Select(t => t.Height).ToList());
                double secondVari = Variance(firstAvg, trainingData.Select(t => t.Weight).ToList());
                double thirdVari = Variance(firstAvg, trainingData.Select(t => t.Foot).ToList());

                var PredictionOutputList = trainingData.Select(t => t.Gender)
                    .GroupBy(str => str, (key, list) => new { Type = key, Count = list.Count() }).ToList();

                double predictOutputA = PredictionOutputList[0].Count/(PredictionOutputList[0].Count + PredictionOutputList[1].Count);
                double predictOutputB = PredictionOutputList[1].Count/(PredictionOutputList[0].Count + PredictionOutputList[1].Count);

                return new List<double>
                {
                    firstAvg,
                    firstVari,
                    secondAvg,
                    secondVari,
                    thirdAvg,
                    thirdVari,
                    predictOutputA,
                    predictOutputB,
                };
            }

            void LoopOverData (IEnumerable<IGrouping<string, Data>> groupedData)
            {
                // outcome is key, dictionary of props is value
                var allDataDictionary = new Dictionary<string, Dictionary<string, Dictionary<string, double>>>();

                // gets properties on the Data class
                var dataProps = typeof(Data).GetProperties();

                // Loops over the different groups of training data
                foreach (var groupedList in groupedData)
                {
                    // Property is key, dictionary of average/variance is value
                    var outcomeDictionary = new Dictionary<string, Dictionary<string, double>>();

                    // loops over properties in Data class
                    foreach (var prop in dataProps)
                    {
                        // average/variance is key, number is value
                        var propDictionary = new Dictionary<string, double>();

                        // for each List of similar outcomes ... run command
                        foreach (var outcome in groupedList)
                        {
                            /* stores value of each property to e. Need to 
                             * rename and expand this part of the logic. 
                             */
                            var e = prop.GetValue(outcome);
                        }
                        outcomeDictionary.Add(prop.Name, propDictionary);
                    }
                    allDataDictionary.Add(groupedList.Key, outcomeDictionary);
                }
            }

            IEnumerable<IGrouping<string, Data>> WhatToSolveFor(List<Data> trainingList)
            {
                var groupedTrainingData = trainingList.GroupBy(x=> x.Gender);
                return groupedTrainingData;
            }

            List<double> predictorConstants = TrainData(info);

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
                if (data.Gender == "male")
                {
                    maleHeight.Add(data.Height);
                    maleWeight.Add(data.Weight);
                    maleFoot.Add(data.Foot);
                    maleCount++;
                }
                else if (data.Gender == "female")
                {
                    femaleHeight.Add(data.Height);
                    femaleWeight.Add(data.Weight);
                    femaleFoot.Add(data.Foot);
                    femaleCount++;
                }
            }

            double Variance(double average, List<double>listOfMeasurement)
            {
                return (listOfMeasurement.Select(x => Math.Pow(x - average, 2)).Sum() / (listOfMeasurement.Count - 1));
            }

            /* double Average(List<Data> trainingData, PropertyInfo propSelector)
            {
                return trainingData.Select(t => new Double(d).ToList().Average();
            } */

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
