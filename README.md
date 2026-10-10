# Gym Management and Booking System

A web app for a chain of gyms: its branches, trainers and services, and the members who book sessions with them.

ASP.NET Core MVC (.NET 8) · Entity Framework Core · PostgreSQL · Bootstrap 5 · jQuery · Ollama

Members sign up, pick a branch, a service and a trainer, and book a time. A booking is checked against the trainer's working hours and existing appointments, so two sessions can never overlap, and it waits for an admin to approve it. Admins manage branches, trainers with their availability, services and all bookings from their own panel; members only see their own.

There is also an exercise page: a member enters their body type, height, weight and goal, and a model running locally through Ollama suggests a workout plan.

Trainers and appointments are also available through a small REST API; appointments can be filtered by trainer, date range and status.

## Running it

You need the .NET 8 SDK and PostgreSQL. The setup scripts check for both (on macOS `setup.sh` installs them with Homebrew if they are missing) and create the database.

```bash
git clone https://github.com/omeraydin00/web-programlama-proje.git
cd web-programlama-proje

./setup.sh          # macOS and Linux
# .\setup.ps1       # Windows PowerShell

dotnet run
```

Then open http://localhost:5001. Set your own database user in `appsettings.json` if it differs from the default. An admin account and sample data are created on the first run by `Data/DbInitializer.cs`. For exercise suggestions, install [Ollama](https://ollama.com) and run `ollama pull llama3.2`.

Made for the Web Programming course at Sakarya University.
