using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.Structure;
using Math = MathNet.Numerics.LinearAlgebra.Generic;
using MathNet.Numerics.LinearAlgebra.Double;
using CS.FeatureExtraction;
using Emgu.CV.VideoSurveillance;
using Emgu.CV.CvEnum;
using CS.Evaluation;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;

namespace CS
{
    class CrowdCountingRegression
    {
        private static object cacheLock = new object();
        private FeatureExtractor featureExtractor;
        private IDataNormalizer dataNormalizer;
        private IMultiRegression innerRegression;
        public string CachePath {get;set;}
        
        public CrowdCountingRegression(FeatureExtractor featureExtractor, IDataNormalizer dataNormalizer, IMultiRegression regression)
        {
            this.featureExtractor = featureExtractor;
            this.dataNormalizer = dataNormalizer;
            this.innerRegression = regression;
        }
               
        public void Train(IList<string> frameImageFilePaths, IList<IList<PointF>> peoplePositions)
        {
            Math::Matrix<double> X = LoadOrCalculateInputMatrix(frameImageFilePaths);

            dataNormalizer.TrainNormalization(X);
            dataNormalizer.Normalize(X);

            Math::Matrix<double> Y = PeoplePositions.GridQuantize(peoplePositions, featureExtractor.N, featureExtractor.M, 640, 480);
            innerRegression.Train(X, Y);
        }

        public Math::Matrix<double> Predict(IList<string> frameImageFilePaths)
        {
            Math::Matrix<double> X = LoadOrCalculateInputMatrix(frameImageFilePaths);
            dataNormalizer.Normalize(X);

            return innerRegression.Predict(X);
        }

        private Math::Matrix<double> CalculateInputMatrix(IList<string> frameImageFilePaths)
        {
            Math::Matrix<double> X = new DenseMatrix(frameImageFilePaths.Count, featureExtractor.Length);
            StructuringElementEx kernel = new StructuringElementEx(3, 3, 1, 1, CV_ELEMENT_SHAPE.CV_SHAPE_ELLIPSE);

            ParallelOptions options = new ParallelOptions();
            options.MaxDegreeOfParallelism = 1;
            Parallel.ForEach(Enumerable.Range(0, frameImageFilePaths.Count), options, i =>
            {
                Math::Vector<double> featureVector = featureExtractor.ExtractFeatures(new Image<Gray, Byte>(frameImageFilePaths[i]));
                X.SetSubMatrix(i, 1, 0, featureExtractor.Length, featureVector.ToRowMatrix());
            });

            Console.Write(X.Row(1));
            return X;
        }

        private Math::Matrix<double> LoadOrCalculateInputMatrix(IList<string> frameImageFilePaths)
        {
            if (CachePath == null)
                return CalculateInputMatrix(frameImageFilePaths);

            string filename = String.Format("count={0}, {1}.dat", frameImageFilePaths.Count, featureExtractor.ToString());
            string path = Path.Combine(CachePath, filename);

            if (File.Exists(path))
            {
                lock (cacheLock)
                {
                    using (var stream = File.Open(path, FileMode.Open))
                    {
                        BinaryFormatter bf = new BinaryFormatter();
                        return DenseMatrix.OfArray((double[,])bf.Deserialize(stream));
                    }
                }
            }
            else
            {
                lock (cacheLock)
                {
                    Math::Matrix<double> inputMatrix = CalculateInputMatrix(frameImageFilePaths);
                    using (var stream = File.Open(path, FileMode.Create))
                    {
                        BinaryFormatter bf = new BinaryFormatter();
                        bf.Serialize(stream, inputMatrix.ToArray());
                    }
                    return inputMatrix;
                }
            }
        }


        public override string ToString()
        {
            return String.Format("Extractor: {0}; Normalization: {1}; Regression: {2}", featureExtractor.ToString(), dataNormalizer.ToString(), innerRegression.ToString());
        }
    }
}
