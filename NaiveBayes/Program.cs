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

            void TrainData(List<Data> trainingData)
            {
                // separates the training data by what is being solved for
                IEnumerable<IGrouping<string, Data>> groupedTrainingData = WhatToSolveFor(trainingData);

                var globalDictionary = LoopOverData(groupedTrainingData, trainingData.Count);

                Finalize(globalDictionary, new WhatToSolveFor(6, 130, 8));
            }

            Dictionary<string,Dictionary<string,Dictionary<string, double>>> LoopOverData(IEnumerable<IGrouping<string, Data>> groupedData, double totalTrainingData)
            {
                // outcome is key, dictionary of props is value
                var allDataDictionary = new Dictionary<string, Dictionary<string, Dictionary<string, double>>>();

                // gets properties on the Data class
                var dataProps = typeof(Data).GetProperties();

                // =============== Loops over the different groups of training data ===============
                foreach (var groupedList in groupedData)
                {
                    // Property is key, dictionary of average/variance is value
                    var outcomeDictionary = new Dictionary<string, Dictionary<string, double>>();

                    // =============== loops over properties in Data class ===============
                    foreach (var prop in dataProps)
                    {
                        // average/variance is key, number is value
                        var propDictionary = new Dictionary<string, double>();


                        // =============== Begin string analysis =================
                        if(prop.PropertyType == typeof(string))
                        {
                            var stringProbability = (groupedList.Count() / totalTrainingData);

                            propDictionary.Add("probability", stringProbability);
                        }
                        // =============== End string analysis ===================



                        // =============== Begin number analysis =================
                        else if (prop.PropertyType == typeof(double))
                        {
                            var listOfProperty = new List<double>();

                            // =============== Get number from identical prop ===============
                            foreach (var outcome in groupedList)
                            {
                                /* stores value of each property to e. Need to 
                                 * rename and expand this part of the logic. 
                                 */
                                var e = Convert.ToDouble(prop.GetValue(outcome));
                                listOfProperty.Add(e);
                            }
                            var doubleAverage = listOfProperty.Average();
                            var doubleVariance = Variance(doubleAverage, listOfProperty);

                            propDictionary.Add("average", doubleAverage);
                            propDictionary.Add("variance", doubleVariance);
                        }
                        // =============== End number analysis ===================

                        outcomeDictionary.Add(prop.Name, propDictionary);
                    } // =============== End Loop on properties ===============
                    allDataDictionary.Add(groupedList.Key, outcomeDictionary);
                } // =============== End Loop on groupedData ===============
                return allDataDictionary;
            } // =============== End LoopOverData Method ===============

            IEnumerable<IGrouping<string, Data>> WhatToSolveFor(List<Data> trainingList)
            {
                var groupedTrainingData = trainingList.GroupBy(x=> x.Gender);
                return groupedTrainingData;
            }

            double Variance(double average, List<double>listOfMeasurement)
            {
                return (listOfMeasurement.Select(x => Math.Pow(x - average, 2)).Sum() / (listOfMeasurement.Count - 1));
            }

             double SingleProbability(double newMeasure, double testMeasureAverage, double testMeasureVariance)
            {
                return ((1/(Math.Sqrt(2*Math.PI*testMeasureVariance))) * Math.Pow(Math.E, ((-1 * Math.Pow(newMeasure - testMeasureAverage, 2)) / (2 * testMeasureVariance))));
            }

            void Finalize(Dictionary<string, Dictionary<string, Dictionary<string, double>>> trainingDictionary,  WhatToSolveFor predictionData)
            {
                // List to give probability in %
                var finalOutcomeProbabilities = new List<Tuple<string, double>>();

                // ================== Each outcome to check ==================
                foreach (var (valToSolveFor, propList) in trainingDictionary)
                {
                    // 1 is neutral number to multiply by for probability function
                    double finalProbforVal = 1;

                    // ================== loop over properteis and output probability per property ==================
                    foreach (var (propName, propOutput) in propList)
                    {
                        // newMeasure will be set to the current property as long as it was given in the prediction data
                        double newMeasure = 0;
                        
                        // If the property doesn't exist in prediction data, it is what is being solved for. Here, we skip over it if it doesn't exist
                        if (typeof(WhatToSolveFor).GetProperty(propName) != null)
                        {
                            // extracts the value from the given property based on the variable property name
                            newMeasure = Convert.ToDouble(predictionData.GetType().GetProperty(propName).GetValue(predictionData));
                        }

                        // creates variables to be set later
                        double average = 0;
                        double variance = 0;

                        // ================== Loops over property values for extraction ==================
                        foreach (var (type, number) in propOutput)
                        {
                            // checks if it is categorical. Passes probability to finalPobforVal equation
                            if(type == "probability")
                            {
                                finalProbforVal = number * finalProbforVal;
                            }

                            // These Else-Ifs set the average and variance to be passed to a probability funciton that takes in the prediction data
                            else if(type == "average")
                            {
                                average = number;
                            }
                            else if (type == "variance")
                            {
                                variance = number;
                            }
                        }

                        // if propOutput.Count is > 1, it has an average and variance. If not, It has already taken care of this step
                        if(propOutput.Count > 1)
                        {
                            finalProbforVal = SingleProbability(newMeasure, average, variance) * finalProbforVal;
                        }
                    }
                    // Adds the probability in a number and what the likely outcome is to a list
                    finalOutcomeProbabilities.Add(new Tuple<string,double>(valToSolveFor, finalProbforVal));
                }

                // Loops over possible outcomes and prints to the console the % probability that outcome will occur.
                foreach (var outcome in finalOutcomeProbabilities)
                {
                    Console.WriteLine($"Percent Likely it is {outcome.Item1}: {(outcome.Item2 / (finalOutcomeProbabilities.Sum(x => x.Item2)))*100}%");
                }
            }

            // Executes the entire set of instructions listed above
            TrainData(info);

            Console.ReadKey();
        }
    }
}
