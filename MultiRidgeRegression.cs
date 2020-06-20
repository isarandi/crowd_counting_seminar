using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathNet.Numerics.LinearAlgebra.Generic;
using MathNet.Numerics.LinearAlgebra.Double;

namespace CS
{
    class MultiRidgeRegression : IMultiRegression
    {
        private Matrix<double> weights;
        private double ridgeC;

        public MultiRidgeRegression(double C)
        {
            ridgeC = C;
        }

        private Matrix AppendOnes(Matrix<double> inputs)
        {
            return (Matrix)inputs.InsertColumn(0, DenseVector.Create(inputs.RowCount, (int i) => 1));
        }

        public void Train(Matrix<double> inputs, Matrix<double> outputs)
        {
            Matrix<double> appendedInputs = AppendOnes(inputs);
            Matrix<double> regularizationPart = 1 / (2 * ridgeC) * DiagonalMatrix.Create(appendedInputs.ColumnCount, appendedInputs.ColumnCount, (int i) => (i == 0 ? 0 : 1));
            Matrix<double> regularizedCovariance = appendedInputs.Transpose() * appendedInputs + regularizationPart;

            Console.WriteLine("Solving for weights.");
            weights = regularizedCovariance.QR().Solve(appendedInputs.Transpose() * outputs);

            //IList<string> lines = new List<string>();
            //for (int i = 0; i < weights.RowCount; i++)
            //{
            //    string line = "";
            //    for (int j = 0; j < weights.ColumnCount; j++)
            //    {
            //        line += String.Format("{0}; ", weights[i,j]); 
            //    }
            //    line = line.Substring(0, line.Length - 2);
            //    lines.Add(line);
            //}

            //Console.WriteLine("Weights done.");
            //System.IO.File.WriteAllLines(@"D:\weights.txt", lines);
        }

        public Matrix<double> Predict(Matrix<double> inputs)
        {
            return AppendOnes(inputs) * weights;
        }

        public override string ToString()
        {
            return String.Format("Linear ridge C={0}", ridgeC);
        }
    }
}
