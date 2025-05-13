# TicketManagementSystem
A basic ticket management application based on C#, .Net MVC core

Features:

Basic Features:
1. Dashboard for active tickets.
2. Ticket addition/modification/deletion.
   
Authorization Features:
1. Cookie-based authentication
2. Redirect to login page for unauthorized access
3. Navigation menu changes based on authentication status
4. Ticket controller actions protected with [Authorize] attribute

Security Enhancements:
1. Passwords are stored with salt and hashed
2. Prevents username and email duplicates
3. Validates user credentials before login
4. Protects all ticket-related action



