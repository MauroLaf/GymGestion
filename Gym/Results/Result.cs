namespace Gym.Results
{
    public class Result<T> //especifico el generico
    {
        public bool Success { get; set; } //exitosa o no
        public T Data { get; set; } //ese tipo de dato si fue exitoso se pasa como T a Data

        public string ErrorMsg { get; set; }

        // Metodo para verificar exito (estatico dentro de clase) pasare parametro de datos si hay si es false da msj

    public static Result<T> SuccesResult(T data)=>new Result<T>
    { Success=true,Data = data };

    public static Result<T> FailureResult(string errorMsg) => new Result<T>
    { Success = false, ErrorMsg = errorMsg };

    }
}
