using System.Collections.Generic;

namespace Backend.Payloads
{
    public class UploadImgResult
    {
        public List<string> paths { get; set; }
        public List<string> errors { get; set; }
    }
}
