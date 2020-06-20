using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Emgu.CV;
using Emgu.CV.Structure;

namespace CS.FeatureExtraction
{
    class GLCMCalculator
    {
        public static double[] GetGLCMFeatures(Image<Gray, byte> image)
        {
            Image<Gray, byte> quantizedImage = image.Clone();
            for (int y = 0; y < quantizedImage.Height; y++)
			{
			    for (int x = 0; x < quantizedImage.Width; x++)
			    {
                    if (image[y,x].Intensity == 0)
			            quantizedImage[y,x] = new Gray(0);
                    else
                        quantizedImage[y,x] = new Gray(image[y,x].Intensity / 32.0 + 1.0/256.0);
			    }
			}

            double[,,] glcms = new double[4,8,8];
            double[] nPairs = new double[4];

            int d = 1;
            for (int y = d; y < quantizedImage.Height - d; y++)
			{
                for (int x = d; x < quantizedImage.Width - d; x++)
			    {
                    if (quantizedImage[y,x].Intensity == 0)
                        continue;

                    int thisLevel = ((int) quantizedImage[y,x].Intensity)-1;
                    int[] otherlevels = new int[4];
                    otherlevels[0] = ((int)quantizedImage[y - d, x].Intensity) - 1;
                    otherlevels[1] = ((int)quantizedImage[y - d, x + d].Intensity) - 1;
                    otherlevels[2] = ((int)quantizedImage[y, x + d].Intensity) - 1;
                    otherlevels[3] = ((int)quantizedImage[y + d, x + d].Intensity) - 1;

                    for (int angle = 0; angle < 4; angle++)
			        {
                        if (otherlevels[angle]!=-1)
                        {
                            ++glcms[angle,thisLevel,otherlevels[angle]];
                            ++nPairs[angle];
                        }
                    }
			    }
			}

            double[] homogeneity = new double[4];
            double[] energy = new double[4];
            double[] entropy = new double[4];


            for (int angle = 0; angle < 4; angle++)
            {
                if (nPairs[angle] == 0)
                    continue;

                for (int i=0; i<8; ++i)
                {
                    for (int j=0; j<8; ++j)
                    {
                        double f = glcms[angle,i,j]/nPairs[angle];

			            homogeneity[angle]+=f/(1.0+(i-j)*(i-j));
                        energy[angle]+=f*f;

                        if (f > Double.Epsilon)
                        {
                            entropy[angle] += -f * Math.Log(f);
                        }
			        }
                }
            }

            double[] result = new double[12];

            for (int i = 0; i < 12; i++)
			{
			    switch (i/4)
                {
                    case 0:
                        result[i] = homogeneity[i%4];
                        break;
                    case 1:
                        result[i] = energy[i%4];
                        break;
                    case 2:
                        result[i] = entropy[i%4];
                        break;
                }
			}

            return result;
        }
    }
}
