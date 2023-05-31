using Pro.Common.Enum;

namespace Pro.Common
{
    public class IResult
    {
        RESUTL_API Result { get; set; }
        string Message { get; set; }
    }
    public class APIResult : IResult
    {
        public APIResult()
        {
            Result = RESUTL_API.SUCCESS;
            Message = string.Empty;
        }
        public RESUTL_API Result { get; set; }
        public string Message { get; set; }
    }
}
