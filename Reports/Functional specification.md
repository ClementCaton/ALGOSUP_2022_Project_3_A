# Functional specification

## What is it ? 
The synthesizer is used for musical applications. It can create and modify musical notes as well as compose melodies out of them.

In our case, the objective is to create a programmable synthesizer.

## Who is our targeted audience

Any person interested in sound synthesis should be able to use this project.

## Use case :

![Use Case](./Files/UseCase.png)

## What can the user do 

While the API will possess a variety of functions to ease the developpement process. The ones that are used by the users are rather simple:

- API.Read
  - Reads a file (.wav or .mp3 formats)
<br><br>
- API.Write
  - Writes a file (.wav or .mp3 formats)
<br><br>
- API.Play
  - Plays sound data
<br><br>
- API.GetNote
  - Returns the sound data of a given musical note with a given duration, note and octave
<br><br>
- API.Compose
  - Returns complex sound data from a list of notes (music)
<br><br>
- API.Add
  - Superpose notes (with a possible temporal offset) to allow for multiple notes to be played at the same time
<br><br>
- API.ApplyFilterX
  - Applies a filter to a given sound (X replaces with the filter in question)
<br><br>
- API.Preview
  - Gives a visual display of the soundwaves generated using Xplot

