using System.Collections.Generic;
using System.Threading.Tasks;
using Siwate.Web.Models;

namespace Siwate.Web.Services
{
    public interface IMachineLearningService
    {

        // Update: Predict sekarang butuh QuestionText dan bersifat Async
        Task<(float Score, string Feedback)> PredictAsync(string questionText, string answerText);
    }
}
