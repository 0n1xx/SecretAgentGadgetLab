# System Agent Gadget Lab

## Overview:
System Agent Gadget Lab is a ASP.NET Core MVC project which is developed as a part of COMP 2084 – Server-Side Scripting (ASP.NET). This web application is made to manage secret Belorussian 
agent gadgets but it's desgined in a user-friendly way.

## Features:
- Full CRUD operations for:
  - Agents
  - Gadgets
- Authentication:
  - Local user registration and login
  - Google login
  - GitHub login
- Authorization:
  - Only authenticated users can create, edit, or delete data
  - Anonymous users can view data but cannot modify it
- Responsive UI with custom styling

## Authentication:

This application uses ASP.NET Identity for authentication:
- Users can register and log in using email/password
- Also using registration is supported via:
  - Google OAuth
  - GitHub OAuth

## Used Technologies:
- ASP.NET core MVC;
- C#;
- SQL (using a local database);
- Bootstrap and regular CSS.

## Purpose:
- This project demonstrates the use of ASP.NET Core MVC architecture. The authentification will be added in the next stage of this ptoject. 

## Future Improvements

In the next iteration of this project, authentication features will be added to enhance security and user management.

Planned improvements include:

* Implementing **user authentication using ASP.NET Core Identity**
* Adding **Google authentication** for quick sign-in
* Adding **GitHub authentication** to allow users to log in with their GitHub accounts

These features will improve the overall functionality of the application and demonstrate integration with external authentication providers.
