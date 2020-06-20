using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathNet.Numerics.LinearAlgebra.Generic;
using MathNet.Numerics.LinearAlgebra.Double;

namespace CS
{
    class LinearMultiRegression : IMultiRegression
    {
        Matrix<double> weights;

        private Matrix AppendOnes(Matrix<double> inputs)
        {
            return (Matrix)inputs.InsertColumn(0, DenseVector.Create(inputs.RowCount, (int i) => 1));
        }

        public void Train(Matrix<double> inputs, Matrix<double> outputs)
        {
            weights = AppendOnes(inputs).QR().Solve(outputs);
        }

        public Matrix<double> Predict(Matrix<double> inputs)
        {
            return (AppendOnes(inputs) * weights);
        }
    }
}
