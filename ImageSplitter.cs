using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Emgu.CV;
using System.Drawing;

namespace CS
{
    class ImageSplitter
    {
        public static IList<Image<TColor, TDepth>> SplitImage<TColor, TDepth>(Image<TColor, TDepth> input, int N, int M)
            where TColor : struct, global::Emgu.CV.IColor
            where TDepth : new()
        {
            int tileWidth = input.Width/M;
            int tileHeight = input.Height/N;

            Rectangle roiBackup = input.ROI;

            IList<Image<TColor, TDepth>> result = new List<Image<TColor, TDepth>>();

            for (int i = 0; i < N; ++i)
            {
                for (int j = 0; j < M; ++j)
                {
                    input.ROI = new Rectangle(j * tileWidth, i * tileHeight, tileWidth, tileHeight);
                    result.Add(input.Clone());
                }
            }

            input.ROI = roiBackup;

            return result;
        }
    }
}
