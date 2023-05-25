using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PR_MashineLearning_BikeShareServiceApp
{
    public class BikeShareResult
    {

        [JsonProperty("Results")]
        public IList<double> Results { get; set; }
    }
}
