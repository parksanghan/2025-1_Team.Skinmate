using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp1.Data
{
    public class DiagnosisDataStore
    {
        private DiagnosisDataStore() { }
        public static DiagnosisDataStore Instance { get; } = new();
        public DiagnosisResult? LatestResult { get; private set; }
        public bool IsUpdated  { get; private set; }
        public void Update(DiagnosisClassification classification, DiagnosisRegression regression)
        {
            LatestResult = new DiagnosisResult(classification, regression);
            IsUpdated = true;
        }
        public void Reset() => IsUpdated = false;
    }
}
