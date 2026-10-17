using System.ComponentModel.DataAnnotations;

namespace TestSSE.Models;

public sealed record SubmitRequestRequest
{
    /// <summary>The message to broadcast to connected clients.</summary>
    [Required, StringLength(500)]
    public required string Message { get; init; }
}
