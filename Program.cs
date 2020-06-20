using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Drawing;

using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra.Generic;
using MathN = MathNet.Numerics.LinearAlgebra.Generic;
using Emgu.CV;
using Emgu.CV.Structure;
using System.IO;
using Emgu.CV.VideoSurveillance;
using Emgu.CV.UI;
using Emgu.CV.CvEnum;
using CS.FeatureExtraction;
using CS.Evaluation;
using System.Threading.Tasks;
using System.Diagnostics;

namespace CS
{
    class Program
    {

        public static int Main(string[] args)
        {
            DataSets sets = new DataSets();
            DataSet dataset = sets.LoadMall();

            string datasetPath = dataset.Path;
            Image<Bgr, byte> background = new Image<Bgr, byte>(Path.Combine(datasetPath, "derived images", "background.png"));

            int width = 320;// dataset.DefaultWidth;
            int height = 240;// dataset.DefaultHeight;
            
            //IPerspectiveCorrector linearPerspectiveCorrector = new LinearPerspectiveCorrector(75 / 480.0, 53.0, 408 / 480.0, 143.0);

            int N = 8;//dataset.DefaultGridN;
            int M = 8;// dataset.DefaultGridM;

            FeatureExtractor extractor = new FeatureExtractor(new IdlePerspectiveCorrector(), N, M, width, height, background);
            IDataNormalizer dataNormalizer = new MeanAndVarianceDataNormalizer();

            IMultiRegression regression = new MultiRidgeRegression(1e-3);//100, 1e-5);
            CrowdCountingRegression countingRegression = new CrowdCountingRegression(extractor, dataNormalizer, regression);
            //countingRegression.CachePath = Path.Combine(dataset.Path, "cache");

            //string experimentFile = Path.Combine(datasetPath,"analysis","experiments_nullcorrection.txt");

            Stopwatch sw = new Stopwatch();
            sw.Start();
            countingRegression.Train(dataset.TrainPlusValidationInput, dataset.TrainPlusValidationOutput);
            Console.WriteLine("Training time (800 frames): {0} ms", sw.ElapsedMilliseconds);

            //PeopleCountingVideoCreator.CreateVideo(countingRegression, N, M, width, height, dataset.TestInput, dataset.TestOutput);
            MathN::Matrix<double> shouldBe = PeoplePositions.GridQuantize(dataset.TestOutput, N, M, 640, 480);

            sw.Restart();
            MathN::Matrix<double> predictions2 = countingRegression.Predict(dataset.TestInput);
            sw.Stop();
            Console.WriteLine("Parallelized prediction time (1200 frames): {0} ms", sw.ElapsedMilliseconds);

            File.WriteAllText("D:\\groundtruth.txt", shouldBe.ToMatrixString(5000, 5000), Encoding.UTF8);


            sw.Restart();
            double[] results = Evaluator.Evaluate(predictions2, shouldBe, new SquaredErrorCalculator(), new AbsoluteErrorCalculator(), new RelativeDeviationErrorCalculator());
            sw.Stop();
            Console.WriteLine("Eval time: {0} ms", sw.ElapsedMilliseconds);

            string resultString = String.Format("{0} ; {1}", countingRegression.ToString(), String.Join(" ; ", results));


            Console.WriteLine(resultString);

            MathN::Matrix<double> shouldBeTrain = PeoplePositions.GridQuantize(dataset.TrainOutput, N, M, 640, 480);
            Console.Write(shouldBeTrain.Row(0));

            //MathN::Matrix<double> predictions = countingRegression.Predict(dataset.ValidationInput);

            //countingRegression.Train(new List<string>(dataset.TrainInput.Concat(dataset.ValidationInput)),
            //    new List<IList<PointF>>(dataset.TrainOutput.Concat(dataset.ValidationOutput)));

            //MathN::Matrix<double> predictions = countingRegression.Predict(dataset.TestInput);
            //MathN::Matrix<double> shouldBe = PeoplePositions.GridQuantize(dataset.ValidationOutput, N, M, 640, 480);

            //double[] results = Evaluator.Evaluate(predictions, shouldBe, new SquaredErrorCalculator(), new AbsoluteErrorCalculator(), new RelativeDeviationErrorCalculator());
            //string resultString = String.Format("{0} ; {1}", countingRegression.ToString(), String.Join(" ; ", results));



            
            //object locker = new object();

            //IMultiRegression regression = new SVMRegression(1, 1e-3);
            //CrowdCountingRegression countingRegression = new CrowdCountingRegression(extractor, dataNormalizer, regression);
            //countingRegression.CachePath = Path.Combine(dataset.Path, "cache");

            ////lock (locker)
            ////{
            ////    if (File.ReadLines(experimentFile).FirstOrDefault((string s) => s.StartsWith(countingRegression.ToString() + " ;")) != null)
            ////    {
            ////        ; //already did this experiment
            ////    }
            ////}

            //Console.WriteLine(countingRegression.ToString());
            //countingRegression.Train(dataset.TrainInput, dataset.TrainOutput);

            //MathN::Matrix<double> predictions = countingRegression.Predict(dataset.ValidationInput);
            
            ////countingRegression.Train(new List<string>(dataset.TrainInput.Concat(dataset.ValidationInput)),
            ////    new List<IList<PointF>>(dataset.TrainOutput.Concat(dataset.ValidationOutput)));

            ////MathN::Matrix<double> predictions = countingRegression.Predict(dataset.TestInput);
            //MathN::Matrix<double> shouldBe = PeoplePositions.GridQuantize(dataset.ValidationOutput, N, M, 640, 480);

            //double[] results = Evaluator.Evaluate(predictions, shouldBe, new SquaredErrorCalculator(), new AbsoluteErrorCalculator(), new RelativeDeviationErrorCalculator());
            //string resultString = String.Format("{0} ; {1}", countingRegression.ToString(), String.Join(" ; ", results));

            //lock (locker)
            //{
            //    File.AppendAllLines(experimentFile, new string[] { resultString });
            //}
            ////    }
            ////});

                        
            return 0;
        }

        private static void AnalyzeWeights(string p)
        {
            string[] lines = File.ReadAllLines(p);
            double[,] weights = new double[lines.Length, 64];

            for (int i = 0; i < lines.Length; i++)
            {
                int j=0;
                foreach (string s in lines[i].Split(new string[]{"; "}, StringSplitOptions.None))
                {
                    weights[i, j] = Double.Parse(s);
                    ++j;
                }
            }


            //double[,] featureImportances = new double[64, 64];
            //int featureLen = lines.Length / 64;

            //for (int i = 0; i < lines.Length; i++)
            //{
            //    for (int j = 0; j < 64; j++)
            //    {
            //        featureImportances[i % featureLen, 0] += Math.Abs(weights[i, j]);
            //    }
            //}

            //IList<string> infShareLines = new List<string>();
            //for (int i = 0; i < 64; i++)
            //{
            //    string line = "";
            //    for (int j = 0; j < 64; j++)
            //    {
            //        line += String.Format("{0}; ", featureImportances[i, j]);
            //    }
            //    line = line.Substring(0, line.Length - 2);
            //    infShareLines.Add(line);
            //}

            double[,] infSharing = new double[64, 64];
            int featureLen = lines.Length / 64;

            double[] weightSumPerTarget = new double[64];

            for (int i = 0; i < lines.Length; i++)
            {
                for (int j = 0; j < 64; j++)
                {
                    infSharing[i / featureLen, j] += Math.Abs(weights[i, j]);
                    weightSumPerTarget[j] += Math.Abs(weights[i, j]);
                }
            }

            for (int i = 0; i < 64; i++)
            {
                for (int j = 0; j < 64; j++)
                {
                    if (weightSumPerTarget[j] != 0)
                    {
                        infSharing[i, j] /= weightSumPerTarget[j];
                    }
                }
            }

            IList<string> infShareLines = new List<string>();
            for (int i = 0; i < 64; i++)
            {
                string line = "";
                for (int j = 0; j < 64; j++)
                {
                    line += String.Format("{0}; ", infSharing[i, j]);
                }
                line = line.Substring(0, line.Length - 2);
                infShareLines.Add(line);
            }


            System.IO.File.WriteAllLines(@"D:\infSharingRelative2.txt", infShareLines);
        }

       
    }
}
