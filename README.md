# WatchList
[![Build status](https://ci.appveyor.com/api/projects/status/go7uy54ipf0yr99k?svg=true)](https://ci.appveyor.com/project/jktoiuhito/watchlist)
[![CodeFactor](https://www.codefactor.io/repository/github/jktoiuhito/watchlist/badge)](https://www.codefactor.io/repository/github/jktoiuhito/watchlist)

WatchList combines listing and selection of series with automatic backlog maintenance under an easy to use command-line interface, making the formalities of proposing and selecting series in eg. movie-nights and anime-circle meetings a breeze. It can also be used for personal series backlog-management.

# Requirements
.NET Core 2.2 (might work on earlier versions)

# How to run
Download the binary from Releases and run with `dotnet WatchList.dll`. A startup helper script is provided for Windows environments.

# How to use
All of the supported commands and their explanations can be viewed anytime with `help`. The program is exited with `exit`.

Series can be added to the list with `add <name>`. Added series can be removed from the list with `rm <name>`. Random series can be selected for watching with `random`, or a specific one can be selected with `watch <name>`.

When watching a series, its total episode count can be set with `episodes <uint>`, and watched episodes count with `watched <uint>`. Watched episode count can be incremented by one with `++`, and decremented by one with `--`. A stream-source can be added with `stream <uri>`, and opened with `stream`. Aliases can be added with `alias <alias>`. Watching of series can be stopped with `finish`.

Aliases can be used instead of the full name in commands `add <name>`, `rm <name>` and `watch <name>`. If the series has a long or hard to remember name, adding aliases to it will definitely make its handling easier.

All series data persist between sessions, but the list is emptied on exit. All stored series can be viewed with `backlog`, and the normal list can be returned to with `list`.

# Completeness
Development is finished, no further updates will be provided.
