using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing;
using MathNet.Numerics.LinearAlgebra.Generic;
using MathNet.Numerics.LinearAlgebra.Double;

namespace CS
{
    class PeoplePositions
    {
        private static Regex regex = new Regex(@"(\d+\.\d+), (\d+\.\d+).*");

        public static IList<IList<PointF>> ReadFromFile(String path)
        {
            IList<IList<PointF>> frames = new List<IList<PointF>>();
            IList<PointF> currentFrame = null;

            foreach (string line in System.IO.File.ReadAllLines(path))
            {
                if (line.Contains("Frame"))
                {
                    if (currentFrame != null)
                    {
                        frames.Add(currentFrame);
                    }
                    currentFrame = new List<PointF>();
                }
                else if (line.Contains(','))
                {
                    Match m = regex.Match(line);
                    currentFrame.Add(new PointF(Single.Parse(m.Groups[1].Value), Single.Parse(m.Groups[2].Value)));
                }
            }

            frames.Add(currentFrame);

            return frames;
        }

        public static Matrix<double> GridQuantize(IList<IList<PointF>> positions, int N, int M, int width, int height)
        {

            Matrix<double> Y = new DenseMatrix(positions.Count, N * M);
            for (int frameID = 0; frameID < positions.Count; frameID++)
            {
                foreach (PointF pos in positions[frameID])
                {
                    int binX = (int)((pos.X / width) * M);
                    int binY = (int)((pos.Y / height) * N);

                    ++Y[frameID, binY * M + binX];
                }
            }
            return Y;
        }
    }
}
