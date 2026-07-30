namespace AngularApp1.Server.Models
{
    public class Result
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
    }
    
    public class Result<T> : Result
    {
        public T? Data { get; set; }
    }
}
