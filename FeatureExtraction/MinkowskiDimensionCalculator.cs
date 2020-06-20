using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
using MathN = MathNet.Numerics.LinearAlgebra.Generic;
using MathNet.Numerics.LinearAlgebra.Double;
using Emgu.CV.UI;

namespace CS.FeatureExtraction
{
    class MinkowskiDimensionCalculator
    {

        public static double GetMinkowskiDimension(Image<Gray, byte> image)
        {
            Image<Gray, byte> dilated = new Image<Gray, byte>(image.Width, image.Height);

            int nMaxDisk = 15;
            int diskStep = 1;
            int nDiskSteps = ((nMaxDisk-2) / diskStep)+1;

            MathN::Matrix<double> X = new DenseMatrix(nDiskSteps, 2);
            MathN::Vector<double> Y = new DenseVector(nDiskSteps);

            X[0, 0] = 1;
            X[0, 1] = Math.Log(image.CountNonzero()[0]);

            int row = 1;
            for (int diskSize = 1+diskStep; diskSize < nMaxDisk; diskSize+=diskStep)
            {
                StructuringElementEx kernel = new StructuringElementEx(diskSize, diskSize, (diskSize-1) / 2, (diskSize - 1) / 2, CV_ELEMENT_SHAPE.CV_SHAPE_ELLIPSE);
                CvInvoke.cvDilate(image, dilated, kernel, 1);

                X[row, 0] = 1;
                X[row, 1] = Math.Log(diskSize);
                Y[row] = Math.Log(dilated.CountNonzero()[0]);

                if (Double.IsNegativeInfinity(Y[row]))
                {
                    Y[row] = 0;
                }
                ++row;
            }

            MathN::Vector<double> W = X.QR().Solve(Y);

            double slope = W[1];
            return 2 - slope;
        }
    }
}
