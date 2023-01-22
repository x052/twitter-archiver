using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TwitterArchiver
{
    class Program
    {
        public static string BearerToken = "";
        public static string AppName = "";

        static void Main(string[] args)
        {
            // Verify that the required command line parameters were provided
            if (args.Length < 2)
            {
                Console.WriteLine("Please provide a Twitter hashtag or username to search for and the number of tweets to retrieve.");
                return;
            }

            BearerToken = Environment.GetEnvironmentVariable("BEARER_TOKEN");
            AppName = Environment.GetEnvironmentVariable("APP_NAME");

            if (string.IsNullOrEmpty(BearerToken) || string.IsNullOrEmpty(AppName))
            {
                Console.WriteLine("Please set the BEARER_TOKEN and APP_NAME environment variables.");
                return;
            }

            // Assign command line parameters to variables
            string searchTerm = args[0];
            int numberOfTweets = int.Parse(args[1]);

            // Call the function to retrieve tweets from the Twitter API
            var tweets = RetrieveTweets(searchTerm, numberOfTweets).Result;

            // Verify that tweets were retrieved
            if (tweets == null)
            {
                Console.WriteLine("No tweets were retrieved.");
                return;
            }

            // Call the function to archive tweets to a json file
            ArchiveTweets(tweets);
        }

        private static async Task<Tweet[]> RetrieveTweets(string searchTerm, int numberOfTweets)
        {
            try
            {
                // Log the start of the tweet retrieval process
                Console.WriteLine($"Retrieving {numberOfTweets} tweets for {searchTerm}...");

                // Create a new HttpClient to connect to the Twitter API
                using (var client = new HttpClient())
                {
                    // Set the appropriate headers for the Twitter API
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + BearerToken);
                    client.DefaultRequestHeaders.Add("User-Agent", AppName);

                    // Build the URL to query the Twitter API for tweets
                    string url = $"https://api.twitter.com/1.1/search/tweets.json?q={searchTerm}&count={numberOfTweets}";

                    // Send a GET request to the Twitter API and retrieve the response
                    var response = await client.GetAsync(url);

                    // Verify that the request was successful
                    if (!response.IsSuccessStatusCode)
                    {
                        // Log the error and return null
                        Console.WriteLine($"Error retrieving tweets: {response.ReasonPhrase}");
                        return null;
                    }

                    // Read the response content as a string
                    var responseContent = await response.Content.ReadAsStringAsync();

                    Console.WriteLine(responseContent);

                    // Deserialize the response content into a Tweet object
                    var tweets = JsonConvert.DeserializeObject<Response>(responseContent).statuses;

                    // Log the successful retrieval of tweets
                    Console.WriteLine($"Retrieved {tweets.Length} tweets.");

                    // Return the tweets
                    return tweets;
                }
            }
            catch (Exception ex)
            {
                // Log the error and return null
                Console.WriteLine($"Error retrieving tweets: {ex.Message}");
                return null;
            }
        }

        private static void ArchiveTweets(Tweet[] tweets)
        {
            try
            {
                // Log the start of the tweet archiving process
                Console.WriteLine("Archiving tweets to json file...");

                // Create a timestamp for the file name
                string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");

                // Build the file path and name
                string filePath = $"tweets_{timestamp}.json";

                // Serialize the tweets object to json
                string json = JsonConvert.SerializeObject(tweets);

                // Write the json to the file
                File.WriteAllText(filePath, json);

                // Log the successful archiving of tweets
                Console.WriteLine($"Tweets archived to {filePath}.");
            }
            catch (Exception ex)
            {
                // Log the error
                Console.WriteLine($"Error archiving tweets: {ex.Message}");
            }
        }
    }

    public class Response
    {
        public Tweet[] statuses { get; set; }
    }

    // Tweet class to deserialize the json response from the Twitter API
    public class Tweet
    {
        public string text { get; set; }
        public User user { get; set; }
    }

    // User class for the Tweet class
    public class User
    {
        public string name { get; set; }
        public string screen_name { get; set; }
    }

}