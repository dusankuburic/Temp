namespace Temp.Services.Applications.Exceptions
{
    public class InvalidApplicationException : Exception
    {
        public InvalidApplicationException(string parameterName, object parameterValue)
            : base($"Invalid Employee, " +
                  $"ParameterName : {parameterName}, " +
                  $"ParameterValue: {parameterValue}.") { }
    }

}
