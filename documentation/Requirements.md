#Nether


##Leaderboard

The leaderboard functionality in Nether will allow a game to submit and retrieve leaderboards in a fast and efficient manner.

###Overall requirements:
* Leaderboard should support thousands of simultaneous players, reporting scores and retrieving leaderboards on a close to 1:1 ratio.
* Data store should support millions of historically reported scores with no or little degradation of speed (as long as scaled properly, see below)
* Should scale close to linearly by adding more resources
* Should only allow authenticated requests. i.e. there is no fun with a leaderboard that is anonymous
* Should provide the ability to plug in server side logic to validate the reported score i.e. logic that cleans up, validates and checks the probability of cheating.

###Required Features/Methods
* Report new score (could be high score, but not necessary)
	* Should be an "immediate" operation no matter the number of stored scores in the system, i.e. once the API call to store a score has completed, operations to retrieve leaderboards should reflect this score.
	* Should report score to Analytics Leaderboard for further analysis
	* Returns a Leaderboard Consistency Token/Number
* Retrieve leaderboard
	* Types
		* Top X
		* X around "me"
		* For my Facebook Friends
	* Partitioned by (Nothing or a combination of Date, Location and/or Custom Tag)
		* Date
			* Actual
				* Year
				* Month
				* Week
				* Day
				* Hour
			* Sliding Timespan
				* Last 365 days
				* Last 30 days
				* Last 7 days
				* Last 24 hours
				* Last 60 minutes
		* Location
			* Country
			* City
			* Custom Area
		* Custom Tag
	* Result
		* Support paging
		* Should include
			* Score
			* Rank - Should be relative to the used combination of partitioning
			* Gamer Tag
			* DateTime Achieved
		* Should be sorted by Score/Rank
		* Should be consistent up to a specific point requested by using a specified Leaderboard Consistency Token/Number
