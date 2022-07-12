# NordigenApiTest
This is an ASP.NET MVC project that shows bank transactions from multiple banks using Nordigen transactions API.

It uses Datatables to show transactions in the browser with ability to dowload the data as Excel/CSV/PDF.

The bank transactions are not categorized (a premium version of the Nordigen API can provide this).

## Getting started

You must provide your own secret API keys which you can get by creating a free user from [Nordigen.com](https://nordigen.com).

Visual Studio is recommended to build and test the project.

Edit the file BankWeb/Services/NordigenService.cs to add your own SECRET_ID and SECRET_KEY.

You should be able to run the project from Visual studio. After logging in or using the DEMO account in you should see the transactions.

Note: Using the DEMO account does not require a username or password just click sign in with blank credentials.

![image](https://user-images.githubusercontent.com/59777181/178502171-672f3dcf-5dbc-4591-a4c4-efe2c4c5840b.png)
