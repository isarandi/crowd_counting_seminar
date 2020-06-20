using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;

namespace CS
{
    class DataSet
    {
        public String Path { get; set; }
        public List<string> TrainInput { get; set; }
        public List<string> ValidationInput { get; set; }
        public List<string> TrainPlusValidationInput { get; set; }

        public List<string> TestInput { get; set; }

        public int DefaultHeight { get; set; }
        public int DefaultWidth { get; set; }

        public int DefaultGridN { get; set; }
        public int DefaultGridM { get; set; }

        public List<IList<PointF>> TrainOutput { get; set; }
        public List<IList<PointF>> ValidationOutput { get; set; }
        public List<IList<PointF>> TrainPlusValidationOutput { get; set; }
        public List<IList<PointF>> TestOutput { get; set; }

        public IPerspectiveCorrector LinearCorrector { get; set; }
    }

    class DataSets
    {

        public DataSet LoadMall()
        {
            string datasetPath = @"D:\! Egyetem\! RWTH\Semester 2\Seminar CV\Datasets\Mall\";

            List<string> allFrames = new List<string>(Directory.GetFiles(Path.Combine(datasetPath, "frames")).Where((s) => s.EndsWith("jpg")));
            List<IList<PointF>> peoplePos = new List<IList<PointF>>(PeoplePositions.ReadFromFile(Path.Combine(datasetPath, "people_positions.txt")));

            int nTrain = 640;
            int nValidation = 800 - 640;
            int nTest = 2000 - 800;

            DataSet ds = new DataSet();

            ds.DefaultWidth = 640;
            ds.DefaultHeight = 480;

            ds.DefaultGridN = 8;
            ds.DefaultGridM = 8;
            ds.LinearCorrector = new LinearPerspectiveCorrector(75 / 480.0, 53.0, 408 / 480.0, 143.0);
            ds.LinearCorrector = new LinearPerspectiveCorrector(75 / 480.0, 53.0, 408 / 480.0, 143.0);

            ds.Path = datasetPath;
            ds.TrainInput = allFrames.GetRange(0, nTrain);
            ds.ValidationInput = allFrames.GetRange(nTrain, nValidation);
            ds.TestInput = allFrames.GetRange(nTrain + nValidation, nTest);

            ds.TrainPlusValidationInput = new List<string>(ds.TrainInput.Concat(ds.ValidationInput));

            ds.TrainOutput = peoplePos.GetRange(0, nTrain);
            ds.ValidationOutput = peoplePos.GetRange(nTrain, nValidation);
            ds.TestOutput = peoplePos.GetRange(nTrain + nValidation, nTest);

            ds.TrainPlusValidationOutput = new List<IList<PointF>>(ds.TrainOutput.Concat(ds.ValidationOutput));
            return ds;
        }

        public DataSet LoadUCSD()
        {
            string datasetPath = @"D:\! Egyetem\! RWTH\Semester 2\Seminar CV\Datasets\UCSD\";

            List<string> allFrames = new List<string>(Directory.GetFiles(Path.Combine(datasetPath, "frames")).Where((s) => s.EndsWith("png")));
            List<IList<PointF>> peoplePos = new List<IList<PointF>>(PeoplePositions.ReadFromFile(Path.Combine(datasetPath, "people_positions.txt")));

            int nTrain = 640;
            int nValidation = 800 - 640;
            int nTest = 2000 - 800;

            DataSet ds = new DataSet();

            ds.DefaultWidth = 238;
            ds.DefaultHeight = 158;
            ds.DefaultGridN = 4;
            ds.DefaultGridM = 6;

            ds.Path = datasetPath;
            ds.TrainInput = allFrames.GetRange(0, nTrain);
            ds.ValidationInput = allFrames.GetRange(nTrain, nValidation);
            ds.TestInput = allFrames.GetRange(nTrain + nValidation, nTest);

            ds.TrainOutput = peoplePos.GetRange(0, nTrain);
            ds.ValidationOutput = peoplePos.GetRange(nTrain, nValidation);
            ds.TestOutput = peoplePos.GetRange(nTrain + nValidation, nTest);
            return ds;
        }

    }
}
