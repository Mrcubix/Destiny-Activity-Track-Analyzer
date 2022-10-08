# Destiny Activity Track Analyzer

- [Track Current Activity](#track-current-activity)
- [List Activities Completed](#list-activities-completed)
- [Track Weekly Completion (To be implemented)](#track-weekly-completion)
- [Settings](#settings)
- ?

<p>ㅤ</p>

## First Launch

### Tutorial ?

#### When

- User infos are null
or
- API Key is null

#### Explain

- Generate an API Key
- Specify User Information
    - API Key

- Save
    - Valid?
        => Show Startup View
    - Invalid?
        => Show Error [OK]

### Skip

#### Do

- Single Field "API Key"
- Button "Next"
    - Valid?
        => Start with new player Setup
    - Invalid?
        => Show Error [OK]

<p>ㅤ</p>

## On Startup (Startup View)

### Player choice

#### No current player

=> Start with new player Setup

#### Continue with current player

- Means that a player player has been specified
    - %localappdata%/DATA/current.json
        - Valid?
            => Track current Activity
        - Invalid?
            => Start with new player Setup

#### Start with new player Setup

##### New Player View

- 2 fields:
    - Specify Username
    - Specify Tag

- Valid? (SearchPlayerByBungieName w/ count == 1)
    - Save
    - Prompt User if they want DATA to generate the Activity list
    => Track current Activity
- May Be Valid?
    - Show list of players found
        - Format `Player#Tag - Platform`
        => Valid?
- Invalid?
    - Show error message [OK]

<p>ㅤ</p>

## Track Current Activity

### Activity

- Name
- Time Elasped since started
- Completion count

### Current

- GetCurrentActivity (API) 1/s

<p>ㅤ</p>

## List Activities Completed

### Initialization

- Manual
or
- After Setup (see [Startup View](#on-startup-startup-view))

- GetActivityHistory (API) (Mode: None (0))
- Property named SelectedActivity


#### Iteration

- if activity Mode is not in the excluded list

##### Activity not in Dictionary (Hash)

- add activity to Dictionary (32 kb each) (In the form of a smaller object with higher limits on values)
- Initialize CompletionCount to 1
- Initialize LastCompleted to Period

##### Activity is in Dictionary (Hash)

- Increment CompletionCount
- Increment LastCompleted to Period
- Increment other values to Values

### Display

From left to right:

- Activity Mode Icon
- Activity Name
- Completion Count

### Click

- Show Activity Details Drop Down (By setting SelectedActivity && IsVisible)

From left to right:

- Kills
- Deaths
- Assists
- K/D Ratio
- Score
- Average Completion Time
- Last Completed
- Details (To be Implemented)

<p>ㅤ</p>

## Track Weekly Completion

(To be Implemented)

<p>ㅤ</p>

## Settings

### API Settings

- API Key
- Username
- Tag

### Interface Settings

- Theme (To be Implemented)
- Language (To be Implemented)

- Startup Page (When valid player found) (Only Work if a default character is set)
    - Default Character

- Activities per pages (see [List Activity](#list-activity))
- Weekly Completion Tracks per pages (see [Allow Weekly Completion](#allow-weekly-completion))

