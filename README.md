# NOTE:
Predictions are based on training data. It is not a guarantee that the outcome will occur, but a probability that the outcome is likely to occur based on the sample data provided

# NaiveBayes Purpose

This is a simple but correct implementation of Naive Bayes Classification. Basically, the project is supposed to take training data and output the probability it is one thing or another.
this code will be updated from time to time to refactor the logic and test for different probability.

## Data Structure
The `Program.cs` file is set up to process data the way you shape it. It is currently only set up to solve for a string. In the `Data` class, make sure you have one property that takes a string. This property is the outcome you are trying to predict. Each additional property relating to the data must be a double type. You can enter as many double type properties as you like. It is best if they relate to the final outcome

### Example
```string Gender {get;}
double Height{get;}``` etc.

## Technical Difficulties
You may notice that double is used for these math functions instead of decimal. This results in a less accurate outcome due to double using 64 bits and decimal using 128. Unfortunately, this could not be circumvented due to the lack of support for math functions such as square root and raising a value to the power of another value. This is likely a reason most machine learning calculations are handled in laguages such as Python or R. The difference in most cases are negligible.
