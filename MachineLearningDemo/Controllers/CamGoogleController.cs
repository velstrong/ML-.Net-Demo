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
        public dynamic texts { get; set; }
        public dynamic labels { get; set; }
        public dynamic faceAnnotations { get; set; }

        public dynamic searchAnnotations { get; set; }
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
            IReadOnlyList<EntityAnnotation> texts = client.DetectText(image5);
            var textList = new List<FaceAttribute>();
            foreach (EntityAnnotation label in texts)
            {
                textList.Add(new FaceAttribute()
                {
                    Score = ((int)(label.Score * 100)).ToString(),
                    Description = label.Description

                }); ;
            }
            SafeSearchAnnotation searchAnnotations = client.DetectSafeSearch(image5);
            return Json(new FaceResponse() { labels = faceAttributeList, faceAnnotations = faceAnnotations, texts = textList, searchAnnotations = searchAnnotations });
        }

        [HttpPost]
        public IActionResult CaptureImage(IFormFile file)
        {
            if (file == null)
            {
                return Json(new
                {
                    Status = 0
                });
            }
            byte[] imageBytes = null;
            using (var ms = new MemoryStream())
            {
                file.CopyTo(ms);
                imageBytes = ms.ToArray();
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

            IReadOnlyList<EntityAnnotation> texts = client.DetectText(image5);
            var textList = new List<FaceAttribute>();
            foreach (EntityAnnotation label in texts)
            {
                textList.Add(new FaceAttribute()
                {
                    Score = ((int)(label.Score * 100)).ToString(),
                    Description = label.Description

                }); ;
            }
           SafeSearchAnnotation searchAnnotations = client.DetectSafeSearch(image5);
           
            return Json(new FaceResponse() { labels = faceAttributeList, faceAnnotations = faceAnnotations, texts=textList, searchAnnotations=searchAnnotations });
        }
    }
}