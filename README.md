# ALGOSUP_2022_Project_3_A | Sound Synthesizer

## **Project:**

The project given by [*Algosup*](https://www.algosup.com/fr/index.html) and [*Robert Pickering*](https://github.com/robertpi) was to create a Sound Synthesizer able to open, modify, create and save sounds.

## **Getting Started**

## Prerequisites

- Download .Net 6.0 or newer

## .Net CLI

> dotnet add package Synthesizer.Json

## windows

> Install-Package Synthesizer.Json

## **Project members**

[*Ivan Molnar*](https://github.com/ivan-molnar) <br>
[*Clement Caton*](https://github.com/ClementCaton) <br>
[*Louis de Choulot*](https://github.com/Louis-de-Lavenne-de-Choulot) <br>
[*Théo Diancourt*](https://github.com/TheoDct) <br>
[*Mathieu Chaput*](https://github.com/Chaput-Mathieu) <br>
[*Léo Chartier*](https://github.com/leo-chartier)



# Project documentation

## NuGet install
<!-- KFC goes here -->

## Basic structure

To interract with the library you'll have to mainly interract with two objects.
The ``Synth`` object which is the actual sound synthesizer and the ``Filter`` object which contains a list of function that will allow you to modify the created sounds.

## Reading files

### Reading wav files



### Reading mp3 files
<span style="color: red;">WIP</span>

## Writing to files / Saving

### Writing to wav files
### Writing to mp3 files
<span style="color: red;">WIP</span>

## Dealing with stereo

## Creating basic audio data

The library supports the creation

### Creating audio data with an envelope
### Creating audio data with a custom envelope

## Finding frequencies from notes and octaves

A more simplified way to find the sound you are looking for is trought musical octaves[^1] and notes[^2].
To call on this form of notation you'll have to use the ``Synth.getNoteFreq (octav:int) (note:Note)`` function to get the right frequency.

Example: 
```fs
let Note = Synth.GetNoteFreq Note.C 4 // This returns the frequency of the C4 note
```

Alternatively, you could directly create a SinWave using the ``Synth.note (duration:Duration) (note:Note) (octav:int)``.

Example:
```fs
let Note = Synth.Note Half Note.C 4 // This returns the frequency a half duration of the C4 note
```

### Finding notes with a custom default frequency

In most cases, the frequency of a note is calculated from a default frequency (mostly, 440Hz for the A4 note).
However, in some cases, you might need to find a note from a different starting frequency.
This can be done using the ``Synth.getNoteFreqOffset (octav:int) (note:Note) (aFourFreq:Float)``

Example:
```fs
let Note = Synth.GetNoteFreqOffset Note.C 4 444. // This returns the frequency of the C4 note calculated from the starting point 444Hz at the A4 note
```

## Creating silence

Creating silence is as simple as calling the ``Synth.silence (duration:Duration)`` function.
```fs
let Silence = Synth.Silence (Seconds 2) // Returns 2 seconds of silence
```

## Additioning audio data
### Additioning audio with a predefined ratio
## Composing

One thing you have to be aware of is the ``cutCorners`` function.
When we first created the compose function we encounterd a strange, small sound between easch end every note.
This sound was caused by the notes ending on a not-zero amplitude.

The solution was to add in a filter that gradually lowers the amplitude of the notes start and end to 0.

<!-- insert before and after image here -->

Therefore; the``Synth.compose (sounds:List<float>)`` function has a default cutCorner value of 100 (this means it cuts away from the first and last 100 bytes from each note).

Example:
```fs
let C4 = Synth.Note Half Note.C 4   // init
let D4 = Synth.Note Half Note.D 4   //
let Silence = Synth.Nilence Quarter //
let B5 = Synth.Note Half Note.B 5   //

let Music = Synth.Compose [          // Returns a single, large sound composed of the smaller sounds given to it
    C4;
    C4;
    D4;
    Silence;
    B5;
]
```

In certain cases, one might need to set a custom value to the cutCorner function.
This can be done with the ``Synth.ComposeCutCorner (Corner:int) (Sounds:List<float>)``

```fs
let Music = Synth.ComposeCutCorner 1000 [
    C4;
    C4;
    D4;
    Silence;
    B5;
]
```

Alternatively, one might want to compose without the cutCorners filter.
This can be done either by giving it a 0 value or by using the ``Synth.composeNoCutCorner (corner:int) (sounds:List<float>)`` function.

With zero value:
```fs
let Music = Synth.ComposeCutCorner 0 [
    C4;
    C4;
    D4;
    Silence;
    B5;
]
```

Or with ``ComposeNoCutCorner``:
```fs
let Music = Synth.ComposeNoCutCorner [
    C4;
    C4;
    D4;
    Silence;
    B5;
]
```
These two are equivalents.

## Preview
## Frequency analysis


## Filters
### Apply multiple filters at once

### Cutting audio
### Changing amplitude

### Reverb, Echo and chorus
### Flanger

### Envelope
### Custom envelope

### Low frequency oscillation
#### AM
#### FM

### LowPass / HighPass / BandPass / RejectBand filters


# Footnotes

Info on [**.mp3 files**](https://github.com/ClementCaton/ALGOSUP_2022_Project_3_A/blob/main/Informations/INFO%20mp3.md)<br>
Info on [**.Wav files**](https://github.com/ClementCaton/ALGOSUP_2022_Project_3_A/blob/main/Informations/INFO.md)<br>
Link to our [**Trello**](https://trello.com/b/itooTuBY/algosup2022project3a)<br>
Link to our [**Functional Specifications**](https://github.com/ClementCaton/ALGOSUP_2022_Project_3_A/blob/main/Reports/Functional%20specification.md)<br>
Link to our [**Technical Specifications**](https://github.com/ClementCaton/ALGOSUP_2022_Project_3_A/blob/main/Reports/Technical%20specification.md)<br>
Link to our [**Software Architecture Design Choices**](https://github.com/ClementCaton/ALGOSUP_2022_Project_3_A/blob/main/Reports/Software%20architecture%20design%20choices.md)

## Definitions:
[^1]: Octaves: A series of eight notes occupying the interval between (and including) two notes, one having twice or half the frequency of vibration of the other.

[^2]: Notes: A note is a symbol denoting a musical sound.