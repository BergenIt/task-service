
using System.ComponentModel.DataAnnotations;

namespace TaskService.Core.Models;

public abstract class BaseEntity
{
    [Key]
    public string Id { get; set; } = string.Empty;
}
