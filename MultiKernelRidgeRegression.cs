using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathNet.Numerics.LinearAlgebra.Generic;
using MathNet.Numerics.LinearAlgebra.Double;
using System.Threading.Tasks;

namespace CS
{
    class MultiKernelRidgeRegression : IMultiRegression
    {
        private Matrix<double> A;
        private double ridgeC;
        private double rbfGamma;
        private delegate double Kernel(Vector<double> x1, Vector<double> x2);
        private Kernel kernel;
        private Matrix<double> trainingInputs;
       
        public MultiKernelRidgeRegression(double C, double gamma)
        {
            ridgeC = C;
            rbfGamma = gamma;

            kernel = this.RbfKernel;
        }
        
        public double RbfKernel(Vector<double> x1, Vector<double> x2)
        {
            double sumOfSquareDiffs = 0;

            for (int j = 0; j < x1.Count; ++j)
            {
                double diff = x1[j] - x2[j];
                sumOfSquareDiffs += diff * diff;
            }

            return Math.Exp(-rbfGamma * sumOfSquareDiffs);
        }
        
        public static void NaNCheck(Matrix<double> matrix, string name)
        {

            for (int i = 0; i < matrix.RowCount; i++)
            {
                for (int j = 0; j < matrix.ColumnCount; j++)
                {
                    if (Double.IsNaN(matrix[i, j]))
                    {
                        Console.WriteLine(name+" {0}, {1}", i, j);
                    }
                }
            }
        }

        public void Train(Matrix<double> inputs, Matrix<double> outputs)
        {
            //NaNCheck(inputs, "normalized inputs");

            trainingInputs = inputs;
            Matrix<double> K = GetKernelMatrix(inputs, inputs);

            //NaNCheck(K, "kernelmatrix");

            Matrix<double> regularizationPart = 1 / (2 * ridgeC) * DenseMatrix.Identity(inputs.RowCount);

            A = (K + regularizationPart).QR().Solve(outputs);

            //NaNCheck(A, "dualweights");

            //IList<string> lines = new List<string>();
            //for (int i = 0; i < A.RowCount; i++)
            //{
            //    string line = "";
            //    for (int j = 0; j < A.ColumnCount; j++)
            //    {
            //        line += String.Format("{0}; ", A[i, j]);
            //    }
            //    line = line.Substring(0, line.Length - 2);
            //    lines.Add(line);
            //}

            //Console.WriteLine("Weights done.");
            //System.IO.File.WriteAllLines(@"D:\weights_kernel2.txt", lines);
        }

        private Matrix<double> GetKernelMatrix(Matrix<double> X1, Matrix<double> X2)
        {
            Matrix<double> K = new DenseMatrix(X1.RowCount, X2.RowCount);

            Parallel.ForEach(Enumerable.Range(0, X1.RowCount), i =>
            {
                for (int j = 0; j < X2.RowCount; j++)
                {
                    K[i, j] = kernel(X1.Row(i), X2.Row(j));
                }
            });

            return K;
        }

        public Matrix<double> Predict(Matrix<double> testInputs)
        {
            return GetKernelMatrix(testInputs, trainingInputs) * A;
        }
        
        public override string ToString()
        {
            return String.Format("Kernel ridge C={0}, rbfGamma={1}", ridgeC, rbfGamma);
        }
    }
}
