# Server-Sent Events (SSE) – Proof of Concept

This project demonstrates a lightweight implementation of **Server-Sent Events (SSE)** using ASP.NET Core. It includes:

- A server-side endpoint that streams real-time messages to clients via SSE.
- A static HTML page hosted within the same project to consume and visualize these messages.
- An API endpoint to simulate server-side event publishing.

---

## Purpose

The goal of this POC is to validate the use of SSE as a one-way real-time communication mechanism suitable for lightweight client notifications, such as:

- System alerts
- Live feeds
- Monitoring events
