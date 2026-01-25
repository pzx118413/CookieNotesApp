# 🍪 CookieNotesApp

A social note-taking application built with ASP.NET Core MVC. Users can write notes, control privacy (Public, Friends Only, Private), and "give cookies" to notes they like.

## Features

- **Authentication:** Secure Login & Registration.
- **Note Management:** Create, Edit, Delete, and Tag notes.
- **Privacy Controls:** Set notes to Private (only you), Friends Only, or Public.
- **Social System:** Send friend requests, accept/decline friends, and view friend profiles.
- **Cookie Economy:** "Give a cookie" to notes you enjoy (limited to 1 cookie per user per note).
- **Dashboard:** A public feed showing notes from the community and your friends.
- **Admin Moderation:** Admin functionality to moderate users and delete content.
## Tech Stack

- **Framework:** ASP.NET Core 8 MVC
- **Database:** SQLite (Entity Framework Core)
- **Frontend:** Razor Views, Bootstrap 5, Bootstrap Icons

## How to Run

1. Clone the repository
2. Update the database
   ```bash
   dotnet ef database update
   ```
3. Start the application:
   ```bash
   dotnet run
   ```
