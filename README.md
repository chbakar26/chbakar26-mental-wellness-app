# chbakar26-mental-wellness-app

## Mental Wellness Platform (ASP.NET Core MVC + SQL Server)

### Local setup
1. Install .NET SDK 10+ and SQL Server LocalDB.
2. From repository root:
   ```bash
   dotnet restore MentalWellnessApp.slnx
   dotnet ef database update --project /home/runner/work/chbakar26-mental-wellness-app/chbakar26-mental-wellness-app/MentalWellnessApp.Web/MentalWellnessApp.Web.csproj
   dotnet run --project /home/runner/work/chbakar26-mental-wellness-app/chbakar26-mental-wellness-app/MentalWellnessApp.Web/MentalWellnessApp.Web.csproj
   ```

### Seeded users
- Admin: `admin@wellness.local` / `Admin123!`
- Counsellor: `counsellor@wellness.local` / `Counsellor123!`
- Participant: `participant@wellness.local` / `Participant123!`

### Implemented capabilities
- Role-based auth for Visitor (public), Participant, Counsellor, Admin.
- Participant/counsellor registration + login for all authenticated roles.
- Admin user activation and role assignment controls.
- Counsellor CRUD for programs, sessions, and resources.
- Session lifecycle updates (open/completed/archived).
- Participant preferred counsellor, mood check-ins, session requests, and session booking.
- Counsellor request approvals and participant mood/profile visibility.
- Program listing for all users with filters and aggregated mood trend score.
- Admin compliance approval/disapproval for programs/sessions/resources.
- Social sharing links for resources.
