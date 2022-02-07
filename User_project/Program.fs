namespace Synthesizer

open System.IO
//open SFML.Audio
//open SFML.System
open System.Diagnostics // needed to play song on MAC OS
open System.Numerics
open Synthesizer



module Program =
    let synth = Synth() // Init
    let basicSound = synth.Note (Seconds 1) Note.A 4

    let exampleCustomEnvelope (note:Note) (octav:int) (duration:float) =
        synth.SoundWithCustomEnveloppe (synth.GetNoteFreq note octav) (Seconds duration) Sin [(0., 0.); ((duration/2.), 1.); (duration, 0.)]

    let custEnvSound1 = exampleCustomEnvelope Note.A 4 1.
    let custEnvSound2 = exampleCustomEnvelope Note.B 6 2.
    let custEnvSound3 = exampleCustomEnvelope Note.D 3 3.

    synth.WriteToWav "basic.wav" [basicSound]
    synth.WriteToWav "custEnvSound1.wav" [custEnvSound1]
    synth.WriteToWav "custEnvSound2.wav" [custEnvSound2]
    synth.WriteToWav "custEnvSound3.wav" [custEnvSound3]
    