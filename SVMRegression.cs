using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathN = MathNet.Numerics.LinearAlgebra.Generic;
using Accord.MachineLearning;
using MathNet.Numerics.LinearAlgebra.Double;
using Accord.MachineLearning.VectorMachines;
using Accord.Statistics.Kernels;
using Accord.MachineLearning.VectorMachines.Learning;
using System.Threading.Tasks;

namespace CS
{
    class SVMRegression : IMultiRegression
    {
        private List<KernelSupportVectorMachine> machines;
        private double C;
        private double epsilon;

        public SVMRegression(double C, double epsilon)
        {
            this.C = C;
            this.epsilon = epsilon;
        }

        public void Train(MathN::Matrix<double> inputs, MathN::Matrix<double> outputs)
        {
            // Example regression problem. Suppose we are trying 
            // to model the following equation: f(x, y) = 2x + y 

            double[][] inputArray = new double[inputs.RowCount][];

            for (int i = 0; i < inputs.RowCount; ++i)
            {
                inputArray[i] = new double[inputs.ColumnCount];

                for (int j = 0; j < inputs.ColumnCount; ++j)
                {
                    inputArray[i][j] = inputs[i, j];
                }
            }

            machines = new List<KernelSupportVectorMachine>(outputs.ColumnCount);

            Parallel.ForEach(outputs.ColumnEnumerator(), col =>
            {
                double[] outputArray = col.Item2.ToArray();
                int colID = col.Item1;

                KernelSupportVectorMachine machine = new KernelSupportVectorMachine(new Polynomial(2), inputs: inputs.ColumnCount);
                machines.Add(machine);

                var learn = new SequentialMinimalOptimizationRegression(machine, inputArray, outputArray);
                learn.Complexity = C;
                learn.Epsilon = epsilon;
                learn.Run(false);
            });

        }

        public MathN::Matrix<double> Predict(MathN::Matrix<double> inputs)
        {
            // Compute theanswer for one particular example 
            MathN::Matrix<double> result = new DenseMatrix(inputs.RowCount, machines.Count);

            Parallel.ForEach(inputs.RowEnumerator(), row =>
            {
                double[] inputArray = row.Item2.ToArray();

                int j = 0;
                foreach (KernelSupportVectorMachine machine in machines)
                {
                    result[row.Item1, j++] = machine.Compute(inputArray);
                }
                
            });
            
            return result;
        }

        public override string ToString()
        {
            return String.Format("SVM polynomial C={0}, epsilon={1}", C, epsilon);
        }
    }
}
