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
        private static float nndrRatio = 0.5f;

        public static MatchResult Match(Bitmap src, TrainedTemplate trainedTemplate)
        {
            try
            {
                Mat originalImage = Bitmap2Mat(src);
                DateTime begin = DateTime.Now;
                SURF surf = new SURF();
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
                DMatch[][] matches = descriptorMatcher.KnnMatch(trainedTemplate.templateDescriptors, originalDescriptors, 2);
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
                    List<KeyPoint> templateKeyPointList = trainedTemplate.templateKeyPoints.ToList();
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
                    templateCorners.Set<Point2f>(1, 0, new Point2f(trainedTemplate.templateImage.Cols, 0));
                    templateCorners.Set<Point2f>(2, 0, new Point2f(trainedTemplate.templateImage.Cols, trainedTemplate.templateImage.Rows));
                    templateCorners.Set<Point2f>(3, 0, new Point2f(0, trainedTemplate.templateImage.Rows));

                    //使用 perspectiveTransform 将模板图进行透视变以矫正图象得到标准图片
                    Cv2.PerspectiveTransform(templateCorners, templateTransformResult, homography);

                    //矩形四个顶点
                    Point2f pointA = templateTransformResult.Get<Point2f>(0, 0);
                    Point2f pointB = templateTransformResult.Get<Point2f>(1, 0);
                    Point2f pointC = templateTransformResult.Get<Point2f>(2, 0);
                    Point2f pointD = templateTransformResult.Get<Point2f>(3, 0);

                    MatchResult matchResult = new MatchResult();
                    matchResult.top = (int)pointA.Y;
                    matchResult.left = (int)pointA.X;
                    matchResult.right = (int)pointB.X;
                    matchResult.bottom = (int)pointD.Y;
                    matchResult.time = DateTime.Now.Subtract(begin).TotalMilliseconds;
                    return matchResult;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
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
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
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
