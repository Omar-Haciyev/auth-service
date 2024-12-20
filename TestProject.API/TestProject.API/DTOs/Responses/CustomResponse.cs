namespace TestProject.API.DTOs.Responses;

public class CustomResponse<T>
{
    public ResultModel Result { get; set; }=new();
    public T? Data { get; set; }

    public class ResultModel
    {
        public bool Status { get; set; }
        public int Code { get; set; }
        public int Time { get; set; } = 0;
        public bool Error { get; set; }
        public string? ErrorMsg { get; set; }
    }
}