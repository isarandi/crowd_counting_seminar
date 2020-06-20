using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathNet.Numerics.Distributions;
using MathNet.Numerics.LinearAlgebra.Generic;
using MathNet.Numerics.LinearAlgebra.Double;

namespace CS
{
    class HyperParamConfig
    {
        List<int> discreteHyperParameters;
        List<double> continuousHyperParameters;

        public HyperParamConfig(List<int> discreteHyperParameters, List<double> continuousHyperParameters)
        {
            this.discreteHyperParameters = discreteHyperParameters;
            this.continuousHyperParameters = continuousHyperParameters;
        }

        public Vector<double> ToVector()
        {
            Vector<double> result = new DenseVector(discreteHyperParameters.Count + continuousHyperParameters.Count);

            int i=0;
            foreach (int val in discreteHyperParameters)
            {
                result[i++] = val;
            }

            foreach (double val in continuousHyperParameters)
            {
                result[i++] = val;
            }

            return result;
        }
    }

    class HyperParamDistributions
    {
        List<IDiscreteDistribution> discreteHyperParameterDistributions;
        List<IContinuousDistribution> continuousHyperParameterDistributions;

        public HyperParamDistributions(
            List<IDiscreteDistribution> discreteHyperParameterDistributions,
            List<IContinuousDistribution> continuousHyperParameterDistributions)
        {
            this.discreteHyperParameterDistributions = discreteHyperParameterDistributions;
            this.continuousHyperParameterDistributions = continuousHyperParameterDistributions;
        }

        public HyperParamConfig Sample()
        {
            List<int> discreteHyperParameters = new List<int>();
            foreach (IDiscreteDistribution distr in discreteHyperParameterDistributions)
            {
                discreteHyperParameters.Add(distr.Sample());
            }
            
            List<double> continuousHyperParameters = new List<double>();
            foreach (IContinuousDistribution distr in continuousHyperParameterDistributions)
            {
                continuousHyperParameters.Add(distr.Sample());
            }

            return new HyperParamConfig(discreteHyperParameters, continuousHyperParameters);
        }

        //public Vector<double> GetNormalized(HyperParamConfig paramConfig)
        //{
        //    for (
        //}

    }

    class ExperimentCollection
    {
        HyperParamConfig bestConfig = null;
        double bestValue;

        public Matrix<double> Inputs { get; private set; }
        public List<double> Outputs { get; private set; }

        public void AddExperiment(HyperParamConfig paramConfig, double result)
        {
            Inputs.InsertRow(Inputs.RowCount, paramConfig.ToVector());
            Outputs.Add(result);

            if (result < bestValue)
            {
                bestValue = result;
                bestConfig = paramConfig;
            }
        }
    }

    class HyperParameterOptimizer
    {
        HyperParamDistributions distributions;
        Func<HyperParamConfig, double> evaluatorFunction;
        
        ExperimentCollection experimentCollection = new ExperimentCollection();

        GaussianProcessRegression gaussianProcess = new GaussianProcessRegression(0.5, 5, 500);
                
        public HyperParameterOptimizer(HyperParamDistributions distributions,
            Func<HyperParamConfig, double> evaluatorFunction)
        {
            this.distributions = distributions;
            this.evaluatorFunction = evaluatorFunction;
        }

        public void TryNext()
        {
            HyperParamConfig config = distributions.Sample();

            //normalizer.
            //IContinuousDistribution predictiveDistribution = gaussianProcess.GetPredictiveDistribution(
            //evaluatorFunction(distributions.Sample());
        }


    }
}
