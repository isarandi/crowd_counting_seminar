using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CS
{
    interface IPerspectiveCorrector
    {
        Func<double, double, double> GetScaleFunction(double xOffset, double yOffset, int height);
    }

    class LinearPerspectiveCorrector: IPerspectiveCorrector
    {
        double scale1;
        double y1;
        double slope;

        public LinearPerspectiveCorrector(double y1, double scale1, double y2, double scale2)
        {
            this.scale1 = scale1;
            this.y1 = y1;
            this.slope = (scale2 - scale1) / (y2 - y1);
        }

        public Func<double, double, double> GetScaleFunction(double xOffset, double yOffset, int height)
        {
            return ((double x, double y) => ((y+yOffset)/height-y1)*slope + scale1);
        }

        public override string ToString()
        {
            return "linear";
        }
    }

    class IdlePerspectiveCorrector : IPerspectiveCorrector
    {
        public IdlePerspectiveCorrector()
        {
        }

        public Func<double, double, double> GetScaleFunction(double xOffset, double yOffset, int height)
        {
            return ((double x, double y) => 1);
        }

        public override string ToString()
        {
            return "no";
        }
    }
}
