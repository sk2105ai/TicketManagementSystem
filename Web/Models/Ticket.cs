// Models/Ticket.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Reflection.Emit;
using System.Reflection.Metadata;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using static System.Collections.Specialized.BitVector32;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Threading;
using System.Xml.Linq;
using TicketMonitoringSystem.Models;
using TicketMonitoringSystem.Services;



namespace TicketMonitoringSystem.Models
{
    public class Ticket
    {
        
        [Display(Name = "Ticket ID")]
        public string TicketId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Category is required")]
        [Display(Name = "Category")]
        public TicketCategory Category { get; set; }

        [Display(Name = "Other Category Details")]
        [MaxLength(25, ErrorMessage = "Other category details cannot exceed 25 words")]
        public string OtherCategoryDetails { get; set; } = string.Empty;

        [Required(ErrorMessage = "Category is required")]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Category is required")]
        [Display(Name = "Severity")]
        public TicketSeverity Severity { get; set; }

        [Display(Name = "Created At")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss}")]
        public DateTime CreatedAt { get; set; }
    }

    public enum TicketCategory
    {
        Issue,
        ServiceRequest,
        Others
    }

    public enum TicketSeverity
    {
        Normal,
        Minor,
        Major,
        Critical
    }

    public class TicketCollection
    {
        public List<Ticket> Tickets { get; set; } = new List<Ticket>();
    }
}
