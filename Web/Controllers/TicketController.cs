// Controllers/TicketController.cs
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using TicketMonitoringSystem.Models;
using TicketMonitoringSystem.Services;
using Microsoft.AspNetCore.Authorization;


namespace TicketMonitoringSystem.Controllers
{
    [Authorize] // Require authentication for all actions added by Shahir Khan on May 12, 2025
    public class TicketController : Controller
    {
        private readonly TicketService _ticketService;

        public TicketController(TicketService ticketService)
        {
            _ticketService = ticketService;
        }


        /// <summary>
        /// Dashboard view
        /// </summary>
        /// <returns>IActionResult</returns>
        /// <CreatedBy>Shahir Khan</CreatedBy>
        /// <CreatedDate>May 11, 2025</CreatedDate>
        public IActionResult Dashboard()
        {
            var tickets = _ticketService.GetAllTickets();
            return View(tickets);
        }

        
        /// <summary>
        /// Create ticket view
        /// </summary>
        /// <returns></returns>
        /// <CreatedBy>Shahir Khan</CreatedBy>
        /// <CreatedDate>May 11, 2025</CreatedDate>
        public IActionResult Create()
        {
            // Initialize a new ticket with default values to avoid null reference issues
            var model = new Ticket
            {
                TicketId = string.Empty,
                OtherCategoryDetails = string.Empty
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Ticket ticket)
        {

            // Clear any existing ModelState errors for TicketId as it's auto-generated
            ModelState.Remove("TicketId");

            // Validate other category details if category is Others
            if (ticket.Category == TicketCategory.Others)
            {
                if (string.IsNullOrWhiteSpace(ticket.OtherCategoryDetails))
                {
                    ModelState.AddModelError("OtherCategoryDetails", "Please provide details for Other category.");
                }
                else
                {
                    // Validate word count (max 25 words)
                    int wordCount = ticket.OtherCategoryDetails.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Length;
                    if (wordCount > 25)
                    {
                        ModelState.AddModelError("OtherCategoryDetails", "Other category details cannot exceed 25 words.");
                    }
                }
            }
            else
            {
                // If Category is not Others, ignore OtherCategoryDetails validation
                ModelState.Remove("OtherCategoryDetails");
                ticket.OtherCategoryDetails = string.Empty;
            }

            if (ModelState.IsValid)
            {
                _ticketService.AddTicket(ticket);
                TempData["SuccessMessage"] = "Ticket created successfully!";
                return RedirectToAction(nameof(Dashboard));
            }
            return View(ticket);
        }

        // Edit ticket view
        /// <summary>
        /// Edit Ticket View
        /// </summary>
        /// <param name="id">string</param>
        /// <returns>IActionResult</returns>
        /// <CreatedBy>Shahir Khan</CreatedBy>
        /// <CreatedDate>May 11, 2025</CreatedDate>
        /// <ModifiedBy>Shahir Khan</ModifiedBy>
        /// <ModifiedDate>May 12,2025</ModifiedDate>
        /// <remarks>Added explicit authorization (though inherited from class-level attribute)</remarks>
        [Authorize] 
        public IActionResult Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var ticket = _ticketService.GetTicketById(id);
            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        /// <ModifiedBy>Shahir Khan</ModifiedBy>
        /// <ModifiedDate>May 12,2025</ModifiedDate>
        /// <remarks>Added explicit authorization (though inherited from class-level attribute)</remarks>
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Ticket ticket)
        {
            // Clear any existing ModelState errors for TicketId as it's passed in hidden field
            ModelState.Remove("TicketId");

            // Validate other category details if category is Others
            if (ticket.Category == TicketCategory.Others)
            {
                if (string.IsNullOrWhiteSpace(ticket.OtherCategoryDetails))
                {
                    ModelState.AddModelError("OtherCategoryDetails", "Please provide details for Other category.");
                }
                else
                {
                    // Validate word count (max 25 words)
                    int wordCount = ticket.OtherCategoryDetails.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Length;
                    if (wordCount > 25)
                    {
                        ModelState.AddModelError("OtherCategoryDetails", "Other category details cannot exceed 25 words.");
                    }
                }
            }
            else
            {
                // If Category is not Others, ignore OtherCategoryDetails validation
                ModelState.Remove("OtherCategoryDetails");
                ticket.OtherCategoryDetails = string.Empty;
            }

            if (ModelState.IsValid)
            {
                _ticketService.UpdateTicket(ticket);
                TempData["SuccessMessage"] = "Ticket updated successfully!";
                return RedirectToAction(nameof(Dashboard));
            }
            return View(ticket);
        }


        /// <summary>
        /// Delete ticket confirmation
        /// </summary>
        /// <param name="id">string</param>
        /// <returns>IActionResult</returns>
        /// <CreatedBy>Shahir Khan</CreatedBy>
        /// <CreatedDate>May 11, 2025</CreatedDate>
        /// <ModifiedBy>Shahir Khan</ModifiedBy>
        /// <ModifiedDate>May 12,2025</ModifiedDate>
        /// <remarks>Added explicit authorization (though inherited from class-level attribute)</remarks>
        [Authorize]
        public IActionResult Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var ticket = _ticketService.GetTicketById(id);
            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(string id)
        {
            _ticketService.DeleteTicket(id);
            TempData["SuccessMessage"] = "Ticket deleted successfully!";
            return RedirectToAction(nameof(Dashboard));
        }


        /// <summary>
        /// Details view
        /// </summary>
        /// <param name="id">string</param>
        /// <returns>IActionResult</returns>
        /// <CreatedBy>Shahir Khan</CreatedBy>
        /// <CreatedDate>May 11, 2025</CreatedDate>
        public IActionResult Details(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var ticket = _ticketService.GetTicketById(id);
            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }
    }
}
