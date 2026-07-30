using System;
using System.Threading.Tasks;
using AngularApp1.Server.Models.Entities;

namespace AngularApp1.Server.Services
{
    public class OcrService : IOcrService
    {
        public async Task<OcrResult> ProcessImageAsync(string filePath, string declaredReference, decimal declaredAmount)
        {
            await Task.Delay(1500);

            var random = new Random();
            var confidence = (decimal)(random.NextDouble() * (0.99 - 0.90) + 0.90);

            bool hasDiscrepancy = random.Next(1, 101) <= 10;
            decimal detectedAmount = declaredAmount;

            if (hasDiscrepancy)
            {
                decimal variance = declaredAmount * 0.10m;
                detectedAmount = declaredAmount + (random.Next(2) == 0 ? variance : -variance);
            }

            return new OcrResult
            {
                DetectedBank = "Banco Simulado",
                DetectedReference = declaredReference,
                DetectedAmount = Math.Round(detectedAmount, 2),
                DetectedDate = DateTime.Now.Date,
                Confidence = Math.Round(confidence, 4),
                RawText = $"Simulación OCR. Monto: {detectedAmount}. Ref: {declaredReference}",
                ProcessedAt = DateTime.Now
            };
        }
    }
}
