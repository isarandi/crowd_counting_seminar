using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Math = MathNet.Numerics.LinearAlgebra.Generic;
using System.IO;

namespace CS.Evaluation
{
    class Evaluator
    {
        public static double[] Evaluate(Math::Matrix<double> predictions, Math::Matrix<double> shouldBe, params ErrorCalculator[] ecs)
        {
            List<string> diffStrings = new List<string>();
            for (int row = 0; row < predictions.RowCount; row++)
            {
                double desiredSum = 0;
                double actualSum = 0;
			    for (int col = 0; col < predictions.ColumnCount; col++)
			    {
                    desiredSum += shouldBe[row, col];
                    actualSum += predictions[row, col];
    			}

                diffStrings.Add(String.Format("{0} {1}", desiredSum, actualSum));
            }
            File.WriteAllLines(@"D:\! Egyetem\! RWTH\Semester 2\Seminar CV\Datasets\Mall\analysis\differences.txt", diffStrings);
            
            double[] results = new double[ecs.Length];
            int i=0;
            foreach (ErrorCalculator ec in ecs)
            {
                results[i++] = Evaluate(predictions, shouldBe, ec);
            }
            return results;
        }

        public static double Evaluate(Math::Matrix<double> predictions, Math::Matrix<double> shouldBe, ErrorCalculator ec)
        {
            double errorSum = 0;

            for (int row = 0; row < predictions.RowCount; row++)
			{
                double desiredSum = 0;
                double actualSum = 0;
			    for (int col = 0; col < predictions.ColumnCount; col++)
			    {
                    desiredSum += shouldBe[row, col];
                    actualSum += predictions[row, col];
    			}

                errorSum += ec.CalculateError(actualSum, desiredSum);
			}

            return errorSum / (predictions.RowCount);
        }

    }
}
