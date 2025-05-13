// Services/TicketService.cs
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using TicketMonitoringSystem.Models;

namespace TicketMonitoringSystem.Services
{
    public class TicketService
    {
        private readonly string _jsonFilePath;
        private static readonly object _fileLock = new object();

        public TicketService()
        {
            _jsonFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "tickets.json");
            EnsureJsonFileExists();
        }

        private void EnsureJsonFileExists()
        {
            var directory = Path.GetDirectoryName(_jsonFilePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            if (!File.Exists(_jsonFilePath))
            {
                var ticketCollection = new TicketCollection();
                string jsonString = JsonSerializer.Serialize(ticketCollection, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_jsonFilePath, jsonString);
            }
        }

        public List<Ticket> GetAllTickets()
        {
            lock (_fileLock)
            {
                if (File.Exists(_jsonFilePath))
                {
                    string jsonString = File.ReadAllText(_jsonFilePath);
                    var ticketCollection = JsonSerializer.Deserialize<TicketCollection>(jsonString);
                    return ticketCollection?.Tickets ?? new List<Ticket>();
                }
                return new List<Ticket>();
            }
        }

        public Ticket GetTicketById(string ticketId)
        {
            var tickets = GetAllTickets();
            return tickets.FirstOrDefault(t => t.TicketId == ticketId);
        }

        public string GenerateTicketId()
        {
            var tickets = GetAllTickets();
            int nextId = tickets.Count + 1;
            string ticketId = $"TKT-{DateTime.Now:yyyyMMdd}-{nextId:D4}";
            return ticketId;
        }

        public void AddTicket(Ticket ticket)
        {
            ticket.TicketId = GenerateTicketId();
            ticket.CreatedAt = DateTime.Now;

            var tickets = GetAllTickets();
            tickets.Add(ticket);
            SaveTickets(tickets);
        }

        public void UpdateTicket(Ticket updatedTicket)
        {
            var tickets = GetAllTickets();
            var existingTicket = tickets.FirstOrDefault(t => t.TicketId == updatedTicket.TicketId);

            if (existingTicket != null)
            {
                existingTicket.Category = updatedTicket.Category;
                existingTicket.OtherCategoryDetails = updatedTicket.OtherCategoryDetails;
                existingTicket.Description = updatedTicket.Description;
                existingTicket.Severity = updatedTicket.Severity;

                SaveTickets(tickets);
            }
        }

        public void DeleteTicket(string ticketId)
        {
            var tickets = GetAllTickets();
            var ticketToRemove = tickets.FirstOrDefault(t => t.TicketId == ticketId);

            if (ticketToRemove != null)
            {
                tickets.Remove(ticketToRemove);
                SaveTickets(tickets);
            }
        }

        private void SaveTickets(List<Ticket> tickets)
        {
            lock (_fileLock)
            {
                var ticketCollection = new TicketCollection { Tickets = tickets };
                string jsonString = JsonSerializer.Serialize(ticketCollection, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_jsonFilePath, jsonString);
            }
        }
    }
}
