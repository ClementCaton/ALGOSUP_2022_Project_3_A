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
### Creating audio data with an envelope
### Creating audio data with a custom envelope

## Finding frequencies from notes and octaves

A more simplified way to find the sound you are looking for is trought musical octaves[^1] and notes[^2].
To call on this form of notation you'll have to use the ``Synth.getNoteFreq octav note`` function to get the right frequency.

Example: 
```fs
let note = Synth.getNoteFreq Note.C 4 // This returns the frequency of the C4 note
```

Alternatively, you could directly create a SinWave using the ``Synth.note duration mNote octave``.

Example:
```fs
let note = Synth.note Half Note.C 4 // This returns the frequency a half duration of the C4 note
```

### Finding notes with a custom default frequency

In most cases, the frequency of a note is calculated from a default frequency (mostly, 440Hz for the A4 note).
However, in some cases, you might need to find a note from a different starting frequency.
This can be done using the ``Synth.getNoteFreqOffset octav note aFourFreq``

Example:
```fs
let note = Synth.getNoteFreqOffset Note.C 4 444. // This returns the frequency of the C4 note calculated from the starting point 444Hz at the A4 note
```

## Creating silence

Creating silence is as simple as calling the ``Synth.silence duration`` function.
```fs
let note = Synth.silence (Seconds 2) // Returns 2 seconds of silence
```

## Additioning audio data
### Additioning audio with a predefined ratio
## Composing

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

## Defenitions:
[^1]: Octaves: A series of eight notes occupying the interval between (and including) two notes, one having twice or half the frequency of vibration of the other.

[^2]: Notes: A note is a symbol denoting a musical sound.