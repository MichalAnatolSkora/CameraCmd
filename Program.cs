using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using OpenCvSharp;
using OpenCvSharp.Extensions;

namespace CameraCmd
{
    class Program
    {
        static void Main(string[] args)
        {
            Mat mat1 = null, mat2 = null, mat3 = null;

            var frame = new Mat();
            var capture = new VideoCapture(0);
            capture.Open(0);

            if (capture.IsOpened())
            {
                while (true)
                {
                    capture.Read(frame);
                    Console.WriteLine("read");

                    mat3 = frame;

                    if (mat1 != null)
                    {
                        //var image = DiffImage(mat3, mat2, mat1).ToBitmap();
                        Mat d1 = new Mat();
                        Cv2.Absdiff(mat3, mat2, d1);

                        var image = mat3.ToBitmap();

                        //image.Save("1.jpg", ImageFormat.Jpeg);
                        //Console.WriteLine("save");

                        var grayImage = new Mat();
                        Cv2.CvtColor(mat3, grayImage, ColorConversionCodes.BGRA2GRAY);
                        Cv2.EqualizeHist(grayImage, grayImage);

                        var cascade = new CascadeClassifier(@".\CascadeClassifiers\haarcascade_frontalface_alt2.xml");
                        var nestedCascade = new CascadeClassifier(@".\CascadeClassifiers\haarcascade_eye_tree_eyeglasses.xml");

                        var faces = cascade.DetectMultiScale(
                            image: grayImage,
                            scaleFactor: 1.1,
                            minNeighbors: 2,
                            flags: HaarDetectionType.DoRoughSearch | HaarDetectionType.ScaleImage,
                            minSize: new Size(30, 30)
                        );

                        Console.WriteLine("Detected faces: {0}", faces.Length);

                        var srcImage = mat3;

                        //new BodyDetector().Detect(mat3);
                        faces.ToList().ForEach(e=>e.Height+=10);
                        foreach (var faceRect in faces)
                        {
                            Cv2.Rectangle(frame, faceRect, Scalar.Red, 2);
                            //var a = new Mat(srcImage, faceRect);
                            //var eigenValues = OutputArray.Create(a);
                            //var eigenVectors = OutputArray.Create(a);
                            //Cv2.Eigen(a, eigenValues, eigenVectors);
                        }

                        Cv2.ImShow("Source", mat3);
                        Cv2.WaitKey(1); // do events

                        //var count = 1;
                        //foreach (var faceRect in faces)
                        //{
                        //    var detectedFaceImage = new Mat(srcImage, faceRect);
                        //    Cv2.ImShow(string.Format("Face {0}", count), detectedFaceImage);
                        //    Cv2.WaitKey(1); // do events

                        //    //var color = Scalar.FromRgb(rnd.Next(0, 255), rnd.Next(0, 255), rnd.Next(0, 255));
                        //    Cv2.Rectangle(srcImage, faceRect, Scalar.Red, 3);

                        //    var detectedFaceGrayImage = new Mat();
                        //    Cv2.CvtColor(detectedFaceImage, detectedFaceGrayImage, ColorConversionCodes.BGRA2GRAY);
                        //    var nestedObjects = nestedCascade.DetectMultiScale(
                        //        image: detectedFaceGrayImage,
                        //        scaleFactor: 1.1,
                        //        minNeighbors: 2,
                        //        flags: HaarDetectionType.DoRoughSearch | HaarDetectionType.ScaleImage,
                        //        minSize: new Size(30, 30)
                        //    );

                        //    Console.WriteLine("Nested Objects[{0}]: {1}", count, nestedObjects.Length);

                        //    foreach (var nestedObject in nestedObjects)
                        //    {
                        //        var center = new Point
                        //        {
                        //            X = (int)(Math.Round(nestedObject.X + nestedObject.Width * 0.5, MidpointRounding.ToEven) + faceRect.Left),
                        //            Y = (int)(Math.Round(nestedObject.Y + nestedObject.Height * 0.5, MidpointRounding.ToEven) + faceRect.Top)
                        //        };
                        //        var radius = Math.Round((nestedObject.Width + nestedObject.Height) * 0.25, MidpointRounding.ToEven);
                        //        Cv2.Circle(srcImage, center, (int)radius, Scalar.Red, thickness: 3);
                        //    }

                        //    count++;
                        //}

                        //Cv2.ImShow("Haar Detection", srcImage);
                        //Cv2.WaitKey(1); // do events
                    }

                    mat1 = mat2;
                    mat2 = mat3;
                }
            }
        }

        static Mat DiffImage(Mat t0, Mat t1, Mat t2)
        {
            Mat d1 = new Mat();
            Cv2.Absdiff(t2, t1, d1);

            Mat d2 = new Mat();
            Cv2.Absdiff(t1, t0, d2);

            Mat diff = new Mat();
            Cv2.BitwiseAnd(d1, d2, diff);

            return diff;
        }
    }
}
