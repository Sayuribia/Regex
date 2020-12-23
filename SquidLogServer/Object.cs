using System;
using System.Collections.Generic;
using System.Text;

namespace EventBusFuncion
{
    [Serializable]
    public class Object
    {
        public DateTime Time { get; set; }
        public string Duration { get; set; }
        public string IP { get; set; }
        public string ResultCode { get; set; }
        public string Bytes { get; set; }
        public string Methods { get; set; }
        public string URL { get; set; }
        public string User { get; set; }
        public string Type { get; set; }


    }
}
