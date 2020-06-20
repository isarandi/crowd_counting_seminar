using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathNet.Numerics.LinearAlgebra.Generic;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.Distributions;
using System.Threading.Tasks;

namespace CS
{
    class GaussianProcessRegression
    {
        private Vector<double> A;
        private double rbfGamma;
        private double signalVariance;
        private double observationNoiseVariance;

        private delegate double Kernel(Vector<double> x1, Vector<double> x2);
        private Kernel kernel;

        private Matrix<double> trainingInputs;
        private Matrix<double> invertedRegularizedTrainingKernel;

        public GaussianProcessRegression(double C, double gamma, double signalVariance)
        {
            observationNoiseVariance = 1.0/(2*C);
            rbfGamma = gamma;

            kernel = this.RbfKernel;
            this.signalVariance = signalVariance;
        }
        
        public double RbfKernel(Vector<double> x1, Vector<double> x2)
        {
            double sumOfSquareDiffs = 0;

            for (int j = 0; j < x1.Count; ++j)
            {
                double diff = x1[j] - x2[j];
                sumOfSquareDiffs += diff * diff;
            }

            return signalVariance * Math.Exp(-rbfGamma * sumOfSquareDiffs);
        }
        
        public void Train(Matrix<double> inputs, Vector<double> outputs)
        {
            NaNCheck(inputs, "normalized inputs");

            trainingInputs = inputs;
            Matrix<double> K_train_train = GetKernelMatrix(inputs, inputs);

            NaNCheck(K_train_train, "kernelmatrix");

            Matrix<double> regularizationPart = observationNoiseVariance * DenseMatrix.Identity(inputs.RowCount);
            invertedRegularizedTrainingKernel = (K_train_train + regularizationPart).Inverse();

            A = invertedRegularizedTrainingKernel * outputs;

            NaNCheck(A, "dualweights");
        }

        public IContinuousDistribution GetPredictiveDistribution(Vector<double> testInput)
        {
            Vector<double> K_test_train = new DenseVector(trainingInputs.RowCount);

            for (int j = 0; j < trainingInputs.RowCount; j++)
            {
                K_test_train[j] = kernel(testInput, trainingInputs.Row(j));
            }

            double mean = K_test_train.DotProduct(A);
            double variance = kernel(testInput, testInput) - (K_test_train.ToRowMatrix() * invertedRegularizedTrainingKernel * K_test_train.ToColumnMatrix())[0,0];
            return new Normal(mean, Math.Sqrt(variance));
        }
        
        public override string ToString()
        {
            return String.Format("Gaussian process (obsNoiseVarSq={0}, lengthScale={1})", observationNoiseVariance, 1.0/(2.0*rbfGamma));
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

        public static void NaNCheck(Vector<double> matrix, string name)
        {
            for (int j = 0; j < matrix.Count; j++)
            {
                if (Double.IsNaN(matrix[j]))
                {
                    Console.WriteLine(name + " {0}", j);
                }
            }
        }

        public static void NaNCheck(Matrix<double> matrix, string name)
        {
            for (int i = 0; i < matrix.RowCount; i++)
            {
                for (int j = 0; j < matrix.ColumnCount; j++)
                {
                    if (Double.IsNaN(matrix[i, j]))
                    {
                        Console.WriteLine(name + " {0}, {1}", i, j);
                    }
                }
            }
        }
    }
}
