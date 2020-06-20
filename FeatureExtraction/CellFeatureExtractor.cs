using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathNet.Numerics.LinearAlgebra.Generic;
using Emgu.CV;
using Emgu.CV.Structure;
using MathNet.Numerics.LinearAlgebra.Double;
using Emgu.CV.VideoSurveillance;
using Emgu.CV.UI;

namespace CS.FeatureExtraction
{
    class CellFeatureExtractor
    {
        public double GetWeightedSum(Image<Gray, byte> binaryInput, Func<double, double, double> scaleAt, bool isEdgeType)
        {
            double result = 0;
            for (int x = 0; x < binaryInput.Width; ++x)
            {
                for (int y = 0; y < binaryInput.Height; y++)
                {
                    if (binaryInput[y, x].Intensity != 0)
                    {
                        if (isEdgeType)
                        {
                            result += 1.0 / scaleAt(x, y);
                        }
                        else
                        {
                            result += 1.0 / Math.Pow(scaleAt(x, y),2);
                        }
                    }
                }
            }

            return result;
        }

        public double[] GetOrientationHistogram(Image<Gray, byte> image, Func<double, double, double> scaleAt, int nBins)
        {
            Image<Gray, float> sobelX = image.Sobel(1, 0, 3);
            Image<Gray, float> sobelY = image.Sobel(0, 1, 3);
            double[] histogram = new double[nBins];

            for (int x = 0; x < image.Width; x++)
            {
                for (int y = 0; y < image.Height; y++)
                {
                    double gradX = sobelX[y, x].Intensity;
                    double gradY = sobelY[y, x].Intensity;
                    //double length = Math.Sqrt(gradX * gradX + gradY * gradY);

                    if (image[y,x].Intensity != 0)
                    {
                        double angle = Math.Atan(gradY / gradX) + Math.PI / 2;
                        int idx = (int)(angle / Math.PI * nBins);
                        if (idx < 0)
                            idx = 0;
                        if (idx > nBins - 1)
                            idx = nBins - 1;

                        histogram[idx] += 1.0 / scaleAt(x, y);
                    }
                }
            }

            return histogram;
        }

        public Vector<double> ExtractFeatures(Image<Gray, byte> image, Image<Gray, byte> foregroundMask, Func<double, double, double> scaleAt)
        {
            //ImageViewer.Show(foregroundMask);

            if (foregroundMask.CountNonzero()[0] == 0)
            {
                return DenseVector.Create(Length, (int i_)=>0);
            }

            

            double weightedArea = foregroundMask.CountNonzero()[0];//GetWeightedSum(foregroundMask, scaleAt, false);
            
            //Image<Gray, byte> outline = foregroundMask - foregroundMask.Erode(1);
            //double weightedPerimeter = GetWeightedSum(outline, scaleAt, true);

            //double ratio = weightedPerimeter/weightedArea;

            //double[] segmentFeatures = new double[]{weightedArea, weightedPerimeter, ratio};
            //double[] perimeterHistogram = GetOrientationHistogram(outline, scaleAt, 6);

            //Image<Gray,byte> edges = image.Canny(80,150);

            //Image<Gray, byte> foregroundEdges = edges.Min(foregroundMask.Dilate(1));
            ////ImageViewer.Show(foregroundEdges);

            //double weightedEdgePixels;
            //double[] edgeHistogram;
            //double minkowskiDimension;
            //double[] glcmFeatures;
            //if (foregroundEdges.CountNonzero()[0] == 0)
            //{
            //    weightedEdgePixels = 0.0;
            //    edgeHistogram = new double[6];
            //    minkowskiDimension = 1;
            //    glcmFeatures = new double[12];
            //}
            //else
            //{
            //    weightedEdgePixels = foregroundEdges.CountNonzero()[0];// GetWeightedSum(foregroundEdges, scaleAt, true);
            //    edgeHistogram = GetOrientationHistogram(foregroundEdges, scaleAt, 6);
            //    minkowskiDimension = MinkowskiDimensionCalculator.GetMinkowskiDimension(foregroundEdges);
            //    glcmFeatures = GLCMCalculator.GetGLCMFeatures(image.Min(foregroundMask));
            //}

            //Vector<double> result = new DenseVector(segmentFeatures.Length + perimeterHistogram.Length + 1 + 1 + edgeHistogram.Length + glcmFeatures.Length);
            //int i=0;
            //result.SetSubVector(i, segmentFeatures.Length, new DenseVector(segmentFeatures));
            //i += segmentFeatures.Length;
            //result.SetSubVector(i, perimeterHistogram.Length, new DenseVector(perimeterHistogram));
            //i += perimeterHistogram.Length;
            //result[i] = weightedEdgePixels;
            //i += 1;
            //result[i] = minkowskiDimension;
            //i += 1;
            //result.SetSubVector(i, edgeHistogram.Length, new DenseVector(edgeHistogram));
            //i += edgeHistogram.Length;
            //result.SetSubVector(i, glcmFeatures.Length, new DenseVector(glcmFeatures));

            //for (int j = 0; j < result.Count; j++)
            //{
            //    if (Double.IsNaN(result[j]))
            //    {
            //        Console.WriteLine(j);
            //    }
            //}

            Vector<double> result = new DenseVector(1);
            result[0] = weightedArea;

            return result;
        }



        public int Length
        {
            get { return 1; }
        }
        

    }
}
