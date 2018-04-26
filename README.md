# AlphaWarehouseManagementSystemPublic

AlphaWMS is an application that allows users to manage users, roles, and reports.  It was built on the .NET framework and uses the MVC design pattern.  This repo contains some of the code used on the site.  If you'd like to visit the site yourself, feel free to click the following link https://AlphaWMS.com.  Finally, if you have any questions, send me an email to my mcguirepjames@gmail.com

## Technologies Used 

#### Front End

- React/Redux
- While the entire site is not built React/Redux.  Some pages use it.  
- Bootstrap
- JQuery
- JavaScript
#### Back End

- ASP.NET MVC 5
- Identity/OAuth 2
    - Used for handling logins and password resets
- Hangfire
    - Allows user to schedule jobs that run and email reports
- SendGrid
    - Handles outgoing emails used for resetting passwords, emailing reports, etc.
- SQL Server
- Azure
    - Image Storage/DB/SSL Cert



