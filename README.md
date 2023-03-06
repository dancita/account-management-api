# account-management-api

# Overview

Create a REST API that allows you to create and assign phone numbers to specific accounts. These accounts should also be creatable via the API.

The project should utilise ASP.NET Web API and an ORM of your choice, as well as accessing a fairly simple database in either SQL Server or MySQL.


## Accounts

Each account should have a unique identifier that can be referenced by the phone number, as well as a human-readable description such as a name.

An account should be creatable via the API, but cannot be deleted.

Each account will have two possible statuses: Active or Suspended, and the API should be able to toggle these statuses around.


## Phone Numbers

Each phone number should have a unique reference number that can be called as a RESTful endpoint, a field that contains the phone number (no more than 11 characters),
and should be creatable via the API. It may also be possible to delete the phone number.

Each phone number should be able to be “assigned” to an account, and all phone numbers should be able to be retrieved by specifying an account identifier
as a “Get All” method.

Phone numbers cannot be assigned against an account if the account is suspended.
