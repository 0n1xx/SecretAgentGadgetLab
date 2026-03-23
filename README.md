# SecretAgentGadgetLab

## Overview
SecretAgentGadgetLab is an ASP.NET Core MVC web application developed for COMP 2084 – Server-Side Scripting (ASP.NET).
The application allows secret belorussian users to manage secret agents and their gadgets through a clean and user-friendly interface :)
---

## Features
- View detailed information about each agent and gadget
- Full CRUD operations for:
  - Agents
  - Gadgets
- Authentication:
  - Local user registration and login
  - Google OAuth login
  - GitHub OAuth login
- Authorization:
  - Only authenticated users can create, edit, or delete data
  - Anonymous users can only view data
- Image upload support for gadgets
---

## Technologies Used
- ASP.NET Core MVC
- C#
- Microsoft SQL Server
- Bootstrap
- OAuth (Google & GitHub)
---

## Authentication
This application uses ASP.NET Core Identity for authentication.

Users can:
- Register and log in using email and password
- Sign in using Google
- Sign in using GitHub

Only authenticated users have access to Create, Edit, and Delete operations.
---

## Live Demo
https://secretagentlab.runasp.net
---
