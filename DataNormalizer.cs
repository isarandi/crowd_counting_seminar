using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathNet.Numerics.LinearAlgebra.Generic;
using System.Threading.Tasks;

namespace CS
{
    interface IDataNormalizer
    {
        void Normalize(Matrix<double> inputs);
        void TrainNormalization(Matrix<double> inputs);
    }

    class MeanAndVarianceDataNormalizer : IDataNormalizer
    {
        private double[] means;
        private double[] stddevs;

        public void Normalize(Matrix<double> inputs)
        {
            Parallel.ForEach(Enumerable.Range(0, inputs.RowCount), i =>
            {
                for (int j = 0; j < inputs.ColumnCount; j++)
                {
                    if (stddevs[j] > 1e-13)
                    {
                        inputs[i, j] = (inputs[i, j] - means[j]) / stddevs[j];
                    }
                }
            });
        }

        public void TrainNormalization(Matrix<double> inputs)
        {
            means = new double[inputs.ColumnCount];
            stddevs = new double[inputs.ColumnCount];
            double[] expectedSquares = new double[inputs.ColumnCount];
            ParallelOptions po = new ParallelOptions();
            po.MaxDegreeOfParallelism = 1;
            Parallel.ForEach(Enumerable.Range(0, inputs.ColumnCount), po, j =>
            {
                for (int i = 0; i < inputs.RowCount; i++)
                {
                    double val = inputs[i, j];
                    means[j] += val;
                    expectedSquares[j] += (val * val);
                }

                means[j] /= inputs.RowCount;
                expectedSquares[j] /= inputs.RowCount;

                double squareOfMean = means[j] * means[j];
                double variance = expectedSquares[j] - squareOfMean;

                stddevs[j] = Math.Sqrt(variance);
            });
        }

        public override string ToString()
        {
            return String.Format("mean and variance");
        }
    }

    class IdleDataNormalizer : IDataNormalizer
    {
        public void Normalize(Matrix<double> inputs)
        {
        }

        public void TrainNormalization(Matrix<double> inputs)
        {
        }

        public override string ToString()
        {
            return String.Format("none");
        }
    }
}
