# SecretAgentGadgetLab

Demo e-commerce app built with ASP.NET Core MVC for COMP 2084 (Server-Side Scripting).  
Browse fictional spy-themed gadgets, create an account, check out with Stripe (test mode), and manage catalog data as an admin.

**Live:** [https://secretagentlab.store](https://secretagentlab.store)

---

## Features

- Browse gadgets by secret agent
- Shopping cart with quantity management
- Checkout with shipping info + Stripe payments (test mode)
- Customer order history
- Admin CRUD for agents, gadgets, and orders
- Image uploads via Cloudinary
- Auth:
  - Email / password registration with email confirmation (Resend)
  - Google OAuth
  - GitHub OAuth
- Roles: authenticated customers can shop; administrators manage catalog and orders

---

## Stack

- ASP.NET Core MVC + Identity
- C#
- Microsoft SQL Server
- Bootstrap
- Stripe, Cloudinary, Resend
- Google & GitHub OAuth
- Hosted on Railway (`secretagentlab.store`)

---

## Auth notes

- New local accounts must confirm email before sign-in
- Google / GitHub sign-in is used only for account authentication (email + display name)
- Google Sign-In branding is verified for production

---

## Author

Vladislav Sakharov
