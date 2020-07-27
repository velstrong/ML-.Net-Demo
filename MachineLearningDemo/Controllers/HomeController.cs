using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ML;
using System.Collections.Generic;
using System;
using MachineLearningDemo.Models;
using MachineLearningDemo.Helpers;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace MachineLearningDemo.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public IActionResult Default()
        {
            var viewModel = new TestDataViewModel();

            return View(viewModel);
        }

        public IActionResult Default2()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Predict()
        {
            // TODO: Do prediction here

            return null;
        }

        [HttpPost]
        public IActionResult Train(IFormFile file)
        {
            if (file == null)
            {
                return Json(new
                {
                    Status = 0
                });
            }

            var fileName = file.FileName;
            var fullPath = FileHelper.UploadPath + fileName;

            using (FileStream fs = System.IO.File.Create(fullPath))
            {
                file.CopyTo(fs);
                fs.Flush();
            }

            // TODO: train model here
            
            return Json(new { 
                Status = 1
            });
        }

        [HttpGet]
        public IActionResult RemoveModel()
        {
            System.IO.File.Delete(FileHelper.ModelPath);

            return Json(new
            {
                Status = !System.IO.File.Exists(FileHelper.ModelPath)
            });
        }
    }
}
