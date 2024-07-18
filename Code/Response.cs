using System.Text;

namespace GenGui_CrystalStar.Code;

public class Response<T>
{


    public Response()
    {

    }

    public Response(T obj)
    {
        this.Data = obj;
    }

    public Response(ResultCode code)
    {
        Success = false;
        this.Code = (int)code;

        switch (code)
        {
            case ResultCode.Okay:
                Success = true;
                break;
            case ResultCode.Invalid:
                Exception = "Invalid";
                break;
            case ResultCode.NotFound:
                Exception = "Not Found";
                break;
            case ResultCode.NullItemInput:
                Exception = "Null Item Input";
                break;
            case ResultCode.Error:
                Exception = "Error";
                break;
            case ResultCode.DataValidationError:
                Exception = "Data Validation Error";
                break;
            case ResultCode.AlreadyExists:
                Exception = "Already Exists";
                break;
            case ResultCode.AccessDenied:
                Exception = "Access Denied";
                break;
            case ResultCode.InvalidOperation:
                Exception = "Invalid Operation";
                break;
            case ResultCode.InvalidData:
                Exception = "Invalid Data";
                break;
            case ResultCode.InvalidArgument:
                Exception = "Invalid Argument";
                break;
            case ResultCode.Timeout:
                Exception = "Timeout";
                break;
            case ResultCode.Warning:
                Exception = "Warning";
                break;
            case ResultCode.Exception:
                Exception = "Exception";
                break;
            case ResultCode.UnhandledException:
                Exception = "Unhandled Exception";
                break;
            case ResultCode.Pending:
                Exception = "Pending";
                break;
            case ResultCode.Failed:
                Exception = "Failed";
                break;
            case ResultCode.DataError:
                Exception = "Data Error";
                break;
            case ResultCode.GeneralError:
                Exception = "General Error";
                break;
            case ResultCode.NotImplemented:
                Exception = "Not Implemented";
                break;
            default:
                Exception = "General Error";
                break;
        }
    }

    public Response(string exception)
    {
        this.SetException(exception);
        this.Code = (int)ResultCode.Error;
    }

    public Response(List<string> exceptions)
    {
        StringBuilder sb = new StringBuilder();

        foreach (var ex in exceptions)
            sb.AppendLine(ex);

        this.SetException(sb.ToString(), ResultCode.Error);
    }

    public Response(string exception, ResultCode code)
    {
        this.SetException(exception, code);
    }

    public Response(List<string> exceptions, ResultCode code)
    {
        StringBuilder sb = new StringBuilder();

        foreach (var ex in exceptions)
            sb.AppendLine(ex);

        this.SetException(sb.ToString(), code);
    }

    public bool Success { get; set; } = true;
    public int Code { get; set; } = 1;
    public string? Exception { get; private set; }
    public T Data { get; set; }

    public void SetException(Exception e)
    {
        Exception = e.Message;
        Success = false;
    }

    public void SetException(string e)
    {
        Exception = e;
        Success = false;
    }

    public void SetException(string e, ResultCode code)
    {
        Exception = e;
        Success = false;
        this.Code = (int)code;
    }
}