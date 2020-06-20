using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Emgu.CV;

using MathN = MathNet.Numerics.LinearAlgebra.Generic;
using MathNet.Numerics.LinearAlgebra.Double;
using Emgu.CV.Structure;
using System.Drawing;

namespace CS
{
    class PeopleCountingVideoCreator
    {

        private static Font font = new Font("Arial", 20, FontStyle.Bold); //creates new font
        private static void drawText(Image<Bgr, Byte> img, Rectangle rect, Brush brush, string text)
        {
            Graphics g = Graphics.FromImage(img.Bitmap);

            int tWidth = (int)g.MeasureString(text, font).Width;
            int tHeight = (int)g.MeasureString(text, font).Height;

            int x;
            if (tWidth >= rect.Width)
                x = rect.Left - ((tWidth - rect.Width) / 2);
            else
                x = (rect.Width / 2) - (tWidth / 2) + rect.Left;

            int y;
            if (tHeight >= rect.Height)
                y = rect.Top - ((tHeight - rect.Height) / 2);
            else
                y = (rect.Height / 2) - (tHeight / 2) + rect.Top;

            g.DrawString(text, font, brush, new PointF(x, y));
        }

        public static void CreateVideo(CrowdCountingRegression regression, int N, int M, int width, int height, IEnumerable<string> frames, List<IList<PointF>> outputs)
        {
            MathN::Matrix<double> shouldBe = PeoplePositions.GridQuantize(outputs, N, M, width, height);

            using (VideoWriter numberVideoWriter = new VideoWriter("D:\\video_predictions.avi", fps: 1, width: width, height: height, isColor: true))
            using (VideoWriter differenceVideoWriter = new VideoWriter("D:\\video_differences.avi", fps: 1, width: width, height: height, isColor: true))
            {

                int cellHeight = height / N;
                int cellWidth = width / M;

                MathN::Matrix<double> prediction = regression.Predict(new List<string>(frames));
                int frameID = 0;
                foreach (string framePath in frames)
                {
                    using (Image<Bgr, Byte> countFrame = new Image<Bgr, Byte>(framePath))
                    using (Image<Bgr, Byte> differenceFrame = new Image<Bgr, Byte>(framePath))
                    {

                        for (int i = 1; i < N; ++i)
                        {
                            LineSegment2D line = new LineSegment2D(
                                new Point(0, i * cellHeight),
                                new Point(width, i * cellHeight));
                            countFrame.Draw(line, new Bgr(Color.Yellow), 2);
                            differenceFrame.Draw(line, new Bgr(Color.Red), 2);
                        }

                        for (int j = 1; j < M; ++j)
                        {
                            LineSegment2D line = new LineSegment2D(
                                new Point(j * cellWidth, 0),
                                new Point(j * cellWidth, height));
                            countFrame.Draw(line, new Bgr(Color.Yellow), 2);
                            differenceFrame.Draw(line, new Bgr(Color.Red), 2);
                        }


                        for (int i = 0; i < N; ++i)
                        {
                            for (int j = 0; j < M; ++j)
                            {
                                double cellPrediction = prediction[frameID, i * M + j];
                                int cellShoudlBe = (int)Math.Round(shouldBe[frameID, i * M + j]);
                                Rectangle rect = new Rectangle(j * cellWidth, i * cellHeight, cellWidth, cellHeight);

                                drawText(countFrame, rect, Brushes.Yellow, String.Format("{0:0.0}",cellPrediction));

                                double difference = (cellPrediction - cellShoudlBe);
                                string differenceString = difference > 0 ? "+"+ String.Format("{0:0.0}",difference) :  String.Format("{0:0.0}",difference);
                                drawText(differenceFrame, rect, Brushes.Red, differenceString);

                            }

                        }

                        numberVideoWriter.WriteFrame(countFrame);
                        differenceVideoWriter.WriteFrame(differenceFrame);

                    }

                    frameID++;
                }

            }

            
        }
    }
}
