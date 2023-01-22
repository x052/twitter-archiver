## Introduction

This is a Twitter archiving project that allows you to retrieve tweets based on a specific username and archive them to a json file.

## How to run

1. Set the BEARER_TOKEN and APP_NAME environment variables. The BEARER_TOKEN is the token that authenticates your connection to the Twitter API, and the APP_NAME is the name of your application.

2. Open a terminal window and navigate to the project's root directory.

3. Run the command BEARER_TOKEN="" APP_NAME="" dotnet run elonmusk 10, where "elonmusk" is the username you want to search for and "10" is the number of tweets you want to retrieve.

4. The project will then retrieve the tweets and archive them to a json file in the project's root directory.

Note: Make sure you have installed .NET Core SDK in your system to run the project.
