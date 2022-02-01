namespace Synthesizer

open System.IO
//open SFML.Audio
//open SFML.System
open System.Diagnostics // needed to play song on MAC OS
open System.Numerics
open Synthesizer



module Program =
    let a = Synth.note (Seconds 1) Note.A 4
    let b = Synth.note (Seconds 1) Note.B 4
    let c = Synth.note (Seconds 1) Note.C 4
    let d = Synth.note (Seconds 1) Note.D 4
    
    let full = Synth.compose 0 [a; b; c; d;]
    // let cut = Synth.cutEdge 44100. 1. 3. full
    let cut = Synth.cutMiddle 44100. 1. 1. full

    Synth.writeToWav "full.wav" [full]
    Synth.writeToWav "cut.wav" [cut]

    // let input = Synth.add [Synth.note Whole Note.A 2; Synth.note Whole Note.A 3; Synth.note Whole Note.A 4; Synth.note Whole Note.A 5]
    // printfn "Wanted:   %A" [CalcNoteFreq(Note.A, 2).Output; CalcNoteFreq(Note.A, 3).Output; CalcNoteFreq(Note.A, 4).Output; CalcNoteFreq(Note.A, 5).Output]
    // Synth.writeToWav "A345.wav" [input]
    // let output = frequencyAnalysis.fourier 44100. input
    // let freq = frequencyAnalysis.localMaxValuesIndices 0.25 output
    // let amplitudes = output |> Map.filter (fun f _ -> List.contains f freq)
    // //printfn "%f %f %f %f" (List.average input) (List.average output) (List.sum input) (List.sum output)
    // printfn "Obtained: %A" freq
    // printfn "Amplitudes: %A" amplitudes
    // Synth.previewMap "A 3,4,5 Analysis" output |> ignore
