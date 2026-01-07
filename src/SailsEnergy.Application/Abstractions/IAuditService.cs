using SailsEnergy.Domain.Common;

namespace SailsEnergy.Application.Abstractions;

public interface IAuditService
{
    void Log(AuditEvent auditEvent);
}
