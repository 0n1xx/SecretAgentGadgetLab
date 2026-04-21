# SecretAgentGadgetLab

## Overview
- SecretAgentGadgetLab is an ASP.NET Core MVC web application developed for COMP 2084 – Server-Side Scripting (ASP.NET).
The application allows secret belorussian users to manage secret agents and their gadgets through a clean and user-friendly interface :)
---

## Features
- Browse gadgets by secret agent
- Full shopping cart with quantity management
- Checkout flow with shipping information
- Stripe payment integration (test mode)
- Order history for customers
- Full CRUD operations for Administrators:
  - Agents
  - Gadgets
  - Orders
- Image upload support via Cloudinary
- Authentication:
  - Local user registration and login
  - Google OAuth login
  - GitHub OAuth login
- Authorization:
  - Only authenticated users can access the shop
  - Only administrators can manage agents, gadgets, and orders

## Technologies Used
- ASP.NET Core MVC
- C#
- Microsoft SQL Server
- Bootstrap
- OAuth (Google & GitHub)

## Authentication
This application uses ASP.NET Core Identity for authentication.

Users can:
- Register and log in using email and password
- Sign in using Google
- Sign in using GitHub
- Only authenticated users have access to Create, Edit, and Delete operations.

## Live Demo

[https://secret-agent-lab.up.railway.app](https://secret-agent-lab.up.railway.app)

---

## Author

Vladislav Sakharov
