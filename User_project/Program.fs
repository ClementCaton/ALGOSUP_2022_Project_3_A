namespace Synthesizer

open System.IO
//open SFML.Audio
//open SFML.System
open System.Diagnostics // needed to play song on MAC OS
open System.Numerics
open Synthesizer



module Program =
    let synth = Synth() // Init

    let exampleCustomEnvelope (data:List<float>) (sampleRate:float) =
        Filter.CustomEnvelope [(0., 0.); ((float data.Length / sampleRate / 2.), 1.); ((float data.Length / sampleRate), 0.)] sampleRate data

    let custEnvSound1 = exampleCustomEnvelope (synth.Note (Seconds 1) Note.A 4) 44100.
    let custEnvSound2 = exampleCustomEnvelope (synth.Note (Seconds 2) Note.B 4) 44100.
    let custEnvSound3 = exampleCustomEnvelope (synth.Note (Seconds 3) Note.C 4) 44100.

    synth.WriteToWav "custEnvSound1.wav" [custEnvSound1]
    synth.WriteToWav "custEnvSound2.wav" [custEnvSound2]
    synth.WriteToWav "custEnvSound3.wav" [custEnvSound3]
