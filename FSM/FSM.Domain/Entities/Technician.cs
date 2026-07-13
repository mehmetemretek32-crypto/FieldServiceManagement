using FSM.Domain.Common;
using System.Collections.Generic;
namespace FSM.Domain.Entities;

public class Technician : BaseEntity, ISoftDeletable
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public bool IsAvailable { get; set; } = true; // Teknisyen o an boşta mı?
    public bool IsDeleted { get; set; } = false;

    public ICollection<TechnicianInventory> InventoryItems { get; set; } = new List<TechnicianInventory>();
}


