using System.Net;

namespace ApPeliculas.Modelos
{
    public class RespuestaApi
    {

        public RespuestaApi()
        {
            ErrorMessages = new List<string>();
        }

        public HttpStatusCode StatusCode { get; set; }  
        public bool IsSuccess { get; set; }
        public List<string> ErrorMessages { get; set;}
        public object Result { get; set; }
    }

}
