#Nether


##Player management

The player management functionality in Nether will allow a game to administrate, authenticate and authorize users/players of your game.

###User Stories
* As a player, I can sign up to the game using
	* Username and password
	* Facebook Account (priority 1)
	* Google Account
* As a player I can login to the game using
	* Username and password
	* Facebook Account
	* Google Account
* As a player I can login with limited capabilities if I’m off line
* As a game developer, I can save user preferences per player using JSON documents
* As a game developer I should be able to register friends of a player
* As a game publisher I can manage and edit user preferences of the registered players using a web based interface


###Overall requirements:
* AAA
* BBB

###Required Features/Methods
* AAA
* BBB

##Leaderboard

The leaderboard functionality in Nether will allow a game to submit and retrieve leaderboards in a fast and efficient manner.

###User Stories

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

##Analytics

The analytics functionality in Nether will allow a game to collect, analyze and react to incoming game events.

###User Stories
* As a game developer I can send game events to the server for further analysis (GameStarted, SessionStarted, LevelStarted, LevelFinished, CustomEvent, SessionEnded, GameEnded)
* As a game publisher I can monitor live events and basic KPIs (CCUs, etc.) in an easy to use Web Interface
* As a game publisher I can get batched reports on basic KPIs (retention, active users per day/week/month, average session length, etc.)
* As a game developer I can easily add additional historical, near real time and predictive analytical queries

###Overall requirements:
* AAA
* BBB

###Required Features/Methods
* AAA
* BBB
