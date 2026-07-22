using System;
using System.Collections.Generic;
using System.Text;

namespace FSM.Application.DTOs.WorkOrders
{
    public class UpdateWorkOrderDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int State { get; set; }
        public int? TechnicianId { get; set; }
        public int CustomerId { get; set; }
        public DateTime? ScheduledStartDate { get; set; }
        public DateTime? ScheduledEndDate { get; set; }
    }
}