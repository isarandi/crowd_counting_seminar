using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CS
{
    interface ErrorCalculator
    {
        double CalculateError(double actual, double desired);
    }

    class AbsoluteErrorCalculator: ErrorCalculator
    {
        public double CalculateError(double actual, double desired)
        {
            return Math.Abs(actual - desired);
        }
    }

    class SquaredErrorCalculator: ErrorCalculator
    {
        public double CalculateError(double actual, double desired)
        {
            double difference = actual - desired;
            return difference * difference;
        }
    }

    class RelativeDeviationErrorCalculator: ErrorCalculator
    {
        public double CalculateError(double actual, double desired)
        {
            double difference = actual - desired;
            return Math.Abs(actual - desired) / desired;
        }
    }


}
