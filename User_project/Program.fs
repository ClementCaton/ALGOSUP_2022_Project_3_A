namespace Synthesizer

open System.IO
//open SFML.Audio
//open SFML.System
open System.Diagnostics // needed to play song on MAC OS
open System.Numerics
open Synthesizer



module Program =

    let synth = Synth()

    // Among Us Drip
    // https://musescore.com/user/5032516/scores/6519100
    let DottedQuarter = Custom (1./4. * 1.5)
    let TripletEighth = Custom (1./8. * 2./3.)
    synth.bpm <- 94.

    synth.waveType <- Triangular
    let music= synth.Compose [
        synth.Silence Quarter
        synth.Note Eighth Note.C 5
        synth.Note Eighth Note.Eb 5
        synth.Note Eighth Note.F 5
        synth.Note Eighth Note.Gb 5
        synth.Note Eighth Note.F 5
        synth.Note Eighth Note.Eb 5
        synth.Note DottedQuarter Note.C 5
        synth.Note Sixteenth Note.Bb 4
        synth.Note Sixteenth Note.D 5
        synth.Note Quarter Note.C 5
    ]

    synth.WriteToWav "amogus.wav" [music]
    // synth.platform <- false
    synth.PlayWav 5 [music] |> ignore