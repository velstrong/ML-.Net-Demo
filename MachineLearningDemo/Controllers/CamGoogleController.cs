using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Google.Cloud.Vision.V1;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace MachineLearningDemo.Controllers
{
    public class FaceAttribute
    {
        public string Description { get; set; }
        public string Score { get; set; }
    }
    public class FaceResponse
    {
        public dynamic labels { get; set; }
        public dynamic faceAnnotations { get; set; }
    }

    [Route("/{Controller}/{Action}")]
    public class CamGoogleController : Controller
    {
        // GET: CamGoogle
        [HttpGet]
        public ActionResult Capture()
        {
            string value = Environment.GetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS");
            return View();
        }

        [HttpPost]
        public ActionResult Capture(string base64String)
        {
            byte[] imageBytes = null;
            //string base64String = string.Empty;
            //if (formFile.Length > 0)
            //{
            //    using (var ms = new MemoryStream())
            //    {
            //        formFile.CopyTo(ms);
            //        var fileBytes = ms.ToArray();
            //        base64String = Convert.ToBase64String(fileBytes);
            //        // act on the Base64 data
            //    }
            //}
            
            if (!string.IsNullOrEmpty(base64String))
            {
                var imageParts = base64String.Split(',').ToList<string>();
                imageBytes = Convert.FromBase64String(imageParts[1]);
            }


            Image image5 = Image.FromBytes(imageBytes);
            ImageAnnotatorClient client = ImageAnnotatorClient.Create();
            IReadOnlyList<EntityAnnotation> labels = client.DetectLabels(image5);
            client.DetectFaces(image5);
            var faceAttributeList = new List<FaceAttribute>();
            foreach (EntityAnnotation label in labels)
            {
                faceAttributeList.Add(new FaceAttribute()
                {
                    Score = ((int)(label.Score * 100)).ToString(),
                    Description = label.Description

                }); ;
                // Console.WriteLine($"Score: {(int)(label.Score * 100)}%; Description: {label.Description}");
            }
            IReadOnlyList<FaceAnnotation> faceAnnotations = client.DetectFaces(image5);

            //var faceAttributeList = new List<FaceAttribute>();
            //foreach (FaceAnnotation label in faceAnnotations)
            //{
            //faceAttributeList.Add(new FaceAttribute()
            //{
            //    Score = ((int)(label. * 100)).ToString(),
            //    Description = label.

            //}); ;
            // Console.WriteLine($"Score: {(int)(label.Score * 100)}%; Description: {label.Description}");
            //}
            //using (var client = new WebClient())
            //{
            //    Mainrequests Mainrequests = new Mainrequests()
            //    {

            //        requests = new List<requests>()
            //            {
            //            new requests()
            //            {
            //            image = new image()
            //            {
            //            content = imageParts[1]
            //        },

            //        features = new List<features>()
            //        {
            //            //FACE_DETECTION
            //            //#############################//
            //            new features()
            //            {
            //                type = "FACE_DETECTION",
            //            }

            //            //LABEL_DETECTION
            //            //#############################//
            //            //new features()
            //            //{
            //            //    type = "LABEL_DETECTION",
            //            //}
            //        }

            //    }

            //    }

            //    };

            //    client.Headers.Add("Content-Type:application/json");
            //    client.Headers.Add("Accept:application/json");
            //    var response = client.UploadString("https://vision.googleapis.com/v1/images:annotate?key=" + "AIzaSyBUPN7TQQUqYcTi6HI4zpqXJkaWm1AG-ic", JsonConvert.SerializeObject(Mainrequests));

            //    return Json(data: response);
            //}
            return Json(new FaceResponse() { labels = faceAttributeList, faceAnnotations = faceAnnotations });
        }

        [HttpPost]
        public ActionResult CaptureImage(string base64String)
        {
            byte[] imageBytes = null;
            if (!string.IsNullOrEmpty(base64String))
            {
                var imageParts = base64String.Split(',').ToList<string>();
                imageBytes = Convert.FromBase64String(imageParts[1]);
            }

            Image image5 = Image.FromBytes(imageBytes);
            ImageAnnotatorClient client = ImageAnnotatorClient.Create();
            IReadOnlyList<EntityAnnotation> labels = client.DetectLabels(image5);
            client.DetectFaces(image5);
            var faceAttributeList = new List<FaceAttribute>();
            foreach (EntityAnnotation label in labels)
            {
                faceAttributeList.Add(new FaceAttribute()
                {
                    Score = ((int)(label.Score * 100)).ToString(),
                    Description = label.Description

                }); ;
                // Console.WriteLine($"Score: {(int)(label.Score * 100)}%; Description: {label.Description}");
            }
            IReadOnlyList<FaceAnnotation> faceAnnotations = client.DetectFaces(image5);

            //var faceAttributeList = new List<FaceAttribute>();
            //foreach (FaceAnnotation label in faceAnnotations)
            //{
            //faceAttributeList.Add(new FaceAttribute()
            //{
            //    Score = ((int)(label. * 100)).ToString(),
            //    Description = label.

            //}); ;
            // Console.WriteLine($"Score: {(int)(label.Score * 100)}%; Description: {label.Description}");
            //}
            //using (var client = new WebClient())
            //{
            //    Mainrequests Mainrequests = new Mainrequests()
            //    {

            //        requests = new List<requests>()
            //            {
            //            new requests()
            //            {
            //            image = new image()
            //            {
            //            content = imageParts[1]
            //        },

            //        features = new List<features>()
            //        {
            //            //FACE_DETECTION
            //            //#############################//
            //            new features()
            //            {
            //                type = "FACE_DETECTION",
            //            }

            //            //LABEL_DETECTION
            //            //#############################//
            //            //new features()
            //            //{
            //            //    type = "LABEL_DETECTION",
            //            //}
            //        }

            //    }

            //    }

            //    };

            //    client.Headers.Add("Content-Type:application/json");
            //    client.Headers.Add("Accept:application/json");
            //    var response = client.UploadString("https://vision.googleapis.com/v1/images:annotate?key=" + "AIzaSyBUPN7TQQUqYcTi6HI4zpqXJkaWm1AG-ic", JsonConvert.SerializeObject(Mainrequests));

            //    return Json(data: response);
            //}
            return Json(new FaceResponse() { labels = faceAttributeList, faceAnnotations = faceAnnotations });
        }
    }
}