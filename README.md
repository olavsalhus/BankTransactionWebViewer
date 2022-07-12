# NordigenApiTest
This is an ASP.NET MVC project that shows bank transactions from multiple banks using Nordigen transactions API.

It uses Datatables to show transactions in the browser with ability to dowload the data as Excel/CSV/PDF.

The bank transactions are not categorized (a premium version of the Nordigen API can provide this).
## Coding style
I wanted to expermint with minimizing html boilerplate and see what would happen. For compability with all browser I recommend adding <doctype html>, <html>, <head> and <body> (this is left as an exercise to the reader :) )

## Getting started

You must provide your own secret API keys which you can get by creating a free user from [Nordigen.com](https://nordigen.com).

Visual Studio is recommended to build and test the project.

Edit the file BankWeb/Services/NordigenService.cs to add your own SECRET_ID and SECRET_KEY. (using appsettings.json which is best practice is left as an exercise to the reader)

You should be able to run the project from Visual studio. After logging in or using the DEMO account you should see the transactions.

Note: Using the DEMO account does not require a username or password just click sign in with blank credentials.

![image](https://user-images.githubusercontent.com/59777181/178507969-f7d83da2-04e9-47c9-b15c-3441b84165fd.png)
