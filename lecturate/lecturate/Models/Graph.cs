using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace lecturate.Models
{
    public class Graph
    {
        public String State { get; set; }
        public Freq freq { get; set; }

    }

    public class Freq
    {
        public int LecturerReadine { get; set; }
        public int LecturerTransferRate { get; set; }
        public int LecturerAttitude { get; set; }
        public int LecturerKnowledge { get; set; }
    }
}