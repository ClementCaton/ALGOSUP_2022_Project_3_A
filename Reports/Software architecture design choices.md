# Software architecture design choices
## Architecture tree
![tree](./Files/Design.png)

The architecture of the application is planned to posess three main parts:

<br>

### 

## User's program

The users will compose their own musical pieces by writting some code in F#.
This part will be entirely created by the user.

<br>

## API
This is where the users access the synthesizer library.
The main reason why we need this is to simplify the process the users will have to go trough troughout their work.

### Function
The API allows the user to create, save and modify his sound files, add filters like echo, reverb and flange, combine sound files, convert them into .wav or .mp3 and also play them directly without saving it.

<br>

## The back end
This part of the application encompasses more-or-less every other file within the project.
More importantly, these functions are only accessible to the API and can't be used directly by the user.
The architecture behind this part of the application focuses on extracting raw data from a sound file and modifying it without overwriting the orignial file.
The only times we access actual files is when we read and write sound files. In any other situation we work with raw information.