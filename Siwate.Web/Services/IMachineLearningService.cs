using System.Collections.Generic;
using System.Threading.Tasks;
using Siwate.Web.Models;

namespace Siwate.Web.Services
{
    public class GeminiResult
    {
        public float Score { get; set; }
        public string Feedback { get; set; }
        public bool IsGeneric { get; set; }
        public string FollowUpQuestion { get; set; }
    }

    public interface IMachineLearningService
    {

        // Update: Predict kini menerima durationSeconds dan context sebelumnya
        Task<GeminiResult> PredictAsync(string questionText, string answerText, int durationSeconds);
    }
}
