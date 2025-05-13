# TicketManagementSystem
A basic ticket management application based on C#, .Net MVC core

Features:

Authorization Features:
Cookie-based authentication
Redirect to login page for unauthorized access
Navigation menu changes based on authentication status
Ticket controller actions protected with [Authorize] attribute

Security Enhancements:
Passwords are stored with salt and hashed
Prevents username and email duplicates
Validates user credentials before login
Protects all ticket-related action

Basic Features:

Ticket addition/Modification/Deletion
Dashboard for active tickets.

