using Microsoft.AspNetCore.Mvc;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using ZXing;
using ZXing.Common;

namespace BarcodeGenerator.Controllers
{
    public class BarcodeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GenerateBarcode()
        {
            string ean13 = GenerateEAN13();
            string barcodeImage = GenerateBarcodeImage(ean13);
            return Json(new { code = ean13, image = barcodeImage });
        }

        [HttpGet]
        public IActionResult DownloadBarcode()
        {
            string ean13 = GenerateEAN13();
            using (Bitmap bitmap = GenerateBarcodeBitmap(ean13))
            using (MemoryStream ms = new MemoryStream())
            {
                bitmap.Save(ms, ImageFormat.Png);
                return File(ms.ToArray(), "image/png", $"barcode_{ean13}.png");
            }
        }

        private string GenerateEAN13()
        {
            string prefix = "2400";
            string dateTime = DateTime.Now.ToString("MMddHHmm");
            string partialCode = prefix + dateTime;
            int checkDigit = CalculateLuhnChecksum(partialCode);
            return partialCode + checkDigit;
        }

        private int CalculateLuhnChecksum(string number)
        {
            int sum = 0;
            bool alternate = true;
            
            for (int i = number.Length - 1; i >= 0; i--)
            {
                int digit = number[i] - '0';
                if (alternate)
                {
                    digit *= 3;
                }
                sum += digit;
                alternate = !alternate;
            }
            int remainder = sum % 10;
            return remainder == 0 ? 0 : 10 - remainder;
        }

        private string GenerateBarcodeImage(string code)
        {
            using (Bitmap bitmap = GenerateBarcodeBitmap(code))
            using (MemoryStream ms = new MemoryStream())
            {
                bitmap.Save(ms, ImageFormat.Png);
                return "data:image/png;base64," + Convert.ToBase64String(ms.ToArray());
            }
        }

        private Bitmap GenerateBarcodeBitmap(string code)
        {
            var barcodeWriter = new BarcodeWriter()
            {
                Format = BarcodeFormat.EAN_13,
                Options = new EncodingOptions
                {
                    Width = 300,
                    Height = 100,
                    Margin = 10
                }
            };
            return barcodeWriter.Write(code);
        }
    }
}
