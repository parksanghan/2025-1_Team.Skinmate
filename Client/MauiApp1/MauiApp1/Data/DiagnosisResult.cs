using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp1.Data
{
    public class DiagnosisResult
    {
        public DiagnosisClassification? Classification {  get; set; }  
        public DiagnosisRegression? Regression { get; set; }    

        public DiagnosisResult(DiagnosisClassification? classification, DiagnosisRegression? regression )
        {
            Classification = classification;
            Regression= regression;
        }
    }
}
