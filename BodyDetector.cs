using System;
using System.Collections.Generic;
using System.Text;
using OpenCvSharp;

namespace CameraCmd
{
    public class BodyDetector
    {
        private CascadeClassifier cascade => new CascadeClassifier(@"C:\p\opencv\data\haarcascades\haarcascade_fullbody.xml");
        public void Detect(Mat frame)
        {
            var grayImage = new Mat();
            Cv2.CvtColor(frame, grayImage, ColorConversionCodes.BGRA2GRAY);
            Cv2.EqualizeHist(grayImage, grayImage);
            var bodies = cascade.DetectMultiScale(
                image: frame,
                scaleFactor: 1.1,
                minNeighbors: 2,
                flags: HaarDetectionType.DoRoughSearch | HaarDetectionType.ScaleImage,
                minSize: new Size(30, 30)
            );

            //return bodies;
            Console.WriteLine("Detected bodies: {0}", bodies.Length);
            foreach (var faceRect in bodies)
            {
                Cv2.Rectangle(frame, faceRect, Scalar.Red, 2);
            }
        }
    }
}
