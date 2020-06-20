using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathNet.Numerics.LinearAlgebra.Generic;

namespace CS
{
    interface IMultiRegression
    {
        void Train(Matrix<double> inputs, Matrix<double> outputs);
        Matrix<double> Predict(Matrix<double> inputs);
    }
}
