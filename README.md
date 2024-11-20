The Ladybird Foundation Website

This is a responsive and dynamic website for The Ladybird Foundation, built using ASP.NET MVC in Visual Studio, styled with Bootstrap, and powered by Firebase for backend services. The website provides users with information about the organization, including banking details, events, team members, and an admin dashboard for managing content dynamically.

The website is hosted at: theladybirdfoundation.co.za.

Features

User Features

	•	Banking Details: View banking information for making donations.
	•	Events: Browse upcoming and past events with details and locations.
	•	About Us: Learn more about the organization’s mission, vision, and story.
	•	Meet the Team: Explore profiles of team members.

Admin Dashboard Features

	•	Event Management: Create, edit, and delete events.
	•	Team Management: Add, update, or remove team member profiles.
	•	About Us Management: Dynamically update the “About Us” section.
	•	Banking Details Update: Manage banking details for donations.

Technology Stack

Frontend

	•	HTML5, CSS3, Bootstrap: For responsive and modern design.
	•	Razor Views: Dynamic server-side rendering.

Backend

	•	ASP.NET MVC: Handles business logic and user requests.
	•	Firebase: Provides real-time database and hosting.

Database

	•	Firebase Realtime Database: Stores events, team member details, and “About Us” content.

Hosting

	•	Domain: Hosted on theladybirdfoundation.co.za.
	•	absolute hosting: Ensures fast and secure delivery of web assets.

Installation

	1.	Clone the Repository

git clone https://github.com/Group-6-WIL/Website  


	2.	Open in Visual Studio
	•	Open the .sln file in Visual Studio.
	3.	Configure Firebase
	•	Set up a Firebase project at Firebase Console.
	•	Add the google-services.json file (or Firebase configuration keys) to your project.
	•	Update the Firebase credentials in your application:

{  
  "apiKey": "your-api-key",  
  "authDomain": "your-auth-domain",  
  "databaseURL": "your-database-url",  
  "projectId": "your-project-id",  
  "storageBucket": "your-storage-bucket",  
  "messagingSenderId": "your-sender-id",  
  "appId": "your-app-id"  
}  


	4.	Run the Application
	•	Press F5 or run the project in Visual Studio.
	•	Open the application in your browser (usually at http://localhost:5000).

Folder Structure

LadybirdFoundation/  
├── Controllers/       # Handles user requests and business logic  
├── Models/            # Firebase integration and data structures  
├── Views/             # Razor views (HTML templates)  
├── wwwroot/           # Static assets (CSS, JavaScript, images)  
├── appsettings.json   # Configuration (Firebase credentials, etc.)  
└── README.md          # Project documentation  

Admin Dashboard Overview

	•	Event Management: Create, update, and delete event information stored in Firebase.
	•	Team Management: Add and manage team member profiles dynamically.
	•	About Us Management: Update the “About Us” content directly from the admin interface.

Hosting and Deployment

	•	The website is hosted at: theladybirdfoundation.co.za.
	•	Deployment is handled via absolute hosting 


Future Enhancements

	•	Add user authentication for admin dashboard access.
	•	Include a donation tracker with progress indicators.
	•	Implement email notifications for new events.
	•	Integrate Google Maps for event locations.

License

This project is licensed under the MIT License.
