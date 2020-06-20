//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using MathNet.Numerics.LinearAlgebra.Generic;
//using NeuronDotNet.Core.Backpropagation;
//using NeuronDotNet.Core;
//using MathNet.Numerics.LinearAlgebra.Double;
//using System.IO;
//using System.Runtime.Serialization.Formatters.Binary;

//namespace CS
//{
//    class NeuralMultiRegression: IMultiRegression
//    {
//        BackpropagationNetwork network;


//        public void Train(Matrix<double> inputs, Matrix<double> outputs)
//        {
            
//            ActivationLayer inputLayer = new LinearLayer(inputs.ColumnCount);
//            ActivationLayer hiddenLayer = new SigmoidLayer(64*10);
//            ActivationLayer outputLayer = new LinearLayer(outputs.ColumnCount);
//            new BackpropagationConnector(inputLayer, hiddenLayer);
//            new BackpropagationConnector(hiddenLayer, outputLayer);
//            network = new BackpropagationNetwork(inputLayer, outputLayer);
//            network.SetLearningRate(0.1);

//            //Generate a training set for the ANN
//            TrainingSet trainingSet = new TrainingSet(inputs.ColumnCount, outputs.ColumnCount);

//            for (int i = 0; i < inputs.RowCount; i++)
//            {
//                trainingSet.Add(new TrainingSample(inputs.Row(i).ToArray(), outputs.Row(i).ToArray()));
//            }

//            //Start network learning
//            Console.WriteLine("Starting training.");
//            network.BeginEpochEvent += new TrainingEpochEventHandler(network_BeginEpochEvent);

//            network.Learn(trainingSet, 100);

            
//        }

//        void network_BeginEpochEvent(object sender, TrainingEpochEventArgs e)
//        {
//            Console.WriteLine("Begin epoch {0}", e.TrainingIteration);
//        }

//        public Matrix<double> Predict(Matrix<double> inputs)
//        {
//            Matrix<double> result = new DenseMatrix(inputs.RowCount, network.OutputLayer.NeuronCount);

//            for (int i = 0; i < inputs.RowCount; i++)
//            {
//                result.SetRow(i, network.Run(inputs.Row(i).ToArray()));
//            }

//            return result;
//        }
//    }
//}
