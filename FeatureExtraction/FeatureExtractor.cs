using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Math = MathNet.Numerics.LinearAlgebra.Generic;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
using MathNet.Numerics.LinearAlgebra.Double;
using Emgu.CV.UI;

namespace CS.FeatureExtraction
{
    class FeatureExtractor
    {
        private CellFeatureExtractor cellFeatureExtractor = new CellFeatureExtractor();

        private int frameWidth;
        private int frameHeight;

        private IPerspectiveCorrector perspectiveCorrector;
        private Image<Gray, byte> background;
        //BackgroundSubtractor backgroundSubtractor = new BackgroundSubtractorMOG2(25, 30.0f, false);

        StructuringElementEx structuringElement;
               
        public FeatureExtractor(IPerspectiveCorrector perspectiveCorrector, int N, int M,  int frameWidth, int frameHeight, Image<Bgr, byte> backgroundImage)
        {
            this.perspectiveCorrector = perspectiveCorrector;
            this.background = backgroundImage.Resize(frameWidth, frameHeight, INTER.CV_INTER_AREA).Convert<Gray, byte>();
            this.N = N;
            this.M = M;
            this.frameWidth = frameWidth;
            this.frameHeight = frameHeight;

            structuringElement = new StructuringElementEx(3, 3, 1, 1, CV_ELEMENT_SHAPE.CV_SHAPE_ELLIPSE);
        }

        int i = 0;
        public Math::Vector<double> ExtractFeatures(Image<Gray, Byte> frame)
        {
            Math::Vector<double> result = new DenseVector(Length);

            Image<Gray, Byte> grayFrame = frame.Resize(frameWidth, frameHeight, INTER.CV_INTER_AREA);
            Image<Gray, Byte> foregroundMask =
                    grayFrame.AbsDiff(background).ThresholdBinary(new Gray(40), new Gray(255)) -
                    grayFrame.ThresholdBinary(new Gray(200), new Gray(255));


            CvInvoke.cvDilate(foregroundMask, foregroundMask, structuringElement, 1);
            CvInvoke.cvErode(foregroundMask, foregroundMask, structuringElement, 2);
            CvInvoke.cvDilate(foregroundMask, foregroundMask, structuringElement, 1);
            
            //Image<Gray, byte> outline = foregroundMask - foregroundMask.Erode(1);
            //Image<Gray, byte> edges = grayFrame.Canny(80, 150);
            //Image<Gray, byte> foregroundEdges = edges.Min(foregroundMask.Dilate(1));
            //outline.Save(@"D:\! Egyetem\! RWTH\Semester 2\Seminar CV\Datasets\UCSD\derived images\outlines\" + i + ".png");
            //foregroundEdges.Save(@"D:\! Egyetem\! RWTH\Semester 2\Seminar CV\Datasets\UCSD\derived images\edges\" + i + ".png");
            //foregroundMask.Save(@"D:\! Egyetem\! RWTH\Semester 2\Seminar CV\Datasets\UCSD\derived images\masks\" + i + ".png");
            //i++;
            
            IList<Image<Gray, Byte>> frameCells = ImageSplitter.SplitImage(grayFrame, N, M);
            IList<Image<Gray, Byte>> foregroundMaskCells = ImageSplitter.SplitImage(foregroundMask, N, M);

            int cellHeight = frame.Height / N;
            int cellWidth = frame.Width / M;

            for (int cellId = 0; cellId < frameCells.Count; ++cellId)
            {
                int offsetX = (cellId % M) * cellWidth;
                int offsetY = (cellId / M) * cellHeight;

                Math::Vector<double> featureVector =
                    cellFeatureExtractor.ExtractFeatures(
                    frameCells[cellId], foregroundMaskCells[cellId],
                    perspectiveCorrector.GetScaleFunction(offsetX, offsetY, frameHeight));

                result.SetSubVector(featureVector.Count * cellId, featureVector.Count, featureVector);
            }

            return result;
        }

        public int Length
        {
            get { return N * M * cellFeatureExtractor.Length; }
        }

        private int _N;

        public int N
        {
            get { return _N; }
            private set { _N = value; }
        }

        private int _M;

        public int M
        {
            get { return _M; }
            private set { _M = value; }
        }

        public override string ToString()
        {
            return String.Format("Experimental width={0}, height={1}, cellRows={2}, cellCols={3}, perspective={4}", frameWidth, frameHeight, N, M, perspectiveCorrector.ToString());
        }
    }
}
