using System;
using System.IO;
using System.Linq;
using OpenCvSharp;

namespace CameraTestRPi1
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Trying to capture video");

            // Opens MP4 file (ffmpeg is probably needed)
            using var capture = new VideoCapture(0);

            int sleepTime = (int)Math.Round(1000 / capture.Fps);

            using (var window = new Window("capture"))
            {
                // Frame image buffer
                var image = new Mat();

                // When the movie playback reaches end, Mat.data becomes NULL.
                while (true)
                {
                    capture.Read(image); // same as cvQueryFrame
                    if (image.Empty())
                        break;

                    var grayImage = new Mat();
                    Cv2.CvtColor(image, grayImage, ColorConversionCodes.BGRA2GRAY);
                    Cv2.EqualizeHist(grayImage, grayImage);

                    var cascadeFrontalFace = new CascadeClassifier(@$".{Path.DirectorySeparatorChar}CascadeClassifiers{Path.DirectorySeparatorChar}haarcascade_frontalface_alt2.xml");
                    var cascade = new CascadeClassifier(@$".{Path.DirectorySeparatorChar}CascadeClassifiers{Path.DirectorySeparatorChar}haarcascade_profileface.xml");

                    var facesProfile = cascade.DetectMultiScale(
                        image: image,
                        scaleFactor: 1.1,
                        minNeighbors: 2,
                        flags: HaarDetectionType.DoRoughSearch | HaarDetectionType.ScaleImage,
                        minSize: new Size(30, 30)
                    );

                    var faces = cascadeFrontalFace.DetectMultiScale(
                        image: grayImage,
                        scaleFactor: 1.1,
                        minNeighbors: 2,
                        flags: HaarDetectionType.DoRoughSearch | HaarDetectionType.ScaleImage,
                        minSize: new Size(30, 30)
                    );

                    Console.WriteLine("{0} Detected faces: {1}", DateTime.Now, faces.Length + facesProfile.Length);

                    faces.ToList().ForEach(e => e.Height += 10);
                    facesProfile.ToList().ForEach(e => e.Height += 10);
                    foreach (var faceRect in faces)
                    {
                        Cv2.Rectangle(image, faceRect, Scalar.Red, 2);
                    }

                    foreach (var faceRect in facesProfile)
                    {
                        Cv2.Rectangle(image, faceRect, Scalar.Blue, 2);
                    }

                    window.ShowImage(image);

                    Cv2.WaitKey(sleepTime);
                }
            }
        }
    }
}
