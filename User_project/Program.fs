namespace Synthesizer

open System.IO
//open SFML.Audio
//open SFML.System
open System.Diagnostics // needed to play song on MAC OS
open System.Numerics
open Synthesizer



module Program =
    let filters = [
        Filter.lowPass 44100 500;
        Filter.highPass 44100 500;
        Filter.flanger 0.1 0.1 44100;
    ]

    let input = Synth.add [Synth.note Whole Note.A 2; Synth.note Whole Note.A 3; Synth.note Whole Note.A 4; Synth.note Whole Note.A 5]

    let a = Synth.ApplyFilters filters input

    printfn "Wanted:   %A" [CalcNoteFreq(Note.A, 2).Output; CalcNoteFreq(Note.A, 3).Output; CalcNoteFreq(Note.A, 4).Output; CalcNoteFreq(Note.A, 5).Output]
    Synth.writeToWav "A345.wav" [a]
    let output = frequencyAnalysis.fourier 44100. a
    let freq = frequencyAnalysis.localMaxValuesIndices 0.25 output
    let amplitudes = output |> Map.filter (fun f _ -> List.contains f freq)
    //printfn "%f %f %f %f" (List.average input) (List.average output) (List.sum input) (List.sum output)
    printfn "Obtained: %A" freq
    printfn "Amplitudes: %A" amplitudes
    Synth.previewMap "A 3,4,5 Analysis" output |> ignore
