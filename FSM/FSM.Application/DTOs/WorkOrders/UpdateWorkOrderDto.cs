using System;
using System.Collections.Generic;
using System.Text;

namespace FSM.Application.DTOs.WorkOrders
{
   public class UpdateWorkOrderDto
    {
        public int Id { get; set; }
        public string Title {  get; set; }
        public string Description { get; set; }
    }
}
