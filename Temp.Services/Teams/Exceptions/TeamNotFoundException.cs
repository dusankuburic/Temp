using Temp.Services.Exceptions;

namespace Temp.Services.Teams.Exceptions;

public class TeamNotFoundException : NotFoundException
{
    public TeamNotFoundException()
        : base("Team", "unknown") {
    }

    public TeamNotFoundException(int teamId)
        : base("Team", teamId) {
    }

    public TeamNotFoundException(string message)
        : base(message) {
    }
}