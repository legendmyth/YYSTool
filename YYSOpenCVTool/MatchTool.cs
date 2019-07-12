using OpenCvSharp;
using OpenCvSharp.CPlusPlus;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace YYSOpenCVTool
{
    public class MatchTool
    {
        private static float nndrRatio = 0.7f;

        public static MatchResult Match(Bitmap src,TrainedTemplate trainedTemplate)
        {
            try
            {
                Mat originalImage = Bitmap2Mat(src);
                DateTime begin = DateTime.Now;
                SURF featureDetector = new SURF();
                SURF descriptorExtractor = new SURF();
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
                DMatch[][] matches = descriptorMatcher.KnnMatch(trainedTemplate.templateDescriptors, originalDescriptors, 2);
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
                int matchesPointCount = goodMatchesList.Count;
                //当匹配后的特征点大于等于 4 个，则认为模板图在原图中，该值可以自行调整
                if (matchesPointCount >= 4)
                {
                    //Console.WriteLine("模板图在原图匹配成功！");
                    List<KeyPoint> templateKeyPointList = trainedTemplate.templateKeyPoints.ToList();
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
                    templateCorners.Set<Point2d>(1, 0, new Point2d(trainedTemplate.templateImage.Cols, 0));
                    templateCorners.Set<Point2d>(2, 0, new Point2d(trainedTemplate.templateImage.Cols, trainedTemplate.templateImage.Rows));
                    templateCorners.Set<Point2d>(3, 0, new Point2d(0, trainedTemplate.templateImage.Rows));

                    //使用 perspectiveTransform 将模板图进行透视变以矫正图象得到标准图片
                    Cv2.PerspectiveTransform(templateCorners, templateTransformResult, homography);

                    //矩形四个顶点
                    Point2d pointA = templateTransformResult.Get<Point2d>(0, 0);
                    Point2d pointB = templateTransformResult.Get<Point2d>(1, 0);
                    Point2d pointC = templateTransformResult.Get<Point2d>(2, 0);
                    Point2d pointD = templateTransformResult.Get<Point2d>(3, 0);

                    //将匹配的图像用用四条线框出来
                    //Cv2.Line(originalImage, pointA, pointB, new Scalar(0, 255, 0), 1);//上 A->B
                    //Cv2.Line(originalImage, pointB, pointC, new Scalar(0, 255, 0), 1);//右 B->C
                    //Cv2.Line(originalImage, pointC, pointD, new Scalar(0, 255, 0), 1);//下 C->D
                    //Cv2.Line(originalImage, pointD, pointA, new Scalar(0, 255, 0), 1);//左 D->A

                    //Cv2.PutText(originalImage, "time:" + DateTime.Now.Subtract(begin).TotalMilliseconds, new OpenCvSharp.CPlusPlus.Point(10, 20), FontFace.HersheySimplex, 0.5, new Scalar(0, 255, 0));
                    MatchResult matchResult = new MatchResult();
                    matchResult.top = (int)pointA.Y;
                    matchResult.left = (int)pointA.X;
                    matchResult.right = (int)pointB.X;
                    matchResult.bottom = (int)pointD.Y;
                    matchResult.time = DateTime.Now.Subtract(begin).TotalMilliseconds;
                    //Cv2.con
                    return matchResult;
                    //matchResult.result=originalImage.tob
                    //Cv2.ImWrite("D:\\Users\\Administrator\\Desktop\\tmp2.jpg", originalImage);
                }
                else
                {
                    return null;
                }
            }
            catch(Exception ex)
            {
                return null;
            }
            
        }
        /// <summary>
        /// bitmap 位图转为mat类型 
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public static Mat Bitmap2Mat(Bitmap bitmap)
        {
            MemoryStream s2_ms = null;
            Mat source = null;
            try
            {
                using (s2_ms = new MemoryStream())
                {
                    bitmap.Save(s2_ms, ImageFormat.Bmp);
                    source = Mat.FromStream(s2_ms,LoadMode.AnyColor);
                }
            }
            catch (Exception e)
            {
                //log.Error(e.ToString());
            }
            finally
            {
                if (s2_ms != null)
                {
                    s2_ms.Close();
                    s2_ms = null;
                }
                GC.Collect();
            }
            return source;
        }
    }

    public class MatchResult
    {
        public int top { get; set; }

        public int bottom { get; set; }

        public int left { get; set; }

        public int right { get; set; }

        public Bitmap result;

        public double time { get; set; }
    }
}
