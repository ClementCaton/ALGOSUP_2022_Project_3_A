# ALGOSUP_2022_Project_3_A | Sound Synthesizer

## **Project:**

The project given by [*Algosup*](https://www.algosup.com/fr/index.html) and [*Robert Pickering*](https://github.com/robertpi) was to create a Sound Synthesizer able to open, modify, create and save sounds.

## **Getting Started**

<br>

## **Project members**

[*Ivan Molnar*](https://github.com/ivan-molnar) <br>
[*Clement Caton*](https://github.com/ClementCaton) <br>
[*Louis de Choulot*](https://github.com/Louis-de-Lavenne-de-Choulot) <br>
[*Théo Diancourt*](https://github.com/TheoDct) <br>
[*Mathieu Chaput*](https://github.com/Chaput-Mathieu) <br>
[*Léo Chartier*](https://github.com/leo-chartier)

# Project documentation

## Prerequisites

- Download .Net 6.0 or newer

<br>

## .Net CLI

``dotnet add package Synthesizer --version 1.1.0``

<br>

## Windows

``Install-Package Synthesizer -Version 1.1.0``

<br>

## Package Reference

``<PackageReference Include="Synthesizer" Version="1.1.0" />``

## **Installation**

## **Basic structure**

To interract with the library you'll have to mainly interract with two objects.
The ``Synth`` object which is the actual sound synthesizer and the ``Filter`` object which contains a list of function that will allow you to modify the created sounds.

## **Reading files**

## Reading wav files

You can extract data from a wav file in the default ``/Output/`` folder using ``Synth.ReadFromWav (fileName:string)``.

You can open it from your own path using ``Synth.readFromWavWithPath (filePath:string)``.

These functions return a tuple containing the ``soundData:list<list<float>>``, ``duration:float``, ``sampleRate:int`` and the ``bitsPerSample:int``.

Example:

```fs
let inOutputData, inOutputDuration, inOutputSampleRate, inOutputBPSampleRate = synth.ReadFromWav "yourFileName.wav"     // get everything from a file in the Output folder

let fromPathData, _, fromPathSampleRate, _ = Synth.readFromWavWithPath "/yourPath/yourFileName.wav"     // get only the sound data and the samplerate from a predefined path
```

## Reading mp3 files

<span style="color: red;">WIP</span>

<!-- You can extract data from a wav file in the default ``/Output/`` folder using ``Synth.ReadFromMp3 name.mp3``

You can open it from your own path using ``readFromWavWithPath /path-to.mp3`` -->

## **Writing to files / Saving**

## Writing wav files

You can save files by writing data into them with the function ``Synth.WriteToWav name music``. This function will put files in the folder "./Output".

Example :

```fs
Synth.WriteToWavWithPath "name.wav" sound// This will save the sound in the file from the path "./Output/name.wav".
```

You can also save files by writing data into them with the function ``Synth.WriteToWavWithPath path fileName music``. This function will put files in "path/fileName".

Example :

```fs
Synth.WriteToWavWithPath "./folder/" "name.wav" sound // This will save the sound in the file from the path "./folder/name.wav".
```

## Writing mp3 files

<span style="color: red;">WIP</span>

## **Playing music**

Your Os is automatically detected to use either SFML on windows or afplay on Mac, this function does not support Linux yet.

You can play music from the code ``Synth.PlayWav (offset:float32) data``.

Example :

```fs
Synth.PlayWav (float32 0.) data // This will play the sound in the variable data with an offset of 0 second.
```

You can also play music from a file with ``Synth.PlayWavFromPath offset (filePath:string)``

Example :

```fs
Synth.PlayWavFromPath (float32 0.) "./Output/name.wav" // This will play the sound in the file from the path "./Output/name.wav" with an offset of 0 second.
```

## **Dealing with stereo**

<span style="color: red;">WIP</span>

## **Creating audio data**

You can create some basic audio using ``Synth.Sound (frequency:float) (duration:Duration) (waveType:BaseWaves)``

Example:
```fs
let synth = Synth() // Init
let newSound = synth.Sound 440. (Seconds 1.) Sin    // Create a 1 second sinwave with a frequence of 440.
let newSound2 synth.Sound (synth.getNoteFreq 3 Note.F) Half Triangular // Create a triangular F3 half note.
```

Alternatively, it is possible to directly create a note with the ``synth.Note (duration:Duration) (mNote:Note) (octave:int)``.

Example:
```fs
let synth = Synth() // Init
let newNote = synth.Note Quarter Note.D 5 // Create a D5 quarter note.
```

## Creating audio data with an envelope

In order to create a sound with an enveloppe you need to use ``Synth.SoundWithEnveloppe

## Creating audio data with a custom envelope

<span style="color: red;">WIP</span>

## **Finding frequencies from notes and octaves**

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

## Finding notes with a custom default frequency

In most cases, the frequency of a note is calculated from a default frequency (mostly, 440Hz for the A4 note).
However, in some cases, you might need to find a note from a different starting frequency.
This can be done using the ``Synth.getNoteFreqOffset (octav:int) (note:Note) (aFourFreq:Float)``

Example:

```fs
let Note = Synth.GetNoteFreqOffset Note.C 4 444. // This returns the frequency of the C4 note calculated from the starting point 444Hz at the A4 note
```

## **Creating silence**

Creating silence is as simple as calling the ``Synth.silence (duration:Duration)`` function.

```fs
let Silence = Synth.Silence (Seconds 2) // Returns 2 seconds of silence
```

## **Cutting audio**

Cutting audio is simple. You can use the following functions

- ``Synth.cutStart (sampleRate:float) (time:float) (data:List<float>)`` : Cuts the start of the audio data returning the end part
  
- ``Synth.cutEnd (sampleRate:float) (time:float) (data:List<float>)`` : Cuts the end of the audio data returning the first part
  
- ``Synth.cutMiddle (sampleRate:float) (timeStart:float) (timeEnd:float) (data:List<float>)`` : Cuts out the middle of the audio data and returns the edges merged together
  
- ``Synth.cutEdge (sampleRate:float) (timeStart:float) (timeEnd:float) (data:List<float>)`` : Cuts of both ends of the audio data and returns the middle part

Example:

```fs
let a = Synth.note (Seconds 1) Note.A 4
let b = Synth.note (Seconds 1) Note.B 4
let c = Synth.note (Seconds 1) Note.C 4
let d = Synth.note (Seconds 1) Note.D 4

let full = Synth.compose 0 [a; b; c; d;]        // Complete sound, takes 4 second and plays 4 different notes

let lastThree = Synth.cutStart 44100. 1. full   // Cuts first note, leaving last 3
let firstThree = Synth.cutEnd 44100. 1. full    // Cuts last note, leaving first 3
let edges = Synth.cutMiddle 44100. 1. 1. full   // Cuts out the 2 middle notes, leaving the first and the last ones
let second = Synth.cutMiddle 44100. 1. 2. full  // Cuts the first and the last 2 notes, leaving the second one
```

## **Additioning audio data**

<span style="color: red;">WIP</span>

## Additioning audio with a predefined ratio

<span style="color: red;">WIP</span>

## **Composing**

One thing you have to be aware of is the ``cutCorners`` function.
When we first created the compose function we encounterd a strange, small sound between easch end every note.
This sound was caused by the notes ending on a not-zero amplitude.

The solution was to add in a filter that gradually lowers the amplitude of the notes start and end to 0.

|          Before cutCorne             |          After cutCorner            |
|:------------------------------------:|:-----------------------------------:|
| ![Before](Reports/Files/cut_b.png)  | ![After](Reports/Files/cut_a.png)  |
<sup>* for the shake of the example, the filter has been exagerated</sup>

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

Or with ``composeNoCutCorner``:

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

## **Preview**

Its possible to create a preview of ant audio loaded into the filter using the ``Synth.preview (title:string) (sound:List<float>)`` function.

Example:

```fs
let basic = Synth.note Whole Note.A 2       // reating a basic note
let cut = Utility.cutCorners 5000 basic     // Making it look a bit more interresting

Synth.preview "Example" cut |> ignore       // Launch preview
```

The above example automatically opens the browser with the following image:
![Preview](Reports/Files/preview.png)

Tools to zoom/zoom out are also present on the page.

## **Frequency analysis**

<span style="color: red;">WIP</span>

## **Filters**

## Usable Filters

To complement your sounds you can add some filters :

- Flanger

- Echo

- Reverb

- Envelope

- LFO AM

- LFO FM

- Low Pass

- High Pass

## Apply multiple filters at once

<span style="color: red;">WIP</span>

## Changing amplitude

<span style="color: red;">WIP</span>

## Reverb

<span style="color: red;">WIP</span>

## Echo

<span style="color: red;">WIP</span>

## Custom repeater filter

<span style="color: red;">WIP</span>

## Frequency analysis

## Flanger

<span style="color: red;">WIP</span>

## Envelope

<span style="color: red;">WIP</span>

## Custom envelope

<span style="color: red;">WIP</span>

## Low frequency oscillation

<span style="color: red;">WIP</span>

### AM

<span style="color: red;">WIP</span>

### FM

<span style="color: red;">WIP</span>

## LowPass / HighPass / BandPass / RejectBand filters

<span style="color: red;">WIP</span>

# Footnotes

## Usable notes

The musical notes available are:
> ``C``,  ``Cs / Db``, ``D``, ``Ds / Eb``, ``E``, ``F``, ``Fs / Gb``, ``G``, ``Gs / Ab``, ``A``, ``As / Bb``, ``B``

## Possible Waves

The wave types available are:
> ``Sin``, ``Square``, ``Triangular``, ``Saw``, ``Silence``, ``CustomInstrument``

- The ``CustomInstrument`` value has a value of ``(float -> float -> float -> float -> float -> float)``. This is because the wave functions need to be written as:

```fs
let waveFunc (frequency:float) (amplitude:float) (verticalShift:float) (phaseShift:float) (timeLength:float) 
```

## Duration of elements

The note durations available are:
> ``Whole``, ``Half``, ``Quarter``, ``Eighth``, ``Sixteenth``, ``Custom``, ``Seconds``

- The Seconds value takes a float as argument.
- The Custom value takes a float as its argument. This translates using the formula ``value *4.* 60. / bpm``.
- The tickspead of the durations can be changed by changing the value ``Synth.bpm`` (default 90).

## see also

Info on [**.mp3 files**](https://github.com/ClementCaton/ALGOSUP_2022_Project_3_A/blob/main/Informations/INFO%20mp3.md)<br>
Info on [**.Wav files**](https://github.com/ClementCaton/ALGOSUP_2022_Project_3_A/blob/main/Informations/INFO.md)<br>
Link to our [**Trello**](https://trello.com/b/itooTuBY/algosup2022project3a)<br>
Link to our [**Functional Specifications**](https://github.com/ClementCaton/ALGOSUP_2022_Project_3_A/blob/main/Reports/Functional%20specification.md)<br>
Link to our [**Technical Specifications**](https://github.com/ClementCaton/ALGOSUP_2022_Project_3_A/blob/main/Reports/Technical%20specification.md)<br>
Link to our [**Software Architecture Design Choices**](https://github.com/ClementCaton/ALGOSUP_2022_Project_3_A/blob/main/Reports/Software%20architecture%20design%20choices.md)

## **Definitions**

[^1]: Octaves: A series of eight notes occupying the interval between (and including) two notes, one having twice or half the frequency of vibration of the other.

[^2]: Notes: A note is a symbol denoting a musical sound.

<https://user-images.githubusercontent.com/91249762/152002722-5442f1d9-fe37-4373-a82c-815790e3420b.mov>
