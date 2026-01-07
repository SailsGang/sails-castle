using Microsoft.Extensions.Logging;
using SailsEnergy.Application.Abstractions;
using SailsEnergy.Domain.Common;

namespace SailsEnergy.Infrastructure.Services;

public class AuditService(ILogger<AuditService> logger) : IAuditService
{
    public void Log(AuditEvent auditEvent)
    {
        logger.LogInformation(
            "AUDIT | {Category}.{EventType} | Actor: {ActorId} ({ActorEmail}) | Resource: {ResourceType}:{ResourceId} | Success: {Success} | Details: {Details}",
            auditEvent.Category,
            auditEvent.EventType,
            auditEvent.ActorId,
            auditEvent.ActorEmail,
            auditEvent.ResourceType,
            auditEvent.ResourceId,
            auditEvent.Success,
            auditEvent.Details);
    }
}
