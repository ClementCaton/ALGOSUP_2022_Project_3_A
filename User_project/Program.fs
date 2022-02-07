namespace Synthesizer

open System.IO
//open SFML.Audio
//open SFML.System
open System.Diagnostics // needed to play song on MAC OS
open System.Numerics
open Synthesizer



module Program =
    let synth = new Synth()
    
    let music = synth.Compose [
        synth.Note Eighth Note.C 5
        synth.Note Eighth Note.Eb 5
        synth.Note Eighth Note.F 5
        synth.Note Eighth Note.Gb 5
        synth.Note Eighth Note.F 5
        synth.Note Eighth Note.Eb 5
        synth.Note Eighth Note.C 5
        synth.Note Sixteenth Note.Bb 4
        synth.Note Sixteenth Note.D 5
        synth.Note Quarter Note.C 5
    ]

    synth.WriteToWav "amogus.wav" [music]
    synth.PlayWav (float32 3) [music] |> ignore 

