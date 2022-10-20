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

- [ ] API Key is null

#### Explain

- [ ] Generate an API Key
- [ ] Specify User Information
    - [ ] API Key

- [ ] Save
    - [ ] Valid?
        => Show Startup View
    - [ ] Invalid?
        => Show Error [OK]

### Skip

#### Do

- [ ] Single Field "API Key"
- [ ] Button "Next"
    - [ ] Valid?
        => Start with new user Setup
    - [ ] Invalid?
        => Show Error [OK]

<p>ㅤ</p>

## On Startup (Startup View)

### Player choice

#### No current player

=> Start with new user Setup

#### Continue with current player

- [x] Means that a default player has been specified
    - %localappdata%/DATA/Defaults.json
        - [ ] Valid?
            => Track current Activity
        - [ ] Invalid?
            => Start with new user Setup

#### Start with new user Setup

##### New Player View

- [ ] 2 fields:
    - [ ] Specify Username
    - [ ] Specify Tag

- [x] Valid? (SearchPlayerByBungieName w/ count == 1)
    - [x] Save
    - [ ] Prompt User if they want DATA to generate the Activity list
    => Track current Activity
- [ ] May Be Valid? (May not be possible, the endpoint only return duped UserInfo when using different platforms)
    - [ ] Show list of players found
        - [ ] Format `Player#Tag - Platform`
        => Valid?
- [ ] Invalid?
    - [ ] Show error message [OK]

<p>ㅤ</p>

## Track Current Activity

### Activity

- [x] Name
- [ ] Time Elapsed since started
- [ ] Completion count

### Current

- [x] GetCurrentActivity (API) 1/s

<p>ㅤ</p>

## List Activities Completed

### Initialization

- [ ] Manual
or
- [ ] After Setup (see [Startup View](#on-startup-startup-view))

- [ ] GetActivityHistory (API) (Mode: None (0))
- [ ] Property named SelectedActivity


#### Iteration

- [ ] If activity Mode is not in the excluded list

##### Activity not in Dictionary (Hash)

- [ ] Add activity to Dictionary (32 kb each) (In the form of a smaller object with higher limits on values)
- [ ] Initialize CompletionCount to 1
- [ ] Initialize LastCompleted to Period

##### Activity is in Dictionary (Hash)

- [ ] Increment CompletionCount
- [ ] Increment LastCompleted to Period
- [ ] Increment other values to Values

### Display

From left to right:

- [ ] Activity Mode Icon
- [ ] Activity Name
- [ ] Completion Count

### Click

- [ ] Show Activity Details Drop Down (By setting SelectedActivity && IsVisible)

From left to right:

- [ ] Kills
- [ ] Deaths
- [ ] Assists
- [ ] K/D Ratio
- [ ] Score
- [ ] Average Completion Time
- [ ] Last Completed
- [ ] Details (To be Implemented)

<p>ㅤ</p>

## Track Weekly Completion

(To be Implemented)

<p>ㅤ</p>

## Settings

### API Settings

- [x] API Key
- [x] Username
- [x] Tag

### Interface Settings

- [ ] Theme (To be Implemented)
- [ ] Language (To be Implemented)

- [ ] Startup Page (When valid player found) (Only Work if a default character is set)
    - [x] Default Character

<p>ㅤ</p>

- [ ] Activities per pages (see [List Activity](#list-activity))
- [ ] Weekly Completion Tracks per pages (see [Allow Weekly Completion](#allow-weekly-completion))
- [ ] Number of Notification to be stored before deletion (see [Notification](#notification-system))

## Notification System

- When a notification is sent, a message containing the content of the notification is still printed in console
- May be system wide

### Tray

- [ ] Button at the top right of the interface to show recent notifications
- [ ] Display up to 10 notifications (see [Store](#store))
- [ ] Can be dismissed by clicking on the X
- [ ] Clicking on them may result in an action

### Store

- [ ] Store up to 10 notifications (by default) (can be set in settings)
