using OpenCvSharp;
using OpenCvSharp.CPlusPlus;
using OpenCvSharp.CPlusPlus.Flann;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YYSOpenCVTool
{
    public class ImageRecognition
    {

        /// <summary>
        /// 图像匹配
        /// </summary>
        /// <param name="templateImage">模板图</param>
        /// <param name="originalImage">原图</param>
        /// <param name="nndrRatio">距离阈值，一般取0.5</param>
        public static void matchImage(Mat templateImage, Mat originalImage,float nndrRatio)
        {

            DateTime start = DateTime.Now;
            //指定特征点算法SURF
            SURF surf = new SURF();


            //获取模板图的特征点
            KeyPoint[] templateKeyPoints = surf.Detect(templateImage);
            //提取模板图的特征描述
            //Mat templateDescriptors = new Mat(templateImage.Rows, templateImage.Cols, templateImage.Type());
            Mat templateDescriptors = new Mat();
            surf.Compute(templateImage, ref templateKeyPoints, templateDescriptors);


            
            //获取原图的特征点            
            KeyPoint[] originalKeyPoints = surf.Detect(originalImage);
            //提取原图的特征点描述;
            Mat originalDescriptors = new Mat();
            surf.Compute(originalImage, ref originalKeyPoints, originalDescriptors);

            
            //开始匹配
            DescriptorMatcher descriptorMatcher = DescriptorMatcher.Create("FlannBased");//或者使用
            /**
             * knnMatch方法的作用就是在给定特征描述集合中寻找最佳匹配
             * 使用KNN-matching算法，令K=2，则每个match得到两个最接近的descriptor，然后计算最接近距离和次接近距离之间的比值，当比值大于既定值时，才作为最终match。
             */
            DMatch[][] matches = descriptorMatcher.KnnMatch(templateDescriptors, originalDescriptors, 2);
            List<DMatch> goodMatchesList = new List<DMatch>();
            foreach (DMatch[] match in matches)
            {
                DMatch m1 = match[0];
                DMatch m2 = match[1];
                if (m1.Distance <= m2.Distance * nndrRatio)
                {
                    goodMatchesList.Add(m1);
                }
            }
            //当匹配后的特征点大于等于 4 个，则认为模板图在原图中，该值可以自行调整
            if (goodMatchesList.Count >= 4)
            {
                //Console.WriteLine("模板图在原图匹配成功！");
                List<KeyPoint> templateKeyPointList = templateKeyPoints.ToList();
                List<KeyPoint> originalKeyPointList = originalKeyPoints.ToList();
                List<Point2f> objectPoints = new List<Point2f>();
                List<Point2f> scenePoints = new List<Point2f>();
                foreach (DMatch goodMatch in goodMatchesList)
                {
                    objectPoints.Add(templateKeyPointList[goodMatch.QueryIdx].Pt);
                    scenePoints.Add(originalKeyPointList[goodMatch.TrainIdx].Pt);
                }
                MatOfPoint2f objMatOfPoint2f = new MatOfPoint2f();
                foreach (Point2f p in objectPoints)
                {
                    objMatOfPoint2f.Add(p);
                }

                MatOfPoint2f scnMatOfPoint2f = new MatOfPoint2f();
                foreach (Point2f p in scenePoints)
                {
                    scnMatOfPoint2f.Add(p);
                }
                //使用 findHomography 寻找匹配上的关键点的变换
                Mat homography = Cv2.FindHomography(objMatOfPoint2f, scnMatOfPoint2f, OpenCvSharp.HomographyMethod.Ransac, 3);

                /**
                 * 透视变换(Perspective Transformation)是将图片投影到一个新的视平面(Viewing Plane)，也称作投影映射(Projective Mapping)。
                 */
                Mat templateCorners = new Mat(4, 1, MatType.CV_32FC2);
                Mat templateTransformResult = new Mat(4, 1, MatType.CV_32FC2);
                templateCorners.Set<Point2f>(0, 0, new Point2f(0, 0));
                templateCorners.Set<Point2f>(1, 0, new Point2f(templateImage.Cols, 0 ));
                templateCorners.Set<Point2f>(2, 0, new Point2f(templateImage.Cols, templateImage.Rows ));
                templateCorners.Set<Point2f>(3, 0, new Point2f(0, templateImage.Rows ));

                //使用 perspectiveTransform 将模板图进行透视变以矫正图象得到标准图片
                Cv2.PerspectiveTransform(templateCorners, templateTransformResult, homography);

                //矩形四个顶点
                Point2f pointA = templateTransformResult.Get<Point2f>(0, 0);
                Point2f pointB = templateTransformResult.Get<Point2f>(1, 0);
                Point2f pointC = templateTransformResult.Get<Point2f>(2, 0);
                Point2f pointD = templateTransformResult.Get<Point2f>(3, 0);

                //将匹配的图像用用四条线框出来
                Cv2.Line(originalImage, pointA, pointB, new Scalar(0, 255, 0), 1);//上 A->B
                Cv2.Line(originalImage, pointB, pointC, new Scalar(0, 255, 0), 1);//右 B->C
                Cv2.Line(originalImage, pointC, pointD, new Scalar(0, 255, 0), 1);//下 C->D
                Cv2.Line(originalImage, pointD, pointA, new Scalar(0, 255, 0), 1);//左 D->A

                Cv2.PutText(originalImage, "time:" + DateTime.Now.Subtract(start).TotalMilliseconds+"ms", new Point(10, originalImage.Height-10), FontFace.HersheySimplex, 0.5, new Scalar(255, 255, 255));
                Cv2.ImWrite(@"C:\Users\Administrator\Desktop\result.jpg", originalImage);
            }
        }

        public static void matchImageORB(Mat templateImage, Mat originalImage, float nndrRatio)
        {

            DateTime start = DateTime.Now;
            //指定特征点算法SURF
            ORB surf = new ORB();


            //获取模板图的特征点
            KeyPoint[] templateKeyPoints = surf.Detect(templateImage);
            //提取模板图的特征描述
            //Mat templateDescriptors = new Mat(templateImage.Rows, templateImage.Cols, templateImage.Type());
            Mat templateDescriptors = new Mat();
            surf.Compute(templateImage, ref templateKeyPoints, templateDescriptors);



            //获取原图的特征点            
            KeyPoint[] originalKeyPoints = surf.Detect(originalImage);
            //提取原图的特征点描述;
            Mat originalDescriptors = new Mat();
            surf.Compute(originalImage, ref originalKeyPoints, originalDescriptors);


            
            
            //FlannBasedMatcher descriptorMatcher = new FlannBasedMatcher();

            //开始匹配
            DescriptorMatcher descriptorMatcher = DescriptorMatcher.Create("FlannBased");//或者使用
            /**
             * knnMatch方法的作用就是在给定特征描述集合中寻找最佳匹配
             * 使用KNN-matching算法，令K=2，则每个match得到两个最接近的descriptor，然后计算最接近距离和次接近距离之间的比值，当比值大于既定值时，才作为最终match。
             */
            DMatch[][] matches = descriptorMatcher.KnnMatch(templateDescriptors, originalDescriptors, 2);
            List<DMatch> goodMatchesList = new List<DMatch>();
            foreach (DMatch[] match in matches)
            {
                DMatch m1 = match[0];
                DMatch m2 = match[1];
                if (m1.Distance <= m2.Distance * nndrRatio)
                {
                    goodMatchesList.Add(m1);
                }
            }
            //当匹配后的特征点大于等于 4 个，则认为模板图在原图中，该值可以自行调整
            if (goodMatchesList.Count >= 4)
            {
                //Console.WriteLine("模板图在原图匹配成功！");
                List<KeyPoint> templateKeyPointList = templateKeyPoints.ToList();
                List<KeyPoint> originalKeyPointList = originalKeyPoints.ToList();
                List<Point2f> objectPoints = new List<Point2f>();
                List<Point2f> scenePoints = new List<Point2f>();
                foreach (DMatch goodMatch in goodMatchesList)
                {
                    objectPoints.Add(templateKeyPointList[goodMatch.QueryIdx].Pt);
                    scenePoints.Add(originalKeyPointList[goodMatch.TrainIdx].Pt);
                }
                MatOfPoint2f objMatOfPoint2f = new MatOfPoint2f();
                foreach (Point2f p in objectPoints)
                {
                    objMatOfPoint2f.Add(p);
                }

                MatOfPoint2f scnMatOfPoint2f = new MatOfPoint2f();
                foreach (Point2f p in scenePoints)
                {
                    scnMatOfPoint2f.Add(p);
                }
                //使用 findHomography 寻找匹配上的关键点的变换
                Mat homography = Cv2.FindHomography(objMatOfPoint2f, scnMatOfPoint2f, OpenCvSharp.HomographyMethod.Ransac, 3);

                /**
                 * 透视变换(Perspective Transformation)是将图片投影到一个新的视平面(Viewing Plane)，也称作投影映射(Projective Mapping)。
                 */
                Mat templateCorners = new Mat(4, 1, MatType.CV_64FC2);
                Mat templateTransformResult = new Mat(4, 1, MatType.CV_64FC2);
                templateCorners.Set<Point2d>(0, 0, new Point2d(0, 0));
                templateCorners.Set<Point2d>(1, 0, new Point2d(templateImage.Cols, 0));
                templateCorners.Set<Point2d>(2, 0, new Point2d(templateImage.Cols, templateImage.Rows));
                templateCorners.Set<Point2d>(3, 0, new Point2d(0, templateImage.Rows));

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

                Cv2.PutText(originalImage, "time:" + DateTime.Now.Subtract(start).TotalMilliseconds + "ms", new Point(10, originalImage.Height - 10), FontFace.HersheySimplex, 0.5, new Scalar(255, 255, 255));
                Cv2.ImWrite(@"C:\Users\Administrator\Desktop\result.jpg", originalImage);
            }
        }
    }
}
