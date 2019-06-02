using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SitecoreCognitiveServices.Feature.IntelligentTagging.Classifiers
{
    public interface IClassTrainer
    {
        Tuple<bool, string> GetTrainingData();
        Tuple<bool, string> TrainModel(HttpRequestBase request, string trainingData);
        Tuple<bool, string> GetTrainingStatus();
        Tuple<bool, string> TestClassifier();
    }
}