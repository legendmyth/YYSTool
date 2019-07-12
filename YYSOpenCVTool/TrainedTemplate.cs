using OpenCvSharp.CPlusPlus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YYSOpenCVTool
{
    public class TrainedTemplate
    {
        public Mat templateImage { get; set; }

        public Mat templateDescriptors { get; set; }

        public KeyPoint[] templateKeyPoints { get; set; }

        public void Train()
        {

        }

    }
}
