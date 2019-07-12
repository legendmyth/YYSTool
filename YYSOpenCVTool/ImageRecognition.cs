using OpenCvSharp;
using OpenCvSharp.CPlusPlus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YYSOpenCVTool
{
    public class ImageRecognition
    {

        private float nndrRatio = 0.7f;//这里设置既定值为0.7，该值可自行调整
        private int matchesPointCount = 0;
        public float getNndrRatio()
        {
            return nndrRatio;
        }
        public void setNndrRatio(float nndrRatio)
        {
            this.nndrRatio = nndrRatio;
        }
        public int getMatchesPointCount()
        {
            return matchesPointCount;
        }
        public void setMatchesPointCount(int matchesPointCount)
        {
            this.matchesPointCount = matchesPointCount;
        }
        public void matchImage(Mat templateImage, Mat originalImage)
        {

            DateTime start = DateTime.Now;
            //指定特征点算法SURF
            SURF featureDetector = new SURF();
            //获取模板图的特征点
            KeyPoint[] templateKeyPoints = featureDetector.Detect(templateImage);

            //提取模板图的特征点
            Mat templateDescriptors = new Mat(templateImage.Rows, templateImage.Cols, templateImage.Type());
            SURF descriptorExtractor = new SURF();
            descriptorExtractor.Compute(templateImage, ref templateKeyPoints, templateDescriptors);

            DateTime begin = DateTime.Now;
            //获取原图的特征点
            Mat originalDescriptors = new Mat();
            KeyPoint[] originalKeyPoints = featureDetector.Detect(originalImage);
            //Console.WriteLine("提取原图的特征点");
            descriptorExtractor.Compute(originalImage, ref originalKeyPoints, originalDescriptors);

            
            DescriptorMatcher descriptorMatcher = DescriptorMatcher.Create("FlannBased");
            //Console.WriteLine("寻找最佳匹配");
            /**
             * knnMatch方法的作用就是在给定特征描述集合中寻找最佳匹配
             * 使用KNN-matching算法，令K=2，则每个match得到两个最接近的descriptor，然后计算最接近距离和次接近距离之间的比值，当比值大于既定值时，才作为最终match。
             */
            DMatch[][] matches = descriptorMatcher.KnnMatch(templateDescriptors, originalDescriptors, 2);
            //Console.WriteLine("计算匹配结果");
            LinkedList<DMatch> goodMatchesList = new LinkedList<DMatch>();
            foreach (DMatch[] match in matches)
            {
                DMatch[] dmatcharray = match.ToArray();
                DMatch m1 = dmatcharray[0];
                DMatch m2 = dmatcharray[1];
                if (m1.Distance <= m2.Distance * nndrRatio)
                {
                    goodMatchesList.AddLast(m1);
                }
            }
            matchesPointCount = goodMatchesList.Count;
            //当匹配后的特征点大于等于 4 个，则认为模板图在原图中，该值可以自行调整
            if (matchesPointCount >= 30)
            {
                //Console.WriteLine("模板图在原图匹配成功！");
                List<KeyPoint> templateKeyPointList = templateKeyPoints.ToList();
                List<KeyPoint> originalKeyPointList = originalKeyPoints.ToList();
                LinkedList<Point2d> objectPoints = new LinkedList<Point2d>();
                LinkedList<Point2d> scenePoints = new LinkedList<Point2d>();
                foreach (DMatch goodMatch in goodMatchesList)
                {
                    objectPoints.AddLast(new Point2d(templateKeyPointList[goodMatch.QueryIdx].Pt.X, templateKeyPointList[goodMatch.QueryIdx].Pt.Y));
                    scenePoints.AddLast(new Point2d(originalKeyPointList[goodMatch.TrainIdx].Pt.X, originalKeyPointList[goodMatch.TrainIdx].Pt.Y));
                }
                MatOfPoint2f objMatOfPoint2f = new MatOfPoint2f();
                
                foreach (Point2d p in objectPoints)
                {
                    objMatOfPoint2f.Add(new Point2f((float)p.X, (float)p.Y));
                }

                MatOfPoint2f scnMatOfPoint2f = new MatOfPoint2f();
                foreach (Point2d p in scenePoints)
                {
                    scnMatOfPoint2f.Add(new Point2f((float)p.X, (float)p.Y));
                }
                //使用 findHomography 寻找匹配上的关键点的变换
                Mat homography = Cv2.FindHomography(objMatOfPoint2f, scnMatOfPoint2f, OpenCvSharp.HomographyMethod.Ransac, 3);

                /**
                 * 透视变换(Perspective Transformation)是将图片投影到一个新的视平面(Viewing Plane)，也称作投影映射(Projective Mapping)。
                 */
                Mat templateCorners = new Mat(4, 1, MatType.CV_64FC2);
                Mat templateTransformResult = new Mat(4, 1, MatType.CV_64FC2);
                templateCorners.Set<Point2d>(0, 0, new Point2d(0, 0));
                templateCorners.Set<Point2d>(1, 0, new Point2d(templateImage.Cols, 0 ));
                templateCorners.Set<Point2d>(2, 0, new Point2d(templateImage.Cols, templateImage.Rows ));
                templateCorners.Set<Point2d>(3, 0, new Point2d(0, templateImage.Rows ));

                //使用 perspectiveTransform 将模板图进行透视变以矫正图象得到标准图片
                Cv2.PerspectiveTransform(templateCorners, templateTransformResult, homography);

                //矩形四个顶点
                Point2d pointA = templateTransformResult.Get<Point2d>(0, 0);
                Point2d pointB = templateTransformResult.Get<Point2d>(1, 0);
                Point2d pointC = templateTransformResult.Get<Point2d>(2, 0);
                Point2d pointD = templateTransformResult.Get<Point2d>(3, 0);

                //将匹配的图像用用四条线框出来
                Cv2.Line(originalImage, pointA, pointB, new Scalar(0, 255, 0), 1);//上 A->B
                Cv2.Line(originalImage, pointB, pointC, new Scalar(0, 255, 0), 1);//右 B->C
                Cv2.Line(originalImage, pointC, pointD, new Scalar(0, 255, 0), 1);//下 C->D
                Cv2.Line(originalImage, pointD, pointA, new Scalar(0, 255, 0), 1);//左 D->A

                Cv2.PutText(originalImage, "time:" + DateTime.Now.Subtract(begin).TotalMilliseconds, new Point(10, 20), FontFace.HersheySimplex, 0.5, new Scalar(0, 255, 0));
                Cv2.ImWrite("D:\\Users\\Administrator\\Desktop\\tmp2.jpg", originalImage);
            }
        }
    }
}
