namespace Synthesizer

open System.IO
//open SFML.Audio
//open SFML.System
open System.Diagnostics // needed to play song on MAC OS
open System.Numerics
open Synthesizer



module Program =
    
    let synth = Synth()
    let basic = synth.Add [synth.Note (Seconds 3.) Note.A 4; synth.Note (Seconds 3.) Note.A 3;] 
    
    synth.WriteToWav "basic.wav" [basic]
    let lfo = Filter.LFO_FM 2 1. basic
    synth.WriteToWav "lfo.wav" [lfo]
    
    // let filterAndWrite periode (step:float) n=
    //     printfn $"{n} {periode} {step}"
    //     printfn "____________________________________________________"
    //     let lfo = Filter.LFO_FM periode step basic
    //     synth.WriteToWav $"lfo{n}.wav" [lfo]
    
    // List.mapi (fun i x -> filterAndWrite (fst x) (snd x) i) [
    //     // (10, 1.);
    //     // (100, 1.);
    //     // (1000, 1.);
    //     // (10, 0.1);
    //     // (100, 0.1);
    //     //(1000, 0.1);
    //     (10, 5.);
    //     (100, 5.);
    //     (1000, 5.);
    // ] |> ignore
    
    (*
    let input = synth.Add [synth.Note Whole Note.A 2; synth.Note Whole Note.A 3; synth.Note Whole Note.A 4; synth.Note Whole Note.A 5]
    printfn "Wanted:   %A" [CalcNoteFreq(Note.A, 2).Output; CalcNoteFreq(Note.A, 3).Output; CalcNoteFreq(Note.A, 4).Output; CalcNoteFreq(Note.A, 5).Output]
    synth.WriteToWav "A345.wav" [input]
    let output = synth.Fourier 44100. input
    let freq = FrequencyAnalysis.LocalMaxValuesIndices 0.25 output
    let amplitudes = output |> Map.filter (fun f _ -> List.contains f freq)
    //printfn "%f %f %f %f" (List.average input) (List.average output) (List.sum input) (List.sum output)
    printfn "Obtained: %A" freq
    printfn "Amplitudes: %A" amplitudes
    synth.PreviewMap "A 3,4,5 Analysis" output |> ignore
    *)


    // Among Us Drip
    // https://musescore.com/user/5032516/scores/6519100
    // let DottedQuarter = Custom (1./4. * 1.5)
    // let TripletEighth = Custom (1./8. * 2./3.)
    // synth.bpm <- 94.
    // synth.waveType <- Triangular

    // let music = synth.Compose [
    //     synth.Note Eighth Note.C 5
    //     synth.Note Eighth Note.Eb 5
    //     synth.Note Eighth Note.F 5
    //     synth.Note Eighth Note.Gb 5
    //     synth.Note Eighth Note.F 5
    //     synth.Note Eighth Note.Eb 5
    //     synth.Note DottedQuarter Note.C 5
    //     synth.Note Sixteenth Note.Bb 4
    //     synth.Note Sixteenth Note.D 5
    //     synth.Note Quarter Note.C 5
    // ]

    // synth.WriteToWav "amogus.wav" [music]