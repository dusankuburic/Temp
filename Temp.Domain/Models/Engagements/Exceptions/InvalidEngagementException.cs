﻿using System;

namespace Temp.Domain.Models.Engagements.Exceptions;

public class InvalidEngagementException : Exception
{
    public InvalidEngagementException(string parameterName, object parameterValue)
        : base($"Invalid Engagement," +
              $"ParameterName : {parameterName}, " +
              $"ParameterValue : {parameterValue}.") {

    }
}
