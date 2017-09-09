using System;

namespace PokemonGo.RocketAPI
{
    public class APIVars
    {
        //public long AndroidUnknown25 { get; set; }
        public long IOSUnknown25 { get; set; }
        public string AndroidClientVersion { get; set; }
        public string IOSClientVersion { get; set; }
        public string UnknownPtr8Message { get; set; }
        public string EndPoint { get; set; }
        //public long Unknown27 { get; set; }

        public uint AndroidClientVersionInt { get; set;
            //get { return uint.Parse(AndroidClientVersion.Replace(".", "")); } 
        }

        public APIVars(long uk25, string cv, uint cvInt, string ios_cv, string endPoint, string initialPTR8)//, long uk27)
        {
            //AndroidUnknown25 = uk25;
            IOSUnknown25 = uk25;
            AndroidClientVersion = cv;
            AndroidClientVersionInt = cvInt;
            IOSClientVersion = ios_cv;
            EndPoint = endPoint;
            UnknownPtr8Message = initialPTR8;
            //Unknown27 = uk27;
        }
    }
}
